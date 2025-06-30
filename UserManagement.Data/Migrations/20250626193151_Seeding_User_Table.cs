using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seeding_User_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "Forename", "IsActive", "Surname" },
                values: new object[,]
                {
                    { 1L, new DateOnly(1993, 11, 12), "ploew@example.com", "Peter", true, "Loew" },
                    { 2L, new DateOnly(1985, 4, 17), "bfgates@example.com", "Benjamin Franklin", true, "Gates" },
                    { 3L, new DateOnly(1992, 8, 23), "ctroy@example.com", "Castor", false, "Troy" },
                    { 4L, new DateOnly(1976, 2, 5), "mraines@example.com", "Memphis", true, "Raines" },
                    { 5L, new DateOnly(2001, 11, 30), "sgodspeed@example.com", "Stanley", true, "Goodspeed" },
                    { 6L, new DateOnly(1968, 7, 12), "himcdunnough@example.com", "H.I.", true, "McDunnough" },
                    { 7L, new DateOnly(1998, 5, 19), "cpoe@example.com", "Cameron", false, "Poe" },
                    { 8L, new DateOnly(1954, 9, 3), "emalus@example.com", "Edward", false, "Malus" },
                    { 9L, new DateOnly(1989, 3, 27), "dmacready@example.com", "Damon", false, "Macready" },
                    { 10L, new DateOnly(1973, 6, 14), "jblaze@example.com", "Johnny", true, "Blaze" },
                    { 11L, new DateOnly(1982, 10, 8), "rfeld@example.com", "Robin", true, "Feld" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11L);
        }
    }
}
