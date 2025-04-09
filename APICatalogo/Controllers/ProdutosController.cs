using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ILogger<ApiLoggingFilter> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ProdutosController(ILogger<ApiLoggingFilter> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPorCategoria(int id)
        {
            var produtos = _unitOfWork.ProdutosRepository.GetProdutosPorCategoria(id);
            if (produtos is null)
            {
                return NotFound($"Produto com categoria id={id} não encontrado...");
            }
            var produtosDTO = produtos.ToList().toProdutoDTOList();
            return Ok(produtosDTO);
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            var produtos = _unitOfWork.ProdutosRepository.GetAll().ToList();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!");
            }
            var produtosDTO = produtos.ToList().toProdutoDTOList();
            return Ok(produtosDTO);
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> GetByID(int id)
        {
            var produto = _unitOfWork.ProdutosRepository.GetById(c => c.ProdutoId == id);
            if (produto is null)
            {
                _logger.LogWarning($"Produto com id={id} não encontrado...");
                return NotFound("Produto não encontrado!");
            }
            var produtosDTO = produto.toProdutoDTO();
            return Ok(produtosDTO);
        }

        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDTO)
        {
            if (produtoDTO is null)
            {
                return BadRequest("Produto inválido!");
            }
            var produto = produtoDTO.toProduto();
            var produtoCriado = _unitOfWork.ProdutosRepository.Create(produto);

            var novoProdutoDTO = produtoCriado.toProdutoDTO();
            _unitOfWork.Commit();

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDTO.ProdutoId }, novoProdutoDTO);
        }

        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.ProdutoId)
            {
                return BadRequest("Produto não cadastrado!");
            }
            var produto = produtoDTO.toProduto();
            var produtoAtualizado = _unitOfWork.ProdutosRepository.Update(produto);
            _unitOfWork.Commit();
            var produtoAtualizadoDTO = produtoAtualizado.toProdutoDTO();
            return Ok(produtoAtualizadoDTO);
        }

        [HttpDelete]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _unitOfWork.ProdutosRepository.GetById(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado!");
            }
            _unitOfWork.ProdutosRepository.Delete(produto);
            _unitOfWork.Commit();
            var produtoDeletadoDTO = produto.toProdutoDTO();
            return Ok(produtoDeletadoDTO);
        }
    }
}
