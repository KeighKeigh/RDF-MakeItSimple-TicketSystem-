using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class relationshipOfOneChargingAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_users_companies_company_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_users_locations_location_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_business_unit_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_company_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_location_id",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "one_charging_code",
                table: "users",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "one_chargings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_one_chargings_code",
                table: "one_chargings",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "ix_users_one_charging_code",
                table: "users",
                column: "one_charging_code");

            migrationBuilder.AddForeignKey(
                name: "fk_users_one_chargings_one_charging_mis_id",
                table: "users",
                column: "one_charging_code",
                principalTable: "one_chargings",
                principalColumn: "code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_one_chargings_one_charging_mis_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_one_charging_code",
                table: "users");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_one_chargings_code",
                table: "one_chargings");

            migrationBuilder.AlterColumn<string>(
                name: "one_charging_code",
                table: "users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "one_chargings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "ix_users_business_unit_id",
                table: "users",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_company_id",
                table: "users",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_location_id",
                table: "users",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_companies_company_id",
                table: "users",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_locations_location_id",
                table: "users",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");
        }
    }
}
