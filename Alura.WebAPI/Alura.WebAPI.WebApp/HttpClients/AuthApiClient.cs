using Alura.ListaLeitura.Seguranca;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class AuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> PostLoginAsync(LoginModel model)
        {
            //Posso deixar o proprio asp.net serializar esse modelo com Json ao inves de adicionar um CreateMultipartFormDataContent
            var resposta = await _httpClient.PostAsJsonAsync("login", model);
            var excecao = resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsStringAsync(); ;
        }
    }
}
