using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyHealthcareFHIR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initital_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hospitals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HospitalKey = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospitals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MappingConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HospitalId = table.Column<int>(type: "INTEGER", nullable: false),
                    SchemaFingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    ResourceType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappingConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceTypeDetections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HospitalId = table.Column<int>(type: "INTEGER", nullable: false),
                    SchemaFingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    ResourceType = table.Column<int>(type: "INTEGER", nullable: false),
                    AiConfidence = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTypeDetections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceFileData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImportJobId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Headers = table.Column<string>(type: "TEXT", nullable: false),
                    SampleRecords = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceFileData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HospitalConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HospitalId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceSystemName = table.Column<string>(type: "TEXT", nullable: false),
                    InputFormat = table.Column<string>(type: "TEXT", nullable: false),
                    EnableAiValidation = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableAiMapping = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalConfigurations_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfigId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceField = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedField = table.Column<string>(type: "TEXT", nullable: false),
                    AiConfidence = table.Column<decimal>(type: "TEXT", nullable: true),
                    AiExplanation = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldMappings_MappingConfigurations_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "MappingConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HospitalId = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", nullable: false),
                    StoredFileName = table.Column<string>(type: "TEXT", nullable: false),
                    InputFormat = table.Column<string>(type: "TEXT", nullable: false),
                    ResourceTypeDetectionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    MappingConfigurationId = table.Column<int>(type: "INTEGER", nullable: true),
                    JobStage = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportJobs_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImportJobs_MappingConfigurations_MappingConfigurationId",
                        column: x => x.MappingConfigurationId,
                        principalTable: "MappingConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ImportJobs_ResourceTypeDetections_ResourceTypeDetectionId",
                        column: x => x.ResourceTypeDetectionId,
                        principalTable: "ResourceTypeDetections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FhirResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HospitalId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImportJobId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResourceType = table.Column<string>(type: "TEXT", nullable: false),
                    ResourceId = table.Column<string>(type: "TEXT", nullable: false),
                    FhirJson = table.Column<string>(type: "TEXT", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidationMessage = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FhirResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FhirResources_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FhirResources_ImportJobs_ImportJobId",
                        column: x => x.ImportJobId,
                        principalTable: "ImportJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FhirResources_HospitalId",
                table: "FhirResources",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_FhirResources_ImportJobId",
                table: "FhirResources",
                column: "ImportJobId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldMappings_ConfigId",
                table: "FieldMappings",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_HospitalConfigurations_HospitalId",
                table: "HospitalConfigurations",
                column: "HospitalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_Code",
                table: "Hospitals",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_HospitalKey",
                table: "Hospitals",
                column: "HospitalKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_HospitalId",
                table: "ImportJobs",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_MappingConfigurationId",
                table: "ImportJobs",
                column: "MappingConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportJobs_ResourceTypeDetectionId",
                table: "ImportJobs",
                column: "ResourceTypeDetectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MappingConfigurations_HospitalId_SchemaFingerprint_ResourceType",
                table: "MappingConfigurations",
                columns: new[] { "HospitalId", "SchemaFingerprint", "ResourceType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTypeDetections_HospitalId_SchemaFingerprint",
                table: "ResourceTypeDetections",
                columns: new[] { "HospitalId", "SchemaFingerprint" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SourceFileData_ImportJobId",
                table: "SourceFileData",
                column: "ImportJobId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FhirResources");

            migrationBuilder.DropTable(
                name: "FieldMappings");

            migrationBuilder.DropTable(
                name: "HospitalConfigurations");

            migrationBuilder.DropTable(
                name: "SourceFileData");

            migrationBuilder.DropTable(
                name: "ImportJobs");

            migrationBuilder.DropTable(
                name: "Hospitals");

            migrationBuilder.DropTable(
                name: "MappingConfigurations");

            migrationBuilder.DropTable(
                name: "ResourceTypeDetections");
        }
    }
}
