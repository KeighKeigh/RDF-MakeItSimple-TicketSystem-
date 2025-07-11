using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignToRelationshipOnRequestConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id2",
                table: "request_concerns");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_assign_to",
                table: "request_concerns",
                column: "assign_to");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns",
                column: "assign_to",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id2",
                table: "request_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id3",
                table: "request_concerns",
                column: "reject_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id2",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id3",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_assign_to",
                table: "request_concerns");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id2",
                table: "request_concerns",
                column: "reject_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
