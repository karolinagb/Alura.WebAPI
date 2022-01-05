using Alura.WebAPI.Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Alura.WebAPI.Api.Filtros
{
    public class ErrorResponseFilter : IExceptionFilter
    {
        //Esse metodo serve pra quando acontecer uma exceção, esse codigo vai atuar
        public void OnException(ExceptionContext context)
        {
            var errorResponse = ErrorResponse.FromException(context.Exception);
            //context = me da o argumento da requisição naquele contexto
            //O metodo StatusCode (da interface IActionResult) igual do controlador, nao estao disponivel aqui
            //A classe ObjectResult implementa a interface IActionResult
            //Passamoa a exceção na ObjjectResult e passando o status Code
            context.Result = new ObjectResult(errorResponse) { StatusCode = 500};
        }
    }
}
