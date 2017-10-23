using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HundredSDK.Authentication.BasicTokens
{
    public static class BasicTokenProviderExtension
    {
        
        public static IApplicationBuilder UseHundredTokenProvider(
            this IApplicationBuilder builder, IConfiguration configuration, IMemoryCache cache)
        {
            ValidarConfiguracion(configuration);
            BasicTokenProvider tokenProvider = new BasicTokenProvider(configuration);
            return builder.UseMiddleware<BasicTokenProviderMiddleware>(configuration, tokenProvider, cache);
        }

        //Generar tokens previa validación
        public static IApplicationBuilder UseHundredTokenProvider(
            this IApplicationBuilder builder, IConfiguration configuration,
            Func<string, string, Task<ClaimsIdentity>> identityResolver)
        {
            ValidarConfiguracion(configuration);
            BasicTokenProvider tokenProvider = new BasicTokenProvider(configuration);
            tokenProvider.IdentityResolver = identityResolver;
            return builder.UseMiddleware<BasicTokenProviderMiddleware>(configuration, tokenProvider);
        }

        public static void AddAuthenticationHundredBasicToken(this IServiceCollection services, IConfiguration configuration) {
            ValidarConfiguracion(configuration);

            BasicTokenProvider tokenProvider = new BasicTokenProvider(configuration);

            //Obtener secretKey
            var signingKey = tokenProvider.GetSecretKey();

            //Validaciones de Token
            var tokenValidationParameters = tokenProvider.GetValidationParameters();
            
            //Agregar autenticación
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParameters; })
            .AddCookie(options => {
                options.Cookie.Name = configuration.GetSection(BasicTokenProviderConfig.CookieSectionName).Value;
                options.TicketDataFormat = new JwtDataFormat(
                    SecurityAlgorithms.HmacSha256, tokenValidationParameters);
            });

            //Agregar Memory Caching
            services.AddMemoryCache();
            
        }

        private static void ValidarConfiguracion(IConfiguration configuration) {
            var seccion = configuration.GetSection(BasicTokenProviderConfig.BaseSectionName).Value;
            if (seccion == null) {
                throw new ApplicationException("No existe configuración. Revisar la sección 'HundredTokenAuthentication' en AppSettings.json or web.config");
            }
        }
        
    }
}
