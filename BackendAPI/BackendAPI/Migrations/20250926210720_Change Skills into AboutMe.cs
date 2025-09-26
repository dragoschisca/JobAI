using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSkillsintoAboutMe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_CompanyId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CompanyId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "SkillsJson",
                table: "Users",
                newName: "AboutMe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AboutMe",
                table: "Users",
                newName: "SkillsJson");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CompanyId",
                table: "Jobs",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_CompanyId",
                table: "Jobs",
                column: "CompanyId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
