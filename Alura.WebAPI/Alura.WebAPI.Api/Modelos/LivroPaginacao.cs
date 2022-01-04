using Alura.ListaLeitura.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alura.WebAPI.Api.Modelos
{
    public static class LivroPaginadoExtensions
    {
        public static LivroPaginado ToLivroPaginado(this IQueryable<LivroApi> query, LivroPaginacao paginacao)
        {
            int totalItens = query.Count();
            //A divisao pode dar numero fracionado, entao eu tenho que pegar o valor inteiro mais proximo maior que esse
            //valor fracionado. Exemplo 9 itens/ 2 paginas = tem que dar 3 (3 é o numero inteiro mais proximo maior que o
            //numero quebrado)
            //A função Ceiling resolve isso para a gente, so que ele retorna decimal, entao a gente converte pra int
            //O double antes do valor do tamanho foi colocado porque o metodo Ceiling possui duas chamadas, uma pra decimal
            //e outra pra double, ai pra informar que quero double eu coloco a conversao de um dos valores pra esse tipo
            int totalPaginas = (int)Math.Ceiling(totalItens / (double)paginacao.Tamanho);
            return new LivroPaginado
            {
                //total de itens
                Total = totalItens,
                //Total de pagina = itens dividido por tamanho de cada pagina
                TotalPaginas = totalPaginas,
                NumeroPagina = paginacao.Pagina,
                TamanhoPagina = paginacao.Tamanho,
                Resultado = query
                    //Antes de pegar os itens tenho que descartar uma quantidade anterior, se houver, com o skip
                    .Skip(paginacao.Tamanho * (paginacao.Pagina-1))
                    //Take pega os itens de uma quantidade de itens
                    .Take(paginacao.Tamanho).ToList(),
                Anterior = (paginacao.Pagina > 1 ) ? 
                $"livros?tamanho={paginacao.Tamanho}&pagina={paginacao.Pagina-1}" : "",
                Proximo = (paginacao.Pagina < totalPaginas) ?
                 $"livros?tamanho={paginacao.Tamanho}&pagina={paginacao.Pagina+1}" : ""
            };
        }
    }
    public class LivroPaginado
    {
        public int Total { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPagina { get; set; }
        public int NumeroPagina { get; set; }
        public IList<LivroApi> Resultado { get; set; }
        public string Anterior { get; set; }
        public string Proximo { get; set; }
    }

    public class LivroPaginacao
    {
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 10;
    }
}
