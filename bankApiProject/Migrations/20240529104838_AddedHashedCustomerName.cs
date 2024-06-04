using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bankApiProject.Migrations
{
    /// <inheritdoc />
    public partial class AddedHashedCustomerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashedCustomerName",
                table: "Customer",
                type: "varchar(255)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashedCustomerName",
                table: "Customer");
        }
    }
}
