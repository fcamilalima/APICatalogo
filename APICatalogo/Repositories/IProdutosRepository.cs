using APICatalogo.Models;
using APICatalogo.Repositories;

namespace APICatalogo.Repositories;

public interface IProdutosRepository :IRepository<Produto>
{
    IEnumerable<Produto> GetProdutosPorCategoria(int id);
}
