using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : Controller
{
    private readonly ILogger<CategoriasController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CategoriasController(ILogger<CategoriasController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public ActionResult<IEnumerable<CategoriaDTO>> Get()
    {
        var categorias = _unitOfWork.CategoriasRepository.GetAll().ToList();
        if(categorias is null)
        {
            _logger.LogWarning("Categorias não encontradas...");
            return NotFound("Categorias não encontradas!");
        }
        var categoriasDTO = categorias.ToList().ToCategoriaDTOList();
        return Ok(categoriasDTO);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public  ActionResult<CategoriaDTO> Get(int id)
    {
        var categoria =  _unitOfWork.CategoriasRepository.GetById(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return NotFound($"Categoria com id={id} não encontrada!");
        }
        var categoriasDTO = categoria.ToCategoriaDTO();
        return Ok(categoriasDTO);
    }

    [HttpPost]
    public ActionResult<CategoriaDTO> Post([FromBody] CategoriaDTO categoriaDTO)
    {
        if (categoriaDTO is null)
        {
            _logger.LogWarning("Categoria nula...");
            return BadRequest("Categoria inválida!");
        }

        var categoria = categoriaDTO.ToCategoria();
        var categoriaCriada = _unitOfWork.CategoriasRepository.Create(categoria);
        _unitOfWork.Commit();

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
    }

    [HttpPut("{id:int}")]
    public ActionResult<CategoriaDTO> Put(int id, [FromBody] CategoriaDTO categoriaDTO)
    {
        if (id != categoriaDTO.CategoriaId)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return BadRequest("Categoria não cadastrada!");
        }
        var categoria = categoriaDTO.ToCategoria();
        _unitOfWork.CategoriasRepository.Update(categoria);
        _unitOfWork.Commit();
        return Ok(categoria);
    }


    [HttpDelete("{id:int}")]
    public ActionResult<CategoriaDTO> Delete(int id)
    {
        var categoria = _unitOfWork.CategoriasRepository.GetById(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return NotFound("Categoria não encontrada!");
        }
        var categoriaExcluida = _unitOfWork.CategoriasRepository.Delete(categoria);
        _unitOfWork.Commit();
        var categoriaExcluidaDTO = categoriaExcluida.ToCategoriaDTO();
        return Ok(categoriaExcluidaDTO);
    }
}
