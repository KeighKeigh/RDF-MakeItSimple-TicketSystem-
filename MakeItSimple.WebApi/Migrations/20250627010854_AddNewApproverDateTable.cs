using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNewApproverDateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "approver_date_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "approver_dates",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    added_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: false),
                    is_approved = table.Column<bool>(type: "bit", nullable: true),
                    approved_date_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    date_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approved_date_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_reject_date = table.Column<bool>(type: "bit", nullable: false),
                    reject_date_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reject_date_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reject_date_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reject_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ticket_approver = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approver_dates", x => x.id);
                    table.ForeignKey(
                        name: "fk_approver_dates_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_approver_dates_users_added_by_user_id",
                        column: x => x.added_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_approver_dates_users_approved_date_by_user_id",
                        column: x => x.approved_date_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_approver_dates_users_reject_date_by_user_id",
                        column: x => x.reject_date_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_approver_date_id",
                table: "approver_ticketings",
                column: "approver_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_dates_added_by_user_id",
                table: "approver_dates",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_dates_approved_date_by_user_id",
                table: "approver_dates",
                column: "approved_date_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_dates_reject_date_by_user_id",
                table: "approver_dates",
                column: "reject_date_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_dates_ticket_concern_id",
                table: "approver_dates",
                column: "ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_approver_dates_approver_date_id",
                table: "approver_ticketings",
                column: "approver_date_id",
                principalTable: "approver_dates",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_approver_dates_approver_date_id",
                table: "approver_ticketings");

            migrationBuilder.DropTable(
                name: "approver_dates");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_approver_date_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "approver_date_id",
                table: "approver_ticketings");
        }
    }
}
