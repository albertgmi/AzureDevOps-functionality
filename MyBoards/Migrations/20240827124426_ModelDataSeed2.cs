using Microsoft.EntityFrameworkCore.Migrations;
using MyBoards.Entities;

#nullable disable

namespace MyBoards.Migrations
{
    public partial class ModelDataSeed2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorkItemsState",
                column: "Value",
                value: "On hold");

            migrationBuilder.InsertData(
                table: "WorkItemsState",
                column: "Value",
                value: "Rejected");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorkItemsState",
                keyColumn: "Value",
                keyValue: "On hold");

            migrationBuilder.DeleteData(
                table: "WorkItemsState",
                keyColumn: "Value",
                keyValue: "Rejected");
        }
    }
}
