using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HundredSDK.Authentication.BasicTokens
{
    public class BasicTokenProvider
    {
        public Func<string, string, Task<ClaimsIdentity>> IdentityResolver { get; set; }

        private readonly IConfiguration configuration;

        private const string baseSectionName = "HundredTokenAuthentication";
        private const string secretKeySectionName = baseSectionName + ":SecretKey";
        private const string issuerSectioName = baseSectionName + ":Issuer";
        private const string audienceSectioName = baseSectionName + ":Audience";
        private const string tokenPathSectionName = baseSectionName + ":TokenPath";
        private const string cookieSectionName = baseSectionName + ":CookieName";

        public BasicTokenProvider(IConfiguration configuration) {
            this.configuration = configuration;            
        }

        public async Task<string> GetToken(string userName)
        {
            var options = this.GetProviderOptions();

            this.Validate(options);

            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                new Claim (JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var jwt = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(options.Expiration),
                signingCredentials: options.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        
        public BasicTokenProviderOptions GetProviderOptions()
        {
            var secretKey = this.GetSecretKey();

            var hundredTokenProviderOptions = new BasicTokenProviderOptions
            {
                Path = configuration.GetSection(tokenPathSectionName).Value,
                Audience = configuration.GetSection(audienceSectioName).Value,
                Issuer = configuration.GetSection(issuerSectioName).Value,
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                //IdentityResolver = identityResolver
            };

            return hundredTokenProviderOptions;
        }

        public  SymmetricSecurityKey GetSecretKey()
        {
            //Obtener secretKey
            var signingKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(this.configuration.GetSection(secretKeySectionName).Value));

            return signingKey;
        }

        public  TokenValidationParameters GetValidationParameters()
        {
            var signingKey = this.GetSecretKey();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = this.configuration.GetSection(issuerSectioName).Value,
                ValidateAudience = true,
                ValidAudience = this.configuration.GetSection(audienceSectioName).Value,
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            return tokenValidationParameters;
        }
        
        private void Validate(BasicTokenProviderOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(BasicTokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(BasicTokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(BasicTokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(BasicTokenProviderOptions.Expiration));
            }
            
            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(BasicTokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(BasicTokenProviderOptions.NonceGenerator));
            }
        }

    }
}
