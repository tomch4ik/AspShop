using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspShop.Migrations
{
    /// <inheritdoc />
    public partial class FeedbacksChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Feedbacks",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Feedbacks",
                newName: "CreateAt");
        }
    }
}
