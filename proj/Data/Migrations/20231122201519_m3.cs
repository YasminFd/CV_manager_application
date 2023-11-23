using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proj.Data.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "Resumes");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Resumes",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Resumes",
                newName: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Resumes",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Resumes",
                newName: "phone");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
