using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Alura.WebAPI.WebApp.Formatters
{
    public class LivroCsvFormatter : TextOutputFormatter //Formato de saída do tipo texto
    {
        //Especificar os tipos de mídia que o formatador dá suporte:
        public LivroCsvFormatter()
        {
            var textCsvMediaType = MediaTypeHeaderValue.Parse("text/csv");
            var appCsvMediaType = MediaTypeHeaderValue.Parse("app/csv");
            SupportedMediaTypes.Add(textCsvMediaType);
            SupportedMediaTypes.Add(appCsvMediaType);
            SupportedEncodings.Add(Encoding.UTF8); //Opcionalmente, um formatador de mídia pode dar suporte a várias codificações de
                                                   //caracteres, como UTF-8 ou ISO 8859-1.
        }

        //Informar o tipo que ele deve serializar:
        protected override bool CanWriteType(Type type)
        {
            return type == typeof(LivroApi);
        }


        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            //Escrever no corpo da resposta meu livro no formato csv
            var livroEmCsv = "";

            if(context.Object is LivroApi)
            {
                LivroApi livro = context.Object as LivroApi;

                livroEmCsv = $"{livro.Titulo};{livro.Subtitulo};{livro.Autor};{livro.Lista}";
            }

            //O Write Factory vai escrever em stream no body
            using (var escritor = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return escritor.WriteAsync(livroEmCsv);
            }//Essa construção com using ja garante que o escritor vai ser fechado depois assim: escritor.Close()




        }
    }
}
