using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;
using X.PagedList;
using Microsoft.AspNetCore.Http;

namespace APICatalogo.Controllers;

[EnableCors("OrigensComAcessoPermitido")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]

//[ApiExplorerSettings(IgnoreApi = true)]
//[EnableRateLimiting("fixedwindow")]
public class CategoriasController : Controller
{
    private readonly ILogger<CategoriasController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CategoriasController(ILogger<CategoriasController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    
    /// <summary>
    /// Obtém uma lista de objetos Categoria
    /// </summary>
    /// <returns>Uma lista de todas as categorias</returns>
    [HttpGet]
    [Authorize]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
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

    /// <summary>
    /// Obtém uma categoria pelo seu identificador ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Lista de objetos Categoria</returns>
    [DisableCors]
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDTO>> GetById(int id)
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

    /// <summary>
    /// Inclusão de uma nova categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    ///     POST api/categorias
    ///     {
    ///         "categoriaId": 1,
    ///         "nome": "Categoria 1",
    ///         "imageUrl": "https://examplo.com/imagem.jpg"
    ///     }
    /// </remarks>
    /// <param name="categoriaDTO">Objeto Categoria</param>
    /// <returns>O objeto da Categoria inclusa</returns>
    /// <remarks>Retorna um objeto Categoria incluído</remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
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

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
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
