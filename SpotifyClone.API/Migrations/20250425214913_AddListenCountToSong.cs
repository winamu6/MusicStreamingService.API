using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddListenCountToSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListenCount",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListenCount",
                table: "Songs");
        }
    }
}
