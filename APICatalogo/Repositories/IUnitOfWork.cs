namespace APICatalogo.Repositories;

public interface IUnitOfWork
{
    IProdutosRepository ProdutosRepository { get; }
    ICategoriasRepository CategoriasRepository { get; }
    void Commit();
    void Dispose();
}
