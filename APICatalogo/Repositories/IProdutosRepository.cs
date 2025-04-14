using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories;

public interface IProdutosRepository :IRepository<Produto>
{
    Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id);
    Produto UpdatePatch(Produto produto);
    Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams);
    Task<IPagedList<Produto>> GetProdutoFiltroPrecoAsync(ProdutosFiltroPreco produtoFiltroParms);
}
