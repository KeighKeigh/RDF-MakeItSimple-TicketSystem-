using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketAttachmentToDateApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "business_unit_id",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "approver_date_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "business_unit_id",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_approver_date_id",
                table: "ticket_attachments",
                column: "approver_date_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_approver_dates_approver_date_id",
                table: "ticket_attachments",
                column: "approver_date_id",
                principalTable: "approver_dates",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_approver_dates_approver_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_approver_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "approver_date_id",
                table: "ticket_attachments");

            migrationBuilder.AlterColumn<int>(
                name: "business_unit_id",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "business_unit_id",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");
        }
    }
}
