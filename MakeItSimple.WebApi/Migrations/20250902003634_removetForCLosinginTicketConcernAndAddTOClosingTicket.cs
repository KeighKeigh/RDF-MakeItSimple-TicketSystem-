using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removetForCLosinginTicketConcernAndAddTOClosingTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "for_closing_date",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<DateTime>(
                name: "for_closing_at",
                table: "closing_tickets",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "for_closing_at",
                table: "closing_tickets");

            migrationBuilder.AddColumn<DateTime>(
                name: "for_closing_date",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);
        }
    }
}
