using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogoxUnitTestes.UnitTests;

public class PutProdutoUnitTestes : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public PutProdutoUnitTestes(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository);
    }

    [Fact]
    public async Task PutProduto_OkResult()
    {
        //Arrange
        var prodId = 1;
        var produto = new ProdutoDTO
        {
            ProdutoId = prodId,
            Nome = "Produto Teste",
            Descricao = "Descricao Teste",
            ImageURL = "produto_teste.png",
            CategoriaId = 1
        };

        //Act
        var data = await _controller.Put(prodId, produto) as ActionResult<ProdutoDTO>;

        //Assert (FluentAssertions)
        data.Should().NotBeNull();
        data.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PutProduto_BadRequestResult()
    {
        //Arrange
        var prodId = 1111;

        var produto = new ProdutoDTO
        {
            ProdutoId = 14,
            Nome = "Produto Atualizado - Testes",
            Descricao = "Minha Descrição Alterada",
            ImageURL = "produto_teste_atualizada.png",
            CategoriaId = 1
        };

        //Act
        var data = await _controller.Put(prodId, produto) as ActionResult<ProdutoDTO>;

        //Assert (FluentAssertions)
        data.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode
            .Should().Be(400);
    }
}
