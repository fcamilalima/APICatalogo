using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repositories;

public class ProdutosRepository : Repository<Produto>, IProdutosRepository
{
    public ProdutosRepository(AppDbContext context) : base(context)
    {
    }
    public IEnumerable<Produto> GetProdutosPorCategoria(int id)
    {
        return GetAll().Where(p => p.CategoriaId == id);
    }
}
