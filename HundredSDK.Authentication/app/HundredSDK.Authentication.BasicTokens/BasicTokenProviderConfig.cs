using System;
using System.Collections.Generic;
using System.Text;

namespace HundredSDK.Authentication.BasicTokens
{
    public struct BasicTokenProviderConfig
    {
        public const string BaseSectionName = "HundredTokenAuthentication";
        public const string SecretKeySectionName = BaseSectionName + ":SecretKey";
        public const string IssuerSectioName = BaseSectionName + ":Issuer";
        public const string AudienceSectioName = BaseSectionName + ":Audience";
        public const string TokenPathSectionName = BaseSectionName + ":TokenPath";
        public const string CookieSectionName = BaseSectionName + ":CookieName";
    }
}
