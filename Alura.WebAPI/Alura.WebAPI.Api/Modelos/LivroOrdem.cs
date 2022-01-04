using Alura.ListaLeitura.Modelos;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Alura.WebAPI.Api.Modelos
{
    public static class LivroOrdemExtensions
    {
        public static IQueryable<Livro> AplicarOrdem(this IQueryable<Livro> query, LivroOrdem ordem)
        {
            if(ordem != null && !string.IsNullOrEmpty(ordem.OrdernarPor))
            {
                //Para ordenar por um tipo que tem o nome do campo também, tenho que instalar o pacote:
                //System.Linq.Dynamic.Core
                query = query.OrderBy(ordem.OrdernarPor);
            }
            return query;
        } 
    }

    public class LivroOrdem
    {
        public string OrdernarPor { get; set; }
    }
}
