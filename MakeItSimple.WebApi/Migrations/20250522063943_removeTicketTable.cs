using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeTicketTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    back_job_id = table.Column<int>(type: "int", nullable: true),
                    business_unit_id = table.Column<int>(type: "int", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    company_id = table.Column<int>(type: "int", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    location_id = table.Column<int>(type: "int", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sub_category_id = table.Column<int>(type: "int", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    unit_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    concern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    concern_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_needed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    handled_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    handled_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_done = table.Column<bool>(type: "bit", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    request_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    severity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_tickets_business_units_business_unit_id",
                        column: x => x.business_unit_id,
                        principalTable: "business_units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_sub_units_sub_unit_id",
                        column: x => x.sub_unit_id,
                        principalTable: "sub_units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_tickets_back_job_id",
                        column: x => x.back_job_id,
                        principalTable: "tickets",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_users_added_by",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_users_modified_by",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_tickets_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_tickets_added_by",
                table: "tickets",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_back_job_id",
                table: "tickets",
                column: "back_job_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_business_unit_id",
                table: "tickets",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_category_id",
                table: "tickets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_channel_id",
                table: "tickets",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_company_id",
                table: "tickets",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_department_id",
                table: "tickets",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_location_id",
                table: "tickets",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_modified_by",
                table: "tickets",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_sub_category_id",
                table: "tickets",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_sub_unit_id",
                table: "tickets",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_unit_id",
                table: "tickets",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_tickets_user_id",
                table: "tickets",
                column: "user_id");
        }
    }
}
