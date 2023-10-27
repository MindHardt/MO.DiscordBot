using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedInlineTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InlineTagsEnabled",
                table: "Guilds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TagPrefix",
                table: "Guilds",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "$");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InlineTagsEnabled",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "TagPrefix",
                table: "Guilds");
        }
    }
}
