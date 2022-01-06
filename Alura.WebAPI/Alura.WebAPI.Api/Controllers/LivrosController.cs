using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace Alura.ListaLeitura.Api.Controllers
{


    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    //Dizer o que deve aparecer no documento. No caso esse controlador é o que vai aparecer para o doc
    //swagger v1
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //Para suportar o versionamento por query string tiramos o {version} pois por padrão ele ja entende que a versao sera
    //informada na query string
    //[Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        [SwaggerOperation(
                Summary = "Lista todos os livros cadastrados",
                Description = "Requer que o usuário esteja autorizado",
                OperationId = "ListaLivros",
                Tags = new[] { "Livros" }
         )]
        [SwaggerResponse(200, "Livros retornados com sucesso", typeof(List<LivroApi>))]
        [SwaggerResponse(401, "Usuário não autorizado")]
        [SwaggerResponse(404, "Nenhum livro foi encontrado")]
        [SwaggerResponse(400, "Falha na requisição. Verifiquei o que está enviando no request")]
        [SwaggerResponse(500, "Erro de Servidor. Tente novamente mais tarde")]
        public IActionResult ListaDeLivros()
        {
            var lista = _repo.All.Select(l => l.ToApi()).ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
                Summary = "Recupera um livro pelo seu ID",
                Description = "Requer que o usuário esteja autorizado. Para recuperar um Id de um " +
            "livro pegue o mesmo na lista de livros do método ListaLivros.",
                OperationId = "RecuperaPorId",
                Tags = new[] { "Livros" }
         )]
        //Podemos dizer o que vai ser retornado no swagger:
        [SwaggerResponse(200, "Livro retornados com sucesso", typeof(List<LivroApi>))]
        [SwaggerResponse(401, "Usuário não autorizado")]
        [SwaggerResponse(404, "Livro não encontrado")]
        [SwaggerResponse(400, "Falha na requisição. Verifiquei se está informando parâmetros válidos")]
        [SwaggerResponse(500, "Erro de Servidor. Tente novamente mais tarde")]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 404)]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToApi());
        }

        [HttpGet("{id}/capa")]
        [SwaggerOperation(
                Summary = "Recupera a capa de um livro de acordo com o ID do mesmo.",
                Description = "Requer que o usuário esteja autorizado. Para recuperar um Id de um " +
            "livro pegue o mesmo na lista de livros do método ListaLivros.",
                OperationId = "RecuperaImagemCapaLivro",
                Tags = new[] { "Livros" }
         )]
        [SwaggerResponse(200, "Imagem de capa retornados com sucesso", typeof(List<LivroApi>))]
        [SwaggerResponse(401, "Usuário não autorizado")]
        [SwaggerResponse(404, "Imagem de capa não encontrada")]
        [SwaggerResponse(400, "Falha na requisição. Verifiquei se está informando parâmetros válidos")]
        [SwaggerResponse(500, "Erro de Servidor. Tente novamente mais tarde")]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpPost]
        [SwaggerOperation(
                Summary = "Cadastra um livro.",
                Description = "Requer que o usuário esteja autorizado. Para incluir um livro informe" +
            "os parâmetros desejados.",
                OperationId = "IncluiLivro",
                Tags = new[] { "Livros" }
         )]
        [SwaggerResponse(200, "Livro cadastrado com sucesso", typeof(List<LivroApi>))]
        [SwaggerResponse(401, "Usuário não autorizado")]
        [SwaggerResponse(400, "Falha na requisição. Verifiquei se está informando parâmetros válidos")]
        [SwaggerResponse(500, "Erro de Servidor. Tente novamente mais tarde")]
        public IActionResult Incluir([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro); //201
            }
            return BadRequest();
        }

        [HttpPut]
        [SwaggerOperation(
                Summary = "Faz a edição de um livro.",
                Description = "Requer que o usuário esteja autorizado. Para editar um livro informe" +
            "todas as informações do dado que quer alterar juntamente com as alterações desejadas.",
                OperationId = "AlteraLivro",
                Tags = new[] { "Livros" }
         )]
        [SwaggerResponse(200, "Livro alterado com sucesso", typeof(List<LivroApi>))]
        [SwaggerResponse(401, "Usuário não autorizado")]
        [SwaggerResponse(404, "Livro não encontrado")]
        [SwaggerResponse(400, "Falha na requisição. Verifiquei se está informando parâmetros válidos")]
        [SwaggerResponse(500, "Erro de Servidor. Tente novamente mais tarde")]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok(); //200
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
                Summary = "Faz a remoção de um livro.",
                Description = "Requer que o usuário esteja autorizado. Para remover um livro informe" +
            "seu ID.",
                OperationId = "RemoveLivro",
                Tags = new[] { "Livros" }
         )]
        [SwaggerResponse(200, "Livro removido com sucesso", typeof(List<LivroApi>))]
        [SwaggerResponse(401, "Usuário não autorizado")]
        [SwaggerResponse(404, "Livro não encontrado não encontrada")]
        [SwaggerResponse(400, "Falha na requisição. Verifiquei se está informando parâmetros válidos")]
        [SwaggerResponse(500, "Erro de Servidor. Tente novamente mais tarde")]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent(); //203
        }
    }
}
