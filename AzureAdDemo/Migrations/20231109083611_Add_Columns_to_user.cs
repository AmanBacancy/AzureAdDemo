using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureAdDemo.Migrations
{
    /// <inheritdoc />
    public partial class Add_Columns_to_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "User",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "dbo",
                table: "User",
                type: "varchar(100)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "dbo",
                table: "User");
        }
    }
}
