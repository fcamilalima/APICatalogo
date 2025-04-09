using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoriasRepository : Repository<Categoria>, ICategoriasRepository
    {
        public CategoriasRepository(AppDbContext context) : base(context)
        {
        }
    }
}
