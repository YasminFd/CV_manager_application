using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proj.Data.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Resumes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_userId",
                table: "Resumes",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_AspNetUsers_userId",
                table: "Resumes",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_AspNetUsers_userId",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_userId",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Resumes");
        }
    }
}
