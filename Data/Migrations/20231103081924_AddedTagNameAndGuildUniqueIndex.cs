using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedTagNameAndGuildUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_GuildId_Name",
                table: "Tags");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_GuildId_Name",
                table: "Tags",
                columns: new[] { "GuildId", "Name" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_GuildId_Name",
                table: "Tags");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_GuildId_Name",
                table: "Tags",
                columns: new[] { "GuildId", "Name" });
        }
    }
}
