using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Scheduley.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RequiredUserEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("3f5289fd-8f4b-41f0-8093-9383839152fd"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("da9bac65-3b7c-467d-8793-f7b453223d47"));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "Name" },
                values: new object[,]
                {
                    { new Guid("6331808a-17e8-467c-901c-79716b011ee3"), "thecityhunterhd@gmail.com", "Molly" },
                    { new Guid("d66a4915-156f-4b35-9281-3ec6d8908ed3"), "ahmad.mhfz1412@gmail.com", "Ahmad Mahfouz" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("6331808a-17e8-467c-901c-79716b011ee3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("d66a4915-156f-4b35-9281-3ec6d8908ed3"));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "Name" },
                values: new object[,]
                {
                    { new Guid("3f5289fd-8f4b-41f0-8093-9383839152fd"), "ahmad.mhfz1412@gmail.com", "Ahmad Mahfouz" },
                    { new Guid("da9bac65-3b7c-467d-8793-f7b453223d47"), "thecityhunterhd@gmail.com", "Molly" }
                });
        }
    }
}
