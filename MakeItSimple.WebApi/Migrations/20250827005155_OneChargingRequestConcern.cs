using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class OneChargingRequestConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "one_charging_code",
                table: "request_concerns",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_one_charging_code",
                table: "request_concerns",
                column: "one_charging_code");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_one_chargings_one_charging_code",
                table: "request_concerns",
                column: "one_charging_code",
                principalTable: "one_chargings",
                principalColumn: "code",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_one_chargings_one_charging_code",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_one_charging_code",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "one_charging_code",
                table: "request_concerns");
        }
    }
}
