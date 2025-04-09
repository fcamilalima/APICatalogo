using APICatalogo.Context;

namespace APICatalogo.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProdutosRepository? _produtosRepository;

    private ICategoriasRepository? _categoriasRepository;

    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProdutosRepository ProdutosRepository
    {
        get
        {
            return _produtosRepository ??= new ProdutosRepository(_context);
        }
    }

    public ICategoriasRepository CategoriasRepository
    {
        get
        {
            return _categoriasRepository ??= new CategoriasRepository(_context);
        }
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
