using Alura.ListaLeitura.Modelos;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            var resposta = await _httpClient.GetAsync($"ListasLeitura/{tipo}");
            return JsonConvert.DeserializeObject<Lista>(await resposta.Content.ReadAsStringAsync());
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

        public async Task PostLivroAsync(LivroUpload model)
        {
            //A classe HttpContent representa conteúdos que serão enviados em requisições
            //HttpContent é um tipo abstrato. Não posso criar objetos a partir dele, posso criar filhos
            //Queremos criar o filho multipart/formdata
            HttpContent content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PostAsync("livros", content);
            var retorno = resposta.Content.ReadAsStringAsync();
            resposta.EnsureSuccessStatusCode();
        }

        //Metodo para colocar as aspas duplas no content para nao ter erros
        private string EnvolveAspasDuplas(string valor)
        {
            return $"\"{valor}\"";
        }

        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            //Cada campo do formulario vai ser uma parte desse conteúdo
            content.Add(new StringContent(model.Titulo), EnvolveAspasDuplas("titulo"));  //Preciso dizer que esse campo é envolvido com aspas duplas
            content.Add(new StringContent(model.Subtitulo), EnvolveAspasDuplas("subtitulo"));
            content.Add(new StringContent(model.Resumo), EnvolveAspasDuplas("resumo"));
            content.Add(new StringContent(model.Autor), EnvolveAspasDuplas("autor"));
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveAspasDuplas("lista"));

            if(model.Capa != null)
            {
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                //Um byte nao é um formato especifico, entao temos que definir um formato pro arquivo (image/png ou pdf e etc)
                imagemContent.Headers.Add("content-type", "image/png");
                content.Add(imagemContent, EnvolveAspasDuplas("capa"));
            }

            return content;
        }
    }
}
