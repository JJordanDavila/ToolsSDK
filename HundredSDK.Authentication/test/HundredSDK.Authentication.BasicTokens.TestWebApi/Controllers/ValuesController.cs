using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace HundredSDK.Authentication.BasicTokens.TestWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IMemoryCache cache;
        public ValuesController(IMemoryCache cache) {
            this.cache = cache;
        }
        // GET api/values
        [HttpGet, Authorize]
        public IEnumerable<string> Get()
        {
                       
            var userName= this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            
            

            return new string[] { "value1", "value2", userName };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {


            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
