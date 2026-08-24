
using System.Linq.Expressions;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infra.Data.Repositories;

public class BaseRepository<T> : IBaseRepository where T : class
{
    public IQueryable<T> ApplySort<T>(IQueryable<T> query, string sort, string order)
    {
        var parameter = Expression.Parameter(typeof(T), "x"); //cria um parametro chamado "x" do tipo T
        var property = Expression.Property(parameter, sort); //acessa a propriedade dentro do sort desse parametro(x.sort)
        var lambda = Expression.Lambda(property, parameter); // junta o parametro + propriedade com um lambda entre eles (x => x.sort)

        var methods = typeof(Queryable).GetMethods(); //busca todos os metodos de queryable

        string orderType = order.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy"; //verifica o tipo do order para utilizar na geração do metodo

        var orderByMethod = methods.First(m => m.Name == orderType && 
                            m.IsGenericMethodDefinition &&  m.GetGenericArguments().Length == 2 && 
                            m.GetParameters().Length == 2); //procura o metodo OrderBy dentro de queryable, que aceite 2 argumentos genericos e tenha 2 parametros

        var genericOrderByMethod = orderByMethod.MakeGenericMethod(typeof(T), property.Type); //Pegue o método OrderBy<TSource, TKey> e substitua TSource pelo tipo do objeto e TKey pelo tipo da propriedade

        var result = genericOrderByMethod.Invoke(null,new object[] { query, lambda } ); //executa o metodo criado ( query.OrderBy() ), utilizando o lambda criado acima ( query.OrderBy(x => x.sort) )

        return (IQueryable<T>)result;
    }
}
