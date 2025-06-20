using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDataToRequestConcernTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_service_provider_channels_service_providers_service_provider_id",
                table: "service_provider_channels");

            migrationBuilder.AlterColumn<int>(
                name: "service_provider_id",
                table: "service_provider_channels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "assign_to",
                table: "request_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "service_provider_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "target_date",
                table: "request_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_service_provider_id",
                table: "request_concerns",
                column: "service_provider_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_service_providers_service_provider_id",
                table: "request_concerns",
                column: "service_provider_id",
                principalTable: "service_providers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_service_provider_channels_service_providers_service_provider_id",
                table: "service_provider_channels",
                column: "service_provider_id",
                principalTable: "service_providers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_service_providers_service_provider_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_service_provider_channels_service_providers_service_provider_id",
                table: "service_provider_channels");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_service_provider_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "assign_to",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "service_provider_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "target_date",
                table: "request_concerns");

            migrationBuilder.AlterColumn<int>(
                name: "service_provider_id",
                table: "service_provider_channels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "fk_service_provider_channels_service_providers_service_provider_id",
                table: "service_provider_channels",
                column: "service_provider_id",
                principalTable: "service_providers",
                principalColumn: "id");
        }
    }
}
