using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _acessor;

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _acessor = accessor;
        }

        private void AddBearerToken()
        {
            //Dentro de uma action eu tenho acesso ao contexto de uma requisição
            //HttpContext.User => É o usuario principal associado a esta requisição
            //HttpContext.User.Claims = esse usuario principal tb tem as politica que adicionamos
            //Posso usar o linq para pegar esse token
            //Type é o tipo que definimos nas claims. Estou guardando o token numa variável
            //A classe HttpContext pertence a classes do framework asp.net core, para acessar
            //essa classe em outr que nao faz parte do framework, podemos fazer isso atraves
            //da interface IHttpContextAccessor
            var token = _acessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            //Codigo para adicionar o cabeçalho de autorização na requisição
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            AddBearerToken();

            var resposta = await _httpClient.GetAsync($"ListasLeitura/{tipo}");
            var retorno = resposta.Content.ReadAsStringAsync();
            resposta.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<Lista>(await resposta.Content.ReadAsStringAsync());
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            AddBearerToken();

            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}");

            response.EnsureSuccessStatusCode();

            var objeto = JsonConvert.DeserializeObject<LivroApi>(await response.Content.ReadAsStringAsync());

            return objeto;
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            AddBearerToken();

            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}/capa");

            response.EnsureSuccessStatusCode();

            var objeto = await response.Content.ReadAsByteArrayAsync();

            return objeto;
        }

        public async Task DeleteLivroAsync(int id)
        {
            AddBearerToken();

            var resposta = await _httpClient.DeleteAsync($"livros/{id}");
            resposta.EnsureSuccessStatusCode();
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            AddBearerToken();

            //A classe HttpContent representa conteúdos que serão enviados em requisições
            //HttpContent é um tipo abstrato. Não posso criar objetos a partir dele, posso criar filhos
            //Queremos criar o filho multipart/formdata
            HttpContent content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PostAsync("livros", content);
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
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveAspasDuplas("lista"));

            if (!string.IsNullOrEmpty(model.Subtitulo))
            {
                content.Add(new StringContent(model.Subtitulo), EnvolveAspasDuplas("subtitulo"));
            }

            if (!string.IsNullOrEmpty(model.Resumo))
            {
                content.Add(new StringContent(model.Resumo), EnvolveAspasDuplas("resumo"));
            }

            if (!string.IsNullOrEmpty(model.Autor))
            {
                content.Add(new StringContent(model.Autor), EnvolveAspasDuplas("autor"));
            }

            //Quando eu crio um conteúdo para alteração de um livro eu preciso passar o id do livro
            if(model.Id > 0)
            {
                content.Add(new StringContent(model.Id.ToString()), EnvolveAspasDuplas("id"));
            }

            if(model.Capa != null)
            {
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                //Um byte nao é um formato especifico, entao temos que definir um formato pro arquivo (image/png ou pdf e etc)
                imagemContent.Headers.Add("content-type", "image/png");
                content.Add(
                    imagemContent, 
                    EnvolveAspasDuplas("capa"),
                    EnvolveAspasDuplas("capa.png")  //Pra fazer upload tem que ter: nome arquivo, tipo do arquivo
                    );
            }

            return content;
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBearerToken();

            HttpContent content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PutAsync("livros", content);
            resposta.EnsureSuccessStatusCode();
        }
    }
}
