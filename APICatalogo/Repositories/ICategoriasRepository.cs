using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using X.PagedList;

namespace APICatalogo.Repositories;

public interface ICategoriasRepository:IRepository<Categoria>
{
    Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters);
    Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasFiltroNome);
}
