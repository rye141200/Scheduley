using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduley.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserCreatedAndLastLoginDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("4bf68c7d-f166-4dc0-92cd-9ae4c519da61"),
                columns: new[] { "CreatedAt", "LastLogin", "UserRole" },
                values: new object[] { new DateTime(2025, 4, 14, 3, 58, 8, 478, DateTimeKind.Utc).AddTicks(2476), new DateTime(2025, 4, 14, 3, 58, 8, 478, DateTimeKind.Utc).AddTicks(2476), 0 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("7e91312b-5175-47cf-ab07-8f293772feed"),
                columns: new[] { "CreatedAt", "LastLogin", "UserRole" },
                values: new object[] { new DateTime(2025, 4, 14, 3, 58, 8, 478, DateTimeKind.Utc).AddTicks(2445), new DateTime(2025, 4, 14, 3, 58, 8, 478, DateTimeKind.Utc).AddTicks(2448), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "Users");
        }
    }
}
