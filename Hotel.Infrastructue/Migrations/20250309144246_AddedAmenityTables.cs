using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastructue.Migrations
{
    /// <inheritdoc />
    public partial class AddedAmenityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenity_Villas_VillaId",
                table: "Amenity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Amenity",
                table: "Amenity");

            migrationBuilder.RenameTable(
                name: "Amenity",
                newName: "Amenities");

            migrationBuilder.RenameIndex(
                name: "IX_Amenity_VillaId",
                table: "Amenities",
                newName: "IX_Amenities_VillaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Amenities",
                table: "Amenities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_Villas_VillaId",
                table: "Amenities",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_Villas_VillaId",
                table: "Amenities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Amenities",
                table: "Amenities");

            migrationBuilder.RenameTable(
                name: "Amenities",
                newName: "Amenity");

            migrationBuilder.RenameIndex(
                name: "IX_Amenities_VillaId",
                table: "Amenity",
                newName: "IX_Amenity_VillaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Amenity",
                table: "Amenity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Amenity_Villas_VillaId",
                table: "Amenity",
                column: "VillaId",
                principalTable: "Villas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
