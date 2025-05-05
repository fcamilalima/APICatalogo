using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogoxUnitTestes.UnitTests;

public class PostProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;
    public PostProdutoUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository);
    }

    [Fact]
    public async Task PostProduto_CreatedResult()
    {
        //Arrange
        var produto = new ProdutoDTO()
        {
            Nome = "Novo Produto",
            Descricao = "Descrição do Novo Produto",
            Preco = 10.99M,
            ImageURL = "novo_produto.jpg",
            CategoriaId = 2
        };

        //Act
        var data = await _controller.Post(produto);

        //Assert (FluentAssertions)
        var result = data.Result.Should().BeOfType<CreatedAtRouteResult>();
        result.Subject.StatusCode.Should().Be(201);
    }
}
