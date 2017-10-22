using HundredSDK.Authentication.BasicTokens.TestWebApi.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HundredSDK.Authentication.BasicTokens.TestWebApi.Model.Services
{
    public interface ISeguridadService
    {
        bool Autenticar(Usuario usuario);
    }
}
