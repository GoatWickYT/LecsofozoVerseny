using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Solution.Database.Migrations
{
    /// <inheritdoc />
    public partial class tweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_City_PostalCode",
                table: "Location");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Location",
                newName: "CityId");

            migrationBuilder.RenameIndex(
                name: "IX_Location_PostalCode",
                table: "Location",
                newName: "IX_Location_CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Race_LocationId",
                table: "Race",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_City_CityId",
                table: "Location",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Race_Location_LocationId",
                table: "Race",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_City_CityId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Race_Location_LocationId",
                table: "Race");

            migrationBuilder.DropIndex(
                name: "IX_Race_LocationId",
                table: "Race");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "Location",
                newName: "PostalCode");

            migrationBuilder.RenameIndex(
                name: "IX_Location_CityId",
                table: "Location",
                newName: "IX_Location_PostalCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_City_PostalCode",
                table: "Location",
                column: "PostalCode",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
