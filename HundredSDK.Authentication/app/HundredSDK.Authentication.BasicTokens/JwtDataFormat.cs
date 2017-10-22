using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HundredSDK.Authentication.BasicTokens
{
    public class JwtDataFormat: ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string algoritmo;
        private readonly TokenValidationParameters validationParameters;
        public JwtDataFormat(string algoritmo, TokenValidationParameters tokenValidationParameters)
        {
            this.algoritmo = algoritmo;
            this.validationParameters = tokenValidationParameters;
        }
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        => Unprotect(protectedText, null);

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            try
            {
                SecurityToken validToken = null;
                principal = handler.ValidateToken(protectedText, this.validationParameters, out validToken);
                var validJwt = validToken as JwtSecurityToken;

                System.Diagnostics.Debug.WriteLine("Excute Unprotect");

                if (validJwt == null)
                {
                    throw new ArgumentException("JWT no válido");
                }
                if (validJwt.Header.Alg.Equals(this.algoritmo, StringComparison.Ordinal))
                {
                    throw new ArgumentException($"EL algoritmo debe ser '{this.algoritmo}'");
                }
            }
            catch (SecurityTokenValidationException ex)
            {
                throw ex;
            }

            catch (ArgumentException ex)
            {
                throw ex;
            }

            return new AuthenticationTicket(principal, new AuthenticationProperties(), "Cookie");

        }
    }
}
