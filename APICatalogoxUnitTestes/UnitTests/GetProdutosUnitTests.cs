using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace APICatalogoxUnitTestes.UnitTests;

public class GetProdutosUnitTests:IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public GetProdutosUnitTests(ProdutosUnitTestController controlller)
    {
        _controller = new ProdutosController(controlller.repository);
    }

    [Fact]
    public async Task GetProdutoById_OkResult()
    {
        //Arrange
        var proID = 2;

        //Act
        var data = await _controller.GetByID(proID);

        //Assert (FluentAssertions)
        data.Result.Should().BeOfType<OkObjectResult>().Which.StatusCode
            .Should().Be(200);
    }

    [Fact]
    public async Task GetProdutoById_NotFoundResult()
    {
        //Arrange
        var proID = 999;

        //Act
        var data = await _controller.GetByID(proID);

        //Assert (FluentAssertions)
        data.Result.Should().BeOfType<NotFoundObjectResult>().
            Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetProdutoById_BadRequestResult()
    {
        //Arrange
        var proID = -1;

        //Act
        var data = await _controller.GetByID(proID);

        //Assert (FluentAssertions)
        data.Result.Should().BeOfType<BadRequestObjectResult>().
            Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetProdutos_ListaDeProdutosDTO_OKResult()
    {
        //Act
        var data = await _controller.Get();

        //Assert (FluentAssertions)
        data.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTO>>()
            .And.NotBeNull();
    }


    [Fact]
    public async Task GetProdutos_ListaDeProdutosDTO_BadRequestResult()
    {
        //Act
        var data = await _controller.Get();

        //Assert (FluentAssertions)
        data.Result.Should().BeOfType<BadRequestResult>();
    }
}
