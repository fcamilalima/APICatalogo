using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;


namespace APICatalogo.Repositories
{
    public class CategoriasRepository : Repository<Categoria>, ICategoriasRepository
    {
        public CategoriasRepository(AppDbContext context) : base(context) { }

        public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters)
        {
            var categorias = await GetAllAsync();
            var categoriasOrdenadasAssincrona = categorias.OrderBy(c => c.Nome)
                .AsQueryable();
            //var categoriasOrdenadas = agedList<Categoria>.ToPagedList(categoriasOrdenadasAssincrona,
            var categoriasOrdenadas = await categorias.ToPagedListAsync(categoriasParameters.PageNumber,
                categoriasParameters.PageSize);
            return categoriasOrdenadas;

        }

        public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasFiltroNome)
        {
            var categorias = await GetAllAsync();

            if (!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
            {
                categorias = categorias.Where(c => c.Nome.Contains(categoriasFiltroNome.Nome));
            }
            var categoriasFiltradas = await categorias.ToList<Categoria>().ToPagedListAsync(categoriasFiltroNome.PageNumber,
                categoriasFiltroNome.PageSize);
            return categoriasFiltradas;
        }
    }
}
