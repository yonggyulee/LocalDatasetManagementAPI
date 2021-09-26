using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalDatasetManagementAPI.Migrations
{
    public partial class Database_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sample",
                columns: table => new
                {
                    SAMPLE_ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DATASET_ID = table.Column<string>(type: "TEXT", nullable: true),
                    SAMPLE_TYPE = table.Column<int>(type: "INTEGER", nullable: false),
                    METADATA = table.Column<string>(type: "TEXT", nullable: true),
                    IMAGE_COUNT = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sample", x => x.SAMPLE_ID);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    IMAGE_ID = table.Column<string>(type: "TEXT", nullable: false),
                    IMAGE_NO = table.Column<int>(type: "INTEGER", nullable: false),
                    IMAGE_CODE = table.Column<string>(type: "TEXT", nullable: true),
                    ORIGINAL_FILENAME = table.Column<string>(type: "TEXT", nullable: true),
                    IMAGE_SCHEMA = table.Column<string>(type: "TEXT", nullable: true),
                    SampleID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.IMAGE_ID);
                    table.ForeignKey(
                        name: "FK_Image_Sample_SampleID",
                        column: x => x.SampleID,
                        principalTable: "Sample",
                        principalColumn: "SAMPLE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_SampleID",
                table: "Image",
                column: "SampleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "Sample");
        }
    }
}
