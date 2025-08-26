using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOneChargingCodeAndHasApproverInUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "has_approver",
                table: "users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "one_charging_code",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "has_approver", "one_charging_code" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_approver",
                table: "users");

            migrationBuilder.DropColumn(
                name: "one_charging_code",
                table: "users");
        }
    }
}
