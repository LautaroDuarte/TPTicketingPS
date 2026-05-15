using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPTicketingPS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSeatVersionToRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Seats");

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Seats",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "Seats",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);
        }
    }
}
