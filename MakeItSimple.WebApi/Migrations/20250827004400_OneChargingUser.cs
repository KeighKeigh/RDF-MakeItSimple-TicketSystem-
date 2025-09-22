using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class OneChargingUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_one_chargings_one_charging_mis_id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_one_chargings_one_charging_code",
                table: "users",
                column: "one_charging_code",
                principalTable: "one_chargings",
                principalColumn: "code",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_one_chargings_one_charging_code",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_one_chargings_one_charging_mis_id",
                table: "users",
                column: "one_charging_code",
                principalTable: "one_chargings",
                principalColumn: "code");
        }
    }
}
