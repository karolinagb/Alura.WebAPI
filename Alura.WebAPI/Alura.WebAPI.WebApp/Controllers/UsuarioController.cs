using Alura.ListaLeitura.HttpClients;
using Alura.ListaLeitura.Seguranca;
using Alura.ListaLeitura.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AuthApiClient _auth;

        public UsuarioController(AuthApiClient auth)
        {
            _auth = auth;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _auth.PostLoginAsync(model);
                //var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);
                if (result.Succeeded)
                {

                    //onde guardar o token? 
                    //através de um cookie de autenticação - link do MS Docs

                    //primeiro vamos criar os direitos/reinvindicações/claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Login),
                        new Claim("Token", result.Token) //em uma claim eu guardo o token!
                    };

                    //e guardar esses direitos na identidade principal
                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    var authProp = new AuthenticationProperties
                    {
                        IssuedUtc = DateTime.UtcNow,
                        //configurar expiração do cookie para um valor menor que a expiração do token
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(25),
                        IsPersistent = true
                    };

                    //e finalmente autenticar via cookie com essa identidade
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProp);


                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(String.Empty, "Erro na autenticação");
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = new Usuario { UserName = model.Login };
                //var result = await _userManager.CreateAsync(user, model.Password);
                //if (result.Succeeded)
                //{

                //    await _signInManager.SignInAsync(user, isPersistent: false);
                //    return RedirectToAction("Index", "Home");
                //}
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}