using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutosRepository _produtoRepository;
        //private readonly Repository<Produto> _repository;
        private readonly ILogger<ApiLoggingFilter> _logger;
        public ProdutosController(IProdutosRepository produtoRepository, ILogger<ApiLoggingFilter> logger)
        {
            //_repository = repository;
            _produtoRepository = produtoRepository;
            _logger = logger;
        }
        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosPorCategoria(int id)
        {
            var produtos = _produtoRepository.GetProdutosPorCategoria(id);
            if (produtos is null)
            {
                return NotFound();
            }
            return Ok(produtos);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _produtoRepository.GetAll().ToList();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!");
            }
            return Ok(produtos);
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Produto> GetByID(int id)
        {
            var produto = _produtoRepository.GetById(c => c.ProdutoId == id);
            if (produto is null)
            {
                _logger.LogWarning($"Produto com id={id} não encontrado...");
                return NotFound("Produto não encontrado!");
            }
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
            {
                return BadRequest("Produto inválido!");
            }
            var produtoCriado = _produtoRepository.Create(produto);
            return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriado.ProdutoId }, produtoCriado);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest("Produto não cadastrado!");
            }

            var produtoAtualizado = _produtoRepository.Update(produto);
            return Ok(produto);
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var produto = _produtoRepository.GetById(p => p.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado!");
            }
            _produtoRepository.Delete(produto);
            return Ok(produto);
        }
    }
}
