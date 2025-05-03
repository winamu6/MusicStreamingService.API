using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRecomendationFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Danceability",
                table: "Songs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Energy",
                table: "Songs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Tempo",
                table: "Songs",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Danceability",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Energy",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Tempo",
                table: "Songs");
        }
    }
}
