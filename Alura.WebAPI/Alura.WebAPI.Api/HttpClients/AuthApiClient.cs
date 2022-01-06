using Alura.ListaLeitura.Seguranca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.HttpClients
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }


    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResult> PostLoginAsync(LoginModel model)
        {
            //Posso deixar o proprio asp.net serializar esse modelo com Json ao inves de adicionar um CreateMultipartFormDataContent
            var resposta = await _httpClient.PostAsJsonAsync("login", model);

            return new LoginResult
            {
                Succeeded = resposta.IsSuccessStatusCode,
                Token = await resposta.Content.ReadAsStringAsync()
            };
        }
    }
}
