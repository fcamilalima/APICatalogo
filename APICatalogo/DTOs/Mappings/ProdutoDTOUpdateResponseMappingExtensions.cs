using APICatalogo.Models;

namespace APICatalogo.DTOs.Mappings;

public static class ProdutoDTOUpdateResponseMappingExtensions
{
    public static ProdutoDTOUpdateResponse toProdutoDTOUpdateResponse(
        this Produto produto)
    {
        if(produto is null)
        {
            return new ProdutoDTOUpdateResponse();
        }
        return new ProdutoDTOUpdateResponse() { 
            ProdutoId = produto.ProdutoId,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            ImagemURL = produto.ImagemURL,
            Estoque = produto.Estoque,
            DataCadastro = produto.DataCadastro,
            CategoriaId = produto.CategoriaId
        };
    }
}
