using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;

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
    [Authorize]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {
        var categorias = await _unitOfWork.CategoriasRepository.GetAllAsync();
        if (categorias is null)
        {
            _logger.LogWarning("Categorias não encontradas...");
            return NotFound("Categorias não encontradas!");
        }
        var categoriasDTO = categorias.ToList().ToCategoriaDTOList();
        return Ok(categoriasDTO);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasAsync(
        [FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _unitOfWork.CategoriasRepository.GetCategoriasAsync(categoriasParameters);
        return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradasPorNome(
        [FromQuery] CategoriasFiltroNome categoriasFiltroNome)
    {
        var categorias = await _unitOfWork.CategoriasRepository.GetCategoriasFiltroNomeAsync(categoriasFiltroNome);

        return ObterCategorias(categorias);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> GetAsynch(int id)
    {
        var categoria = await _unitOfWork.CategoriasRepository.GetByIdAsync(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return NotFound($"Categoria com id={id} não encontrada!");
        }
        var categoriasDTO = categoria.ToCategoriaDTO();
        return Ok(categoriasDTO);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> Post([FromBody] CategoriaDTO categoriaDTO)
    {
        if (categoriaDTO is null)
        {
            _logger.LogWarning("Categoria nula...");
            return BadRequest("Categoria inválida!");
        }

        var categoria = categoriaDTO.ToCategoria();
        var categoriaCriada = _unitOfWork.CategoriasRepository.Create(categoria);
        await _unitOfWork.CommitAsync();

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, [FromBody] CategoriaDTO categoriaDTO)
    {
        if (id != categoriaDTO.CategoriaId)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return BadRequest("Categoria não cadastrada!");
        }
        var categoria = categoriaDTO.ToCategoria();
        _unitOfWork.CategoriasRepository.Update(categoria);
        await _unitOfWork.CommitAsync();
        return Ok(categoria);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _unitOfWork.CategoriasRepository.GetByIdAsync(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id={id} não encontrada...");
            return NotFound("Categoria não encontrada!");
        }
        var categoriaExcluida = _unitOfWork.CategoriasRepository.Delete(categoria);
        await _unitOfWork.CommitAsync();
        var categoriaExcluidaDTO = categoriaExcluida.ToCategoriaDTO();
        return Ok(categoriaExcluidaDTO);
    }


    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage,
            categorias.TotalItemCount
        };
        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var categoriasDTO = categorias.ToCategoriaDTOList();
        return Ok(categoriasDTO);
    }
}
