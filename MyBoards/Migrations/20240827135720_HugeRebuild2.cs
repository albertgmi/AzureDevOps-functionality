using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoards.Migrations
{
    public partial class HugeRebuild2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Users_UserId",
                table: "WorkItems");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "WorkItems",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkItems_UserId",
                table: "WorkItems",
                newName: "IX_WorkItems_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Users_AuthorId",
                table: "WorkItems",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Users_AuthorId",
                table: "WorkItems");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "WorkItems",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkItems_AuthorId",
                table: "WorkItems",
                newName: "IX_WorkItems_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Users_UserId",
                table: "WorkItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
