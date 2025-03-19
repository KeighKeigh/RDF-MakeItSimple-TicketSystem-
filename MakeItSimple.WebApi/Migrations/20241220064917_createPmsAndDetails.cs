using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createPmsAndDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pms_form_id = table.Column<int>(type: "int", nullable: true),
                    requestor = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    is_approved = table.Column<bool>(type: "bit", nullable: false),
                    is_rejected = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_pms_forms_pms_form_id",
                        column: x => x.pms_form_id,
                        principalTable: "pms_forms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pms_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pms_users_requestor_by_user_id",
                        column: x => x.requestor,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pms_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pms_id = table.Column<int>(type: "int", nullable: true),
                    pms_questionaire_module_id = table.Column<int>(type: "int", nullable: true),
                    pms_questionaire_id = table.Column<int>(type: "int", nullable: true),
                    pms_question_type_id = table.Column<int>(type: "int", nullable: false),
                    answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_details_pms_pms_id",
                        column: x => x.pms_id,
                        principalTable: "pms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_details_pms_question_types_pms_question_type_id",
                        column: x => x.pms_question_type_id,
                        principalTable: "pms_question_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pms_details_pms_questionaire_modules_pms_questionaire_module_id",
                        column: x => x.pms_questionaire_module_id,
                        principalTable: "pms_questionaire_modules",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_details_pms_questionaires_pms_questionaire_id",
                        column: x => x.pms_questionaire_id,
                        principalTable: "pms_questionaires",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_added_by",
                table: "pms",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_modified_by",
                table: "pms",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_pms_form_id",
                table: "pms",
                column: "pms_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_requestor",
                table: "pms",
                column: "requestor");

            migrationBuilder.CreateIndex(
                name: "ix_pms_details_pms_id",
                table: "pms_details",
                column: "pms_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_details_pms_question_type_id",
                table: "pms_details",
                column: "pms_question_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_details_pms_questionaire_id",
                table: "pms_details",
                column: "pms_questionaire_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_details_pms_questionaire_module_id",
                table: "pms_details",
                column: "pms_questionaire_module_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pms_details");

            migrationBuilder.DropTable(
                name: "pms");
        }
    }
}
