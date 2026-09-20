using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyHealthcareFHIR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMappingConfigurationToImportJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "ImportJobs");

            migrationBuilder.DropColumn(
                name: "FailedRecords",
                table: "ImportJobs");

            migrationBuilder.DropColumn(
                name: "ProgressPercentage",
                table: "ImportJobs");

            migrationBuilder.DropColumn(
                name: "StartedAtUtc",
                table: "ImportJobs");

            migrationBuilder.DropColumn(
                name: "SuccessfulRecords",
                table: "ImportJobs");

            migrationBuilder.DropColumn(
                name: "TotalRecords",
                table: "ImportJobs");

            migrationBuilder.AddColumn<int>(
                name: "MappingConfigurationId",
                table: "ImportJobs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_MappingConfigurationId",
                table: "ImportJobs",
                column: "MappingConfigurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportJobs_MappingConfigurations_MappingConfigurationId",
                table: "ImportJobs",
                column: "MappingConfigurationId",
                principalTable: "MappingConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportJobs_MappingConfigurations_MappingConfigurationId",
                table: "ImportJobs");

            migrationBuilder.DropIndex(
                name: "IX_ImportJobs_MappingConfigurationId",
                table: "ImportJobs");

            migrationBuilder.DropColumn(
                name: "MappingConfigurationId",
                table: "ImportJobs");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "ImportJobs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedRecords",
                table: "ImportJobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProgressPercentage",
                table: "ImportJobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAtUtc",
                table: "ImportJobs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuccessfulRecords",
                table: "ImportJobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRecords",
                table: "ImportJobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
