using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalDatasetManagementAPI.Migrations
{
    public partial class Database_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Sample_SampleID",
                table: "Image");

            migrationBuilder.AlterColumn<int>(
                name: "SampleID",
                table: "Image",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Sample_SampleID",
                table: "Image",
                column: "SampleID",
                principalTable: "Sample",
                principalColumn: "SAMPLE_ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Sample_SampleID",
                table: "Image");

            migrationBuilder.AlterColumn<int>(
                name: "SampleID",
                table: "Image",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Sample_SampleID",
                table: "Image",
                column: "SampleID",
                principalTable: "Sample",
                principalColumn: "SAMPLE_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
