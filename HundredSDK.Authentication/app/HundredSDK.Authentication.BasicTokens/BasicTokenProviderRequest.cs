using System;
using System.Collections.Generic;
using System.Text;

namespace HundredSDK.Authentication.BasicTokens
{
    public class BasicTokenProviderRequest
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
        public string UserName { get; set; }
        
    }
}
