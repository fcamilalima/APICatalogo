using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : Controller
{
    public readonly AppDbContext _context;

    public CategoriasController(AppDbContext contexto)
    {
        _context = contexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> Get()
    {
        try
        {
            return await _context.Categorias.AsNoTracking().ToListAsync();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro ao tratar a sua solicitação.");
        }
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<Categoria>> GetByID(int id)
    {
        try
        {
            var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);
            if (categoria is null)
            {
                return NotFound("Categoria não encontrada!");
            }
            return Ok(categoria);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro ao tratar a sua solicitação.");
        }
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        try
        {
            return _context.Categorias.Include(c => c.Produtos).AsNoTracking().ToList();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro ao tratar a sua solicitação.");
        }
    }
    [HttpPost]
    public ActionResult Post([FromBody]Categoria categoria)
    {
        if (categoria is null)
        {
            return BadRequest("Categoria inválida!");
        }
        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, [FromBody]Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            return BadRequest("Categoria não cadastrada!");
        }
        _context.Entry(categoria).State = EntityState.Modified;
        _context.SaveChanges();
        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);
        if (categoria is null)
        {
            return NotFound("Categoria não encontrada!");
        }
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        return Ok(categoria);
    }
}
