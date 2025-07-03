using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addAssignToToTicketConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_categories_categories_category_id",
                table: "ticket_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_categories_request_concerns_request_concern_id",
                table: "ticket_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_sub_categories_request_concerns_request_concern_id",
                table: "ticket_sub_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_sub_categories_sub_categories_sub_category_id",
                table: "ticket_sub_categories");

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "ticket_sub_categories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "request_concern_id",
                table: "ticket_sub_categories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "assign_to",
                table: "ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "request_concern_id",
                table: "ticket_categories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "ticket_categories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_categories_categories_category_id",
                table: "ticket_categories",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_categories_request_concerns_request_concern_id",
                table: "ticket_categories",
                column: "request_concern_id",
                principalTable: "request_concerns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_sub_categories_request_concerns_request_concern_id",
                table: "ticket_sub_categories",
                column: "request_concern_id",
                principalTable: "request_concerns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_sub_categories_sub_categories_sub_category_id",
                table: "ticket_sub_categories",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_categories_categories_category_id",
                table: "ticket_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_categories_request_concerns_request_concern_id",
                table: "ticket_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_sub_categories_request_concerns_request_concern_id",
                table: "ticket_sub_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_sub_categories_sub_categories_sub_category_id",
                table: "ticket_sub_categories");

            migrationBuilder.DropColumn(
                name: "assign_to",
                table: "ticket_concerns");

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "ticket_sub_categories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "request_concern_id",
                table: "ticket_sub_categories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "request_concern_id",
                table: "ticket_categories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "ticket_categories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_categories_categories_category_id",
                table: "ticket_categories",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_categories_request_concerns_request_concern_id",
                table: "ticket_categories",
                column: "request_concern_id",
                principalTable: "request_concerns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_sub_categories_request_concerns_request_concern_id",
                table: "ticket_sub_categories",
                column: "request_concern_id",
                principalTable: "request_concerns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_sub_categories_sub_categories_sub_category_id",
                table: "ticket_sub_categories",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
