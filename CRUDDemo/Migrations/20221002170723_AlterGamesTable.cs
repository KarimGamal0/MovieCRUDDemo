using Microsoft.EntityFrameworkCore.Migrations;

namespace CRUDDemo.Migrations
{
    public partial class AlterGamesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_games_Genres_GenreId",
                table: "games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_games",
                table: "games");

            migrationBuilder.RenameTable(
                name: "games",
                newName: "Games");

            migrationBuilder.RenameIndex(
                name: "IX_games_GenreId",
                table: "Games",
                newName: "IX_Games_GenreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Games",
                table: "Games",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Genres_GenreId",
                table: "Games",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Genres_GenreId",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Games",
                table: "Games");

            migrationBuilder.RenameTable(
                name: "Games",
                newName: "games");

            migrationBuilder.RenameIndex(
                name: "IX_Games_GenreId",
                table: "games",
                newName: "IX_games_GenreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_games",
                table: "games",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_games_Genres_GenreId",
                table: "games",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
