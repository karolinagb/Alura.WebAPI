using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;  //apelidando a classe pra n dar conflito de namespace

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    public class ListasLeituraController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public ListasLeituraController(IRepository<Livro> repo)
        {
            _repo = repo;
        }

        private Lista CriarLista(TipoListaLeitura tipo)
        {
            return new Lista
            {
                Tipo = tipo.ParaString(),
                Livros = _repo.All.Where(l => l.Lista == tipo).Select(l => l.ToApi()).ToList()
            };
        }

        [HttpGet]
        [SwaggerOperation(
                Summary = "Recupera toda as listas com seus livros cadastrados",
                Description = "Requer que o usuário esteja autorizado. Há livros cadastrados na lista" +
            "ParaLer, Lendo e Lidos.",
                OperationId = "ListaLivrosPorLista",
                Tags = new[] { "Listas" }
         )]
        public IActionResult TodasListas()
        {
            Lista paraLer = CriarLista(TipoListaLeitura.ParaLer);
            Lista lendo = CriarLista(TipoListaLeitura.Lendo);
            Lista lidos = CriarLista(TipoListaLeitura.Lidos);
            var colecao = new List<Lista> { paraLer, lendo, lidos };
            return Ok(colecao);
        }

        [HttpGet("{tipo}")]
        [SwaggerOperation(
                Summary = "Recupera uma determinada lista pelo seu tipo",
                Description = "Requer que o usuário esteja autorizado. Recupera uma lista informando" +
            "o tipo dela (ParaLer, Lendo ou Lidos) com seus respectivos livros cadastrados",
                OperationId = "RecuperaListaPorTipo",
                Tags = new[] { "Listas" }
         )]
        public IActionResult Recuperar(TipoListaLeitura tipo)
        {
            var lista = CriarLista(tipo);
            return Ok(lista);
        }
    }
}
