using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace Alura.WebAPI.Api.Modelos
{
    public class ErrorResponse
    {
        public int Codigo { get; set; }
        public string Mensagem { get; set; }
        public ErrorResponse InnerError { get; set; }
        public string[] Detalhes { get; set; }

        public static ErrorResponse FromException(Exception e)
        {
            if(e == null)
            {
                return null;
            }

            return new ErrorResponse
            {
                Codigo = e.HResult,
                Mensagem = e.Message,
                InnerError = ErrorResponse.FromException(e.InnerException)
            };
        }

        public static ErrorResponse FromModelState(ModelStateDictionary modelState)
        {
            //Com Select ele retorna uma IEnumerable de uma coleção de erros. Eu quero apenas a coleção de erros.
            //Quando eu tenho uma lista de outra lista eu uso o SelectMany. O SelectMany gera uma lista apenas das listas
            //que estão no segundo nivel que no nosso caso é a coleção de erros em si.
            var erros = modelState.Values.SelectMany(e => e.Errors);
            return new ErrorResponse
            {
                Codigo = 100,
                Mensagem = "Houve erro(s) no envio da requisição.",
                //Dentro dessa lista retornada eu vou selecionar as propriedades string(errorMessage)
                //Como nossa propriedade é um Array eu converto pra array
                Detalhes = erros.Select(e => e.ErrorMessage).ToArray()
            };
        }
    }
}
