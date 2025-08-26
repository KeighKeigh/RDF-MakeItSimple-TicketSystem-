using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOneChargingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "one_chargings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sync_id = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_id = table.Column<int>(type: "int", nullable: true),
                    business_unit_id = table.Column<int>(type: "int", nullable: true),
                    business_unit_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_unit_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_unit_id = table.Column<int>(type: "int", nullable: true),
                    department_unit_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_unit_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    sub_unit_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sub_unit_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_id = table.Column<int>(type: "int", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    date_added = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_one_chargings", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "one_chargings");
        }
    }
}
