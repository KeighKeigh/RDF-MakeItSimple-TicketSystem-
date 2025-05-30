using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableServiceProvider_channel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "service_providers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_provider_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_providers", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_providers_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_providers_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_providers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "service_provider_channels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    service_provider_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_provider_channels", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_provider_channels_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_service_provider_channels_service_providers_service_provider_id",
                        column: x => x.service_provider_id,
                        principalTable: "service_providers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_service_provider_channels_channel_id",
                table: "service_provider_channels",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_provider_channels_service_provider_id",
                table: "service_provider_channels",
                column: "service_provider_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_providers_added_by",
                table: "service_providers",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_service_providers_modified_by",
                table: "service_providers",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_service_providers_user_id",
                table: "service_providers",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_provider_channels");

            migrationBuilder.DropTable(
                name: "service_providers");
        }
    }
}
