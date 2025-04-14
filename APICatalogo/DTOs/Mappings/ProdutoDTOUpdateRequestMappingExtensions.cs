using APICatalogo.Models;

namespace APICatalogo.DTOs.Mappings;

public static class ProdutoDTOUpdateRequestMappingExtensions
{
    public static ProdutoDTOUpdateRequest? toProdutoDTOUpdateRequest(this Produto produto)
    {
        if (produto is null) return null;
        return new ProdutoDTOUpdateRequest()
        {
            Estoque = produto.Estoque,
            DataCadastro = produto.DataCadastro
        };
    }

    public static Produto? toProduto(this ProdutoDTOUpdateRequest produtoDTOUpdateRequest, Produto produto)
    {
        if (produtoDTOUpdateRequest is null) return new Produto();
        return new Produto()
        {
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            ImageURL = produto.ImageURL,
            CategoriaId = produto.CategoriaId,

            Estoque = produtoDTOUpdateRequest.Estoque,
            DataCadastro = produtoDTOUpdateRequest.DataCadastro

        };
    }
}
