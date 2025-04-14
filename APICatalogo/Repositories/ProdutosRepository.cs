using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalogo.Repositories;

public class ProdutosRepository : Repository<Produto>, IProdutosRepository
{
    public ProdutosRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Produto>> GetProdutoFiltroPrecoAsync(ProdutosFiltroPreco produtoFiltroParms)
    {
        var produtos = await GetAllAsync();
        if (produtoFiltroParms.Preco.HasValue && !string.IsNullOrEmpty(produtoFiltroParms.PrecoCriterio))
        {
            if (produtoFiltroParms.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco > produtoFiltroParms.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtoFiltroParms.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco < produtoFiltroParms.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtoFiltroParms.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco == produtoFiltroParms.Preco.Value).OrderBy(p => p.Preco);
            }

        }
        var produtosFiltrados = await produtos.ToPagedListAsync(produtoFiltroParms.PageNumber, produtoFiltroParms.PageSize);
        return produtosFiltrados;
    }

    public async Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams)
    {
        var produtos = await GetAllAsync();
        var produtosAssincronos = produtos.OrderBy(p => p.ProdutoId).AsQueryable();
        var produtoOrdenados = await produtosAssincronos.ToPagedListAsync(
            produtosParams.PageNumber, produtosParams.PageSize);

        return produtoOrdenados;
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
    {
        var produtos = await GetAllAsync();
        var produtosCategoria = produtos.Where(p => p.CategoriaId == id);
        return produtosCategoria;
    }

    public Produto UpdatePatch(Produto produto)
    {
        var local = _context.Set<Produto>()
            .Local
            .FirstOrDefault(entry => entry.ProdutoId.Equals(produto.ProdutoId));

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        _context.Update(produto);
        return produto;
    }
}
