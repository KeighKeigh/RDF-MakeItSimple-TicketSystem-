using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createOrderForPms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "pms_questionaires",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "pms_questionaire_modules",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "pms_questionaires");

            migrationBuilder.DropColumn(
                name: "order",
                table: "pms_questionaire_modules");
        }
    }
}
