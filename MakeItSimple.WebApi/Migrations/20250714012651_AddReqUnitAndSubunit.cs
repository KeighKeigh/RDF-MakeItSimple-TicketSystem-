using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddReqUnitAndSubunit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "req_sub_unit_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "req_unit_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_req_sub_unit_id",
                table: "request_concerns",
                column: "req_sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_req_unit_id",
                table: "request_concerns",
                column: "req_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_sub_units_req_sub_unit_id",
                table: "request_concerns",
                column: "req_sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_units_req_unit_id",
                table: "request_concerns",
                column: "req_unit_id",
                principalTable: "units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_sub_units_req_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_units_req_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_req_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_req_unit_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "req_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "req_unit_id",
                table: "request_concerns");
        }
    }
}
