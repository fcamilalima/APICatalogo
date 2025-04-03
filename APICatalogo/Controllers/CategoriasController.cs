using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : Controller
{
    private readonly ILogger<CategoriasController> _logger;
    private readonly IRepository<Categoria> _repository;

    public CategoriasController(ICategoriasRepository repository, ILogger<CategoriasController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        var categorias = _repository.GetAll();
        return Ok(categorias);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public  ActionResult<Categoria> Get(int id)
    {
        var categoria =  _repository.GetById(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return NotFound($"Categoria com id={id} não encontrada!");
        }
        return Ok(categoria);
    }

    [HttpPost]
    public ActionResult Post([FromBody] Categoria categoria)
    {
        if (categoria is null)
        {
            _logger.LogWarning("Categoria nula...");
            return BadRequest("Categoria inválida!");
        }
        var categoriaCriada = _repository.Create(categoria);

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, [FromBody] Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return BadRequest("Categoria não cadastrada!");
        }

        _repository.Update(categoria);
        return Ok(categoria);
    }


    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var categoria = _repository.GetById(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return NotFound("Categoria não encontrada!");
        }
        var categoriaExcluida = _repository.Delete(categoria);
        return Ok(categoriaExcluida);
    }
}
