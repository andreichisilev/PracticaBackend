using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectPractica.Migrations
{
    /// <inheritdoc />
    public partial class removedUserAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}
