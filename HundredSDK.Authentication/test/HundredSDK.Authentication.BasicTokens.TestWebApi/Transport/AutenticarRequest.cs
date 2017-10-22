using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HundredSDK.Authentication.BasicTokens.TestWebApi.Transport
{
    public class AutenticarRequest
    {
        public string Login { get; set; }
        public string Token { get; set; }
        public TimeSpan TokenExpiration { get; set; }
    }
}
