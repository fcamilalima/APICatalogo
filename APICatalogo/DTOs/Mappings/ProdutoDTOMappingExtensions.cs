using APICatalogo.Models;

namespace APICatalogo.DTOs.Mappings;

public static class ProdutoDTOMappingExtensions
{
    public static Produto? toProduto(this ProdutoDTO produtoDTO)
    {
        if (produtoDTO is null) return null;

        return new Produto()
        {
            ProdutoId = produtoDTO.ProdutoId,
            Nome = produtoDTO.Nome,
            Descricao = produtoDTO.Descricao,
            Preco = produtoDTO.Preco,
            ImagemURL = produtoDTO.ImagemURL,
            CategoriaId = produtoDTO.CategoriaId
        };
    }

    public static ProdutoDTO? toProdutoDTO(this Produto produto)
    {
        if (produto is null) return null;

        return new ProdutoDTO()
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            ImagemURL = produto.ImagemURL,
            CategoriaId = produto.CategoriaId
        };
    }

    public static IEnumerable<ProdutoDTO> toProdutoDTOList(this IEnumerable<Produto> produtos)
    {
        if (produtos is null || !produtos.Any())
        {
            return new List<ProdutoDTO>();
        }

        return produtos.Select(
            produto => new ProdutoDTO()
            {
                ProdutoId = produto.ProdutoId,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                ImagemURL = produto.ImagemURL,
                CategoriaId = produto.CategoriaId
            })
            .ToList();
    }
}
