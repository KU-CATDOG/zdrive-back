using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zdrive_back.Migrations
{
    /// <inheritdoc />
    public partial class EditStudentNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentNums_Members_StudentNumber",
                table: "StudentNums");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Members_StudentNumber",
                table: "Members");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_StudentNums_StudentNumber",
                table: "Members",
                column: "StudentNumber",
                principalTable: "StudentNums",
                principalColumn: "StudentNumber",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_StudentNums_StudentNumber",
                table: "Members");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Members_StudentNumber",
                table: "Members",
                column: "StudentNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentNums_Members_StudentNumber",
                table: "StudentNums",
                column: "StudentNumber",
                principalTable: "Members",
                principalColumn: "StudentNumber");
        }
    }
}
