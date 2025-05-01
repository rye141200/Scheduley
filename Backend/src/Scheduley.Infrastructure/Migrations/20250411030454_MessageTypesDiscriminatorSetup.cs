using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Scheduley.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MessageTypesDiscriminatorSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("1345caa2-c5dd-42f4-afbe-99b8ab2598ab"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("e006e75f-94d7-4a3e-84bd-0a1dff18735f"));

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "Name" },
                values: new object[,]
                {
                    { new Guid("4bf68c7d-f166-4dc0-92cd-9ae4c519da61"), "thecityhunterhd@gmail.com", "Molly" },
                    { new Guid("7e91312b-5175-47cf-ab07-8f293772feed"), "ahmad.mhfz1412@gmail.com", "Ahmad Mahfouz" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("4bf68c7d-f166-4dc0-92cd-9ae4c519da61"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("7e91312b-5175-47cf-ab07-8f293772feed"));

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Messages",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "Name" },
                values: new object[,]
                {
                    { new Guid("1345caa2-c5dd-42f4-afbe-99b8ab2598ab"), "thecityhunterhd@gmail.com", "Molly" },
                    { new Guid("e006e75f-94d7-4a3e-84bd-0a1dff18735f"), "ahmad.mhfz1412@gmail.com", "Ahmad Mahfouz" }
                });
        }
    }
}
