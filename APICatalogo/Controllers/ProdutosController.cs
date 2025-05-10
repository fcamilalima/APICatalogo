using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ProdutosController : ControllerBase
    {
        //private readonly ILogger<ApiLoggingFilter> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ProdutosController(/* ILogger<ApiLoggingFilter> logger, */
            IUnitOfWork unitOfWork)
        {
            //_logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("produtos/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int id)
        {
            var produtos = await _unitOfWork.ProdutosRepository.GetProdutosPorCategoriaAsync(id);
            if (produtos is null)
            {
                return NotFound($"Produto com categoria id={id} não encontrado...");
            }
            var produtosDTO = produtos.ToList().toProdutoDTOList();
            return Ok(produtosDTO);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutos(
            [FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _unitOfWork.ProdutosRepository.GetProdutosAsync(produtosParameters);

            var metadata = new
            {
                produtos.Count,
                produtos.PageSize,
                produtos.PageCount,
                produtos.HasNextPage,
                produtos.HasPreviousPage,
                produtos.TotalItemCount
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDTO = produtos.toProdutoDTOList();
            return Ok(produtosDTO);

        }

        [HttpGet("filter/preco/pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> ObterProdutos(
            [FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
        {
            var produtos = await _unitOfWork.ProdutosRepository.GetProdutoFiltroPrecoAsync(produtosFiltroPreco);
            var metadata = new
            {
                produtos.Count,
                produtos.PageSize,
                produtos.PageCount,
                produtos.HasNextPage,
                produtos.HasPreviousPage,
                produtos.TotalItemCount
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
            var produtoDTO = produtos.toProdutoDTOList();
            return Ok(produtoDTO);
        }

        /// <summary>
        /// Exibe uma lista com todos os produtos
        /// </summary>
        /// <returns>Retorna uma lista de objetos de Produtos</returns>
        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
        {
            try
            {
                var produtos = await _unitOfWork.ProdutosRepository.GetAllAsync();
                // Está linha abaixo serve para efetuar teste unitário - Bas Request
                //throw new Exception("Erro ao obter produtos");

                if (produtos is null)
                {
                    return NotFound("Nenhum produto encontrado!");
                }
                var produtosDTO = produtos.ToList().toProdutoDTOList();
                return Ok(produtosDTO);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Obtém o produto pelo seu identificador ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> GetByID(int? id)
        {
            if (id is null || id <= 0)
            {
                //_logger.LogWarning($"Produto com id={id} não encontrado...");
                return BadRequest("ID de Produto Inválido!");
            }
            var produto = await _unitOfWork.ProdutosRepository.GetByIdAsync(c => c.ProdutoId == id);
            if (produto is null)
                return NotFound($"Produto com id={id} não encontrado...");

            var produtosDTO = produto.toProdutoDTO();
            return Ok(produtosDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDTO)
        {
            if (produtoDTO is null)
            {
                return BadRequest("Produto inválido!");
            }
            var produto = produtoDTO.toProduto();
            var produtoCriado = _unitOfWork.ProdutosRepository.Create(produto);

            var novoProdutoDTO = produtoCriado.toProdutoDTO();
            await _unitOfWork.CommitAsync();

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDTO.ProdutoId }, novoProdutoDTO);
        }

        [HttpPatch("{id}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(
            int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if (patchProdutoDTO is null || id <= 0)
            {
                return BadRequest("Produto inválido!");
            }

            var produto = await _unitOfWork.ProdutosRepository.GetByIdAsync(c => c.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado!");
            }

            var produtoUpdateRequest = produto.toProdutoDTOUpdateRequest();

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            {
                return BadRequest(ModelState);
            }

            var produtoAtualizado = produtoUpdateRequest.toProduto(produto);

            _unitOfWork.ProdutosRepository.UpdatePatch(produtoAtualizado);
            await _unitOfWork.CommitAsync();

            return Ok(produtoAtualizado.toProdutoDTOUpdateResponse());
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.ProdutoId)
            {
                return BadRequest("Produto não cadastrado!");
            }
            var produto = produtoDTO.toProduto();
            var produtoAtualizado = _unitOfWork.ProdutosRepository.Update(produto);
            await _unitOfWork.CommitAsync();
            var produtoAtualizadoDTO = produtoAtualizado.toProdutoDTO();
            return Ok(produtoAtualizadoDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _unitOfWork.ProdutosRepository.GetByIdAsync(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado!");
            }
            _unitOfWork.ProdutosRepository.Delete(produto);
            await _unitOfWork.CommitAsync();
            var produtoDeletadoDTO = produto.toProdutoDTO();
            return Ok(produtoDeletadoDTO);
        }
    }
}
