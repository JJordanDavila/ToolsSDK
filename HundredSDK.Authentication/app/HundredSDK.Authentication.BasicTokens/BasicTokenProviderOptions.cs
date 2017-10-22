using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HundredSDK.Authentication.BasicTokens
{
    public class BasicTokenProviderOptions
    {
        public string Path { get; set; } = "/token";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
        public SigningCredentials SigningCredentials { get; set; }
        public Func<Task<string>> NonceGenerator { get; set; }
        = () => Task.FromResult(Guid.NewGuid().ToString());
    }
}
