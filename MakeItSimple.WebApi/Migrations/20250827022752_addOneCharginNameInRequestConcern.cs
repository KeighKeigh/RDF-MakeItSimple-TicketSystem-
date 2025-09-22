using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addOneCharginNameInRequestConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_business_units_business_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_companies_company_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_departments_department_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_locations_location_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_sub_units_req_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_sub_units_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_units_req_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_units_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_business_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_company_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_department_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_location_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_req_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_req_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_unit_id",
                table: "request_concerns");

            migrationBuilder.AddColumn<string>(
                name: "one_charging_name",
                table: "request_concerns",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "one_charging_name",
                table: "request_concerns");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_business_unit_id",
                table: "request_concerns",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_company_id",
                table: "request_concerns",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_department_id",
                table: "request_concerns",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_location_id",
                table: "request_concerns",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_req_sub_unit_id",
                table: "request_concerns",
                column: "req_sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_req_unit_id",
                table: "request_concerns",
                column: "req_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_sub_unit_id",
                table: "request_concerns",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_unit_id",
                table: "request_concerns",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_business_units_business_unit_id",
                table: "request_concerns",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_companies_company_id",
                table: "request_concerns",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_departments_department_id",
                table: "request_concerns",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_locations_location_id",
                table: "request_concerns",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_sub_units_req_sub_unit_id",
                table: "request_concerns",
                column: "req_sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_sub_units_sub_unit_id",
                table: "request_concerns",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_units_req_unit_id",
                table: "request_concerns",
                column: "req_unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_units_unit_id",
                table: "request_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");
        }
    }
}
