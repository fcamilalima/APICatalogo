namespace APICatalogo.Repositories;

public interface IUnitOfWork
{
    IProdutosRepository ProdutosRepository { get; }
    ICategoriasRepository CategoriasRepository { get; }
    Task CommitAsync();
    void Dispose();
}
