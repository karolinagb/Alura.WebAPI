using Alura.ListaLeitura.Modelos;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}");

            response.EnsureSuccessStatusCode();

            var objeto = JsonConvert.DeserializeObject<LivroApi>(await response.Content.ReadAsStringAsync());

            return objeto;
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}/capa");

            response.EnsureSuccessStatusCode();

            var objeto = await response.Content.ReadAsByteArrayAsync();

            return objeto;
        }

        public async Task DeleteLivroAsync(int id)
        {
            var resposta = await _httpClient.DeleteAsync($"livros/{id}");
            resposta.EnsureSuccessStatusCode();
        }
    }
}
