using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addModelForPmsApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pms_approvals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    approver_level = table.Column<int>(type: "int", nullable: true),
                    pms_id = table.Column<int>(type: "int", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_approvals", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_approvals_pms_pms_id",
                        column: x => x.pms_id,
                        principalTable: "pms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_approvals_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pms_approvals_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pms_attachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_size = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    pms_id = table.Column<int>(type: "int", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_attachments_pms_pms_id",
                        column: x => x.pms_id,
                        principalTable: "pms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_attachments_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pms_histories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    request = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transacted_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    transaction_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approver_level = table.Column<int>(type: "int", nullable: true),
                    pms_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_histories_pms_pms_id",
                        column: x => x.pms_id,
                        principalTable: "pms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_histories_users_transacted_by_user_id",
                        column: x => x.transacted_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pms_technicians",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    pms_id = table.Column<int>(type: "int", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_technicians", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_technicians_pms_pms_id",
                        column: x => x.pms_id,
                        principalTable: "pms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_technicians_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_approvals_added_by",
                table: "pms_approvals",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_approvals_pms_id",
                table: "pms_approvals",
                column: "pms_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_approvals_user_id",
                table: "pms_approvals",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_attachments_added_by",
                table: "pms_attachments",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_attachments_pms_id",
                table: "pms_attachments",
                column: "pms_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_histories_pms_id",
                table: "pms_histories",
                column: "pms_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_histories_transacted_by",
                table: "pms_histories",
                column: "transacted_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_technicians_pms_id",
                table: "pms_technicians",
                column: "pms_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_technicians_user_id",
                table: "pms_technicians",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pms_approvals");

            migrationBuilder.DropTable(
                name: "pms_attachments");

            migrationBuilder.DropTable(
                name: "pms_histories");

            migrationBuilder.DropTable(
                name: "pms_technicians");
        }
    }
}
