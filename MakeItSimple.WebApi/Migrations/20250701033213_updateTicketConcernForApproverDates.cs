using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketConcernForApproverDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id2",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id3",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<Guid>(
                name: "approved_date_by",
                table: "ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_approved_at",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_date_approved",
                table: "ticket_concerns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "approved_date_at",
                table: "approver_dates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_approved_date_by",
                table: "ticket_concerns",
                column: "approved_date_by");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id2",
                table: "ticket_concerns",
                column: "approved_date_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id3",
                table: "ticket_concerns",
                column: "closed_approve_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id6",
                table: "ticket_concerns",
                column: "transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id2",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id3",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id6",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_approved_date_by",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "approved_date_by",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "date_approved_at",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "is_date_approved",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "approved_date_at",
                table: "approver_dates");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id2",
                table: "ticket_concerns",
                column: "closed_approve_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id3",
                table: "ticket_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns",
                column: "transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
