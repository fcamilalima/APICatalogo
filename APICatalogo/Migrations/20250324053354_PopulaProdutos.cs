using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImageURL, Estoque, DataCadastro, CategoriasCategoriaId) VALUES('Coca-Cola Diet', 'Refrigerante de cola', 5.45, 'cocacola.jpg', 50, now(), 1)");
            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImageURL, Estoque, DataCadastro, CategoriasCategoriaId) VALUES('Lanche de Atum', 'Lanche de Atum com maionese', 8.50, 'atum.jpg', 10, now(), 2)");
            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImageURL, Estoque, DataCadastro, CategoriasCategoriaId) VALUES('Pudim 100g', 'Pudim de leite condensado 100g', 6.75, 'pudim.jpg', 20, now(), 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Produtos");
        }
    }
}
