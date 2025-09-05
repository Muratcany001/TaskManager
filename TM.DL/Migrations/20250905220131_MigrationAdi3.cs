using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MigrationAdi3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Tasks_TaskId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Versions_TaskVersionId",
                table: "Documents");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Tasks_TaskId",
                table: "Documents",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Versions_TaskVersionId",
                table: "Documents",
                column: "TaskVersionId",
                principalTable: "Versions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Tasks_TaskId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Versions_TaskVersionId",
                table: "Documents");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Tasks_TaskId",
                table: "Documents",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Versions_TaskVersionId",
                table: "Documents",
                column: "TaskVersionId",
                principalTable: "Versions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
