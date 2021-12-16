using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Alura.WebAPI.DAL.Livros.Migrations
{
    public partial class CriacaoBanco : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Livros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Subtitulo = table.Column<string>(type: "nvarchar(75)", nullable: true),
                    Resumo = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    ImagemCapa = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Autor = table.Column<string>(type: "nvarchar(75)", nullable: true),
                    Lista = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livros", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Livros");
        }
    }
}
