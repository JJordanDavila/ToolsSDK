using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using HundredSDK.Authentication.BasicTokens.TestWebApi.Model.Domain;
using HundredSDK.Authentication.BasicTokens.TestWebApi.Model.Services;
using HundredSDK.Authentication.BasicTokens.TestWebApi.Transport;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HundredSDK.Authentication.BasicTokens.TestWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SeguridadController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ISeguridadService seguridadService;
        private readonly IMemoryCache cache;

        public SeguridadController(IConfiguration configuration, ISeguridadService seguridadService, IMemoryCache cache) {
            this.configuration = configuration;
            this.seguridadService = seguridadService;
            this.cache = cache;
        }
        // POST api/values
        [HttpPost]
        public async Task<string> GetToken([FromBody]string userName)
        {
            //Invocar a la generación de tokens
            BasicTokenProvider tokenProvider = new BasicTokenProvider(this.configuration);

            var token = await tokenProvider.GetToken(userName);
            
            return token;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Autenticar([FromBody]Usuario usuario)
        {
            try
            {
                BasicTokenProviderRequest tokenRequest = new BasicTokenProviderRequest();

                var valido = this.seguridadService.Autenticar(usuario);
                //validar credenciales
                if (valido) {
                    //Generar token

                    using (var client = new HttpClient())
                    {
                        var strUrl = this.HttpContext.Request.GetDisplayUrl();
                        strUrl = strUrl.Replace(this.HttpContext.Request.Path, "");
                        var urlToken = "/api/token";
                        client.BaseAddress = new Uri(strUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                        var content = new FormUrlEncodedContent(new[]
                        {                            
                            new KeyValuePair<string, string>("userName" , usuario.Login),
                            new KeyValuePair<string, string>("password" , usuario.Password),
                        });
                        var result = await client.PostAsync(urlToken, content);
                        
                        var data = result.Content.ReadAsStringAsync();

                        tokenRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<BasicTokenProviderRequest>(data.Result);
                                                
                    }

                }

                var response = new
                {
                    Valido = valido,
                    Login = usuario.Login,
                    Acceso= tokenRequest                    
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
          
        }

        [Authorize]
        public  IActionResult CerrarSesion([FromBody]string userName) {

            var valoresToken = (Dictionary<string, BasicTokenProviderRequest>) this.cache.Get("TOKEN_COLLECTION");

            valoresToken.Remove(userName);
            

            return Ok(valoresToken);
        }
    }
}
