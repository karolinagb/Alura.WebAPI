using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<Usuario> _signInManager;

        public LoginController(SignInManager<Usuario> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Token(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors));
            }

            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, true, true);
            if (result.Succeeded)
            {
                //cria token (header + payload (claims/direitos) + signature)

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Login), //Sub é o sujeito, o login do usuario que vai receber o token
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  //Gerar um token cm id unico
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("alura-webapi-authentication-valid"));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var informacoesToken = new JwtSecurityToken(
                    issuer: "Alura.WebApp",
                    audience: "Postman",
                    claims: claims,
                    signingCredentials: credentials,
                    expires: DateTime.Now.AddMinutes(30)
                    );

                var token = new JwtSecurityTokenHandler().WriteToken(informacoesToken);

                return Ok(token);
            }
            return Unauthorized(); //401
        }
    }
}
