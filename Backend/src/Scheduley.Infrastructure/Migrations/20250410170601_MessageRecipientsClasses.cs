using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Scheduley.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MessageRecipientsClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("6331808a-17e8-467c-901c-79716b011ee3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: new Guid("d66a4915-156f-4b35-9281-3ec6d8908ed3"));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Messages",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipientID",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhoneNumber",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "Name" },
                values: new object[,]
                {
                    { new Guid("1345caa2-c5dd-42f4-afbe-99b8ab2598ab"), "thecityhunterhd@gmail.com", "Molly" },
                    { new Guid("e006e75f-94d7-4a3e-84bd-0a1dff18735f"), "ahmad.mhfz1412@gmail.com", "Ahmad Mahfouz" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "RecipientID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RecipientPhoneNumber",
                table: "Messages");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "Name" },
                values: new object[,]
                {
                    { new Guid("6331808a-17e8-467c-901c-79716b011ee3"), "thecityhunterhd@gmail.com", "Molly" },
                    { new Guid("d66a4915-156f-4b35-9281-3ec6d8908ed3"), "ahmad.mhfz1412@gmail.com", "Ahmad Mahfouz" }
                });
        }
    }
}
