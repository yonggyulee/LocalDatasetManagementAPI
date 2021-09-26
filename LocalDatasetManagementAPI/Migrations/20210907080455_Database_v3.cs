using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalDatasetManagementAPI.Migrations
{
    public partial class Database_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Sample_SampleID",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "SampleID",
                table: "Image",
                newName: "SAMPLE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Image_SampleID",
                table: "Image",
                newName: "IX_Image_SAMPLE_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Sample_SAMPLE_ID",
                table: "Image",
                column: "SAMPLE_ID",
                principalTable: "Sample",
                principalColumn: "SAMPLE_ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Sample_SAMPLE_ID",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "SAMPLE_ID",
                table: "Image",
                newName: "SampleID");

            migrationBuilder.RenameIndex(
                name: "IX_Image_SAMPLE_ID",
                table: "Image",
                newName: "IX_Image_SampleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Sample_SampleID",
                table: "Image",
                column: "SampleID",
                principalTable: "Sample",
                principalColumn: "SAMPLE_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
