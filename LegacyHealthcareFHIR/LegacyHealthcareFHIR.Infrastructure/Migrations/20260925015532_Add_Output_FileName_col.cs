using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyHealthcareFHIR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Output_FileName_col : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OutputFileName",
                table: "ImportJobs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutputFileName",
                table: "ImportJobs");
        }
    }
}
