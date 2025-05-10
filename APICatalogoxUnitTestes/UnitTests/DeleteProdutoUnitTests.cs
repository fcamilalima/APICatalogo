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

public class DeleteProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public DeleteProdutoUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository);
    }

    [Fact]
    public async Task DeleteProdutoByID_OkResult()
    {
        // Arrange
        var prodId = 14;

        //Act
        var data = await _controller.Delete(prodId) as ActionResult<ProdutoDTO>;

        //Assert
        data.Should().NotBeNull();
        data.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteProdutoByID_NotFound()
    {
        //Arrange
        var prodId = 1000;

        //Act
        var data = await _controller.Delete(prodId) as ActionResult<ProdutoDTO>;

        //Assert
        data.Should().NotBeNull();
        data.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
