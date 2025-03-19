using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addPmsApprover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pms_approvers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    pms_form_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    approver_level = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_approvers", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_approvers_pms_forms_pms_form_id",
                        column: x => x.pms_form_id,
                        principalTable: "pms_forms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_approvers_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pms_approvers_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pms_approvers_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_approvers_added_by",
                table: "pms_approvers",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_approvers_modified_by",
                table: "pms_approvers",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_approvers_pms_form_id",
                table: "pms_approvers",
                column: "pms_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_approvers_user_id",
                table: "pms_approvers",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pms_approvers");
        }
    }
}
