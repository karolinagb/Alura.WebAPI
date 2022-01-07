using Alura.ListaLeitura.Seguranca;
using Alura.WebAPI.Api.HttpClients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthApiClient _auth;

        public AuthController(AuthApiClient auth)
        {
            _auth = auth;
        }

        [HttpPost]
        public async Task<LoginResult> Login(LoginModel model)
        {
            var result = await _auth.PostLoginAsync(model);
            return result;
        }
    }
}
