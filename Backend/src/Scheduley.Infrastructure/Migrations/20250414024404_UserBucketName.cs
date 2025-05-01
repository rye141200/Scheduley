using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduley.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserBucketName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BucketName",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("4bf68c7d-f166-4dc0-92cd-9ae4c519da61"),
                column: "BucketName",
                value: new Guid("bb5c4df4-1b56-4f30-9d45-ec7a57743ce6"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("7e91312b-5175-47cf-ab07-8f293772feed"),
                column: "BucketName",
                value: new Guid("0a04b8fb-7416-4bfc-8ef0-7a505fe803a1"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BucketName",
                table: "Users");
        }
    }
}
