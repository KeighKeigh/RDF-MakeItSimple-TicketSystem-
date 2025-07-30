using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferChannelIdInRequestConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "added_by",
                table: "ticket_transaction_notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "transfer_channel_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_transfer_channel_id",
                table: "request_concerns",
                column: "transfer_channel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_channels_transfer_channel_id",
                table: "request_concerns",
                column: "transfer_channel_id",
                principalTable: "channels",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_channels_transfer_channel_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_transfer_channel_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "transfer_channel_id",
                table: "request_concerns");

            migrationBuilder.AlterColumn<Guid>(
                name: "added_by",
                table: "ticket_transaction_notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
