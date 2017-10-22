using HundredSDK.Authentication.BasicTokens.TestWebApi.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HundredSDK.Authentication.BasicTokens.TestWebApi.Model.Services
{
    public class SeguridadService : ISeguridadService
    {
        public bool Autenticar(Usuario usuario)
        {
            if (usuario.Login == "ALFREDO" && usuario.Password == "password123") {
                return true;
            }
            return false;
        }
    }
}
