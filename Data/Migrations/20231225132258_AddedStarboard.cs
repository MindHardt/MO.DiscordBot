using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedStarboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "StarboardChannelId",
                table: "Guilds",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StarboardQuotes",
                columns: table => new
                {
                    OriginalMessageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    QuoteMessageId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DiscordGuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarboardQuotes", x => x.OriginalMessageId);
                    table.ForeignKey(
                        name: "FK_StarboardQuotes_Guilds_DiscordGuildId",
                        column: x => x.DiscordGuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StarboardTracks",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Emoji = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StarboardThreshold = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarboardTracks", x => new { x.GuildId, x.Emoji });
                    table.ForeignKey(
                        name: "FK_StarboardTracks_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StarboardQuotes_DiscordGuildId",
                table: "StarboardQuotes",
                column: "DiscordGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_StarboardTracks_GuildId",
                table: "StarboardTracks",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StarboardQuotes");

            migrationBuilder.DropTable(
                name: "StarboardTracks");

            migrationBuilder.DropColumn(
                name: "StarboardChannelId",
                table: "Guilds");
        }
    }
}
