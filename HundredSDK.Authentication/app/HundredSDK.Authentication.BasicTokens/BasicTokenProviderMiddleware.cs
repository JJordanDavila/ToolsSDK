using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace HundredSDK.Authentication.BasicTokens
{
    public class BasicTokenProviderMiddleware
    {
        private readonly RequestDelegate next;
        private readonly BasicTokenProvider tokenProvider;
        private readonly JsonSerializerSettings serializerSettings;
        private const string keyTokenCollection = "TOKEN_COLLECTION";
        private readonly MemoryCacheHelper cacheHelper;
        public BasicTokenProviderMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            BasicTokenProvider tokenProvider,
            IMemoryCache cache
            )
        {
            this.next = next;
            this.tokenProvider = tokenProvider;

            var options = this.tokenProvider.GetProviderOptions();

            this.cacheHelper = new MemoryCacheHelper(cache, TimeSpan.FromSeconds(options.Expiration.TotalSeconds) );
           
            this.serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context)
        {
            var options = this.tokenProvider.GetProviderOptions();

            var headerAuthorization = context.Request.Headers["Authorization"].FirstOrDefault();
            string keyToken = string.Empty;
            string keyBearer = "Bearer ";
            if (headerAuthorization != null && headerAuthorization.Length > 0) {
                keyToken = headerAuthorization.Replace(keyBearer, "");
                
                var tokenValido = false;

                if (this.cacheHelper.Exists(keyTokenCollection)) {
                    var dictionary = (Dictionary<string, BasicTokenProviderRequest>) this.cacheHelper.Get(keyTokenCollection);
                    tokenValido= dictionary.ContainsKey(keyToken);
                }

                if (!tokenValido) {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return context.Response.WriteAsync("Unauthorized");
                }
            }
            
            if (!context.Request.Path.Equals(options.Path, StringComparison.Ordinal))
            {
                return this.next(context);
            }

            if (context.Request.Method.Equals("POST") && context.Request.HasFormContentType)
                return GetToken(context);

            context.Response.StatusCode = 400;
            return context.Response.WriteAsync("Bad request");

        }

        private async Task GetToken(HttpContext context)
        {
            var userName = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            var options = this.tokenProvider.GetProviderOptions();

            if (this.tokenProvider.IdentityResolver != null)
            {
                var identity = await this.tokenProvider.IdentityResolver(userName, password);

                if (identity == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid username or password");
                    return;
                }
            }
                        
            var encodedJwt = this.tokenProvider.GetToken(userName);
            

            Dictionary<string, BasicTokenProviderRequest> dictionary = new Dictionary<string, BasicTokenProviderRequest>();

            //Obtener de cache
            if (this.cacheHelper.Exists(keyTokenCollection))
                dictionary= (Dictionary<string, BasicTokenProviderRequest>)this.cacheHelper.Get(keyTokenCollection);
            
            dictionary.Add(encodedJwt.Result, new BasicTokenProviderRequest
            {
                Token = encodedJwt.Result,
                ExpiresIn = 0,
                UserName = userName
            });

            //Guardar InMemory Cache
            this.cacheHelper.Add(keyTokenCollection, dictionary);

            var response = new BasicTokenProviderRequest
            {
                Token = encodedJwt.Result,
                ExpiresIn = (int)options.Expiration.TotalSeconds
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, this.serializerSettings));

        }
        



    }
}
