using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace APICatalogoxUnitTestes.UnitTests;

public class ProdutosUnitTestController
{
    public IUnitOfWork repository;
    //public IMapper mapper;
    public static DbContextOptions<AppDbContext> dbContextOptions { get;  }

    public static string connectionString =
        "Server=localhost;Port=3305;Database=ApiCatalogoDB;Uid=root;Pwd=Catalogo#2025;";

    static ProdutosUnitTestController()
    {
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }
    public ProdutosUnitTestController()
    {
        var context = new AppDbContext(dbContextOptions);
        repository = new UnitOfWork(context);
    }
}
