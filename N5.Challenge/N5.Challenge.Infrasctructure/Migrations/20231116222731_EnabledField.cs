using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N5.Challenge.Infrasctructure.Migrations
{
    /// <inheritdoc />
    public partial class EnabledField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Permissions",
                type: "bit",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Permissions");
        }
    }
}
