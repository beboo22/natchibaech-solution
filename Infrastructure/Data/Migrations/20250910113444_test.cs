using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberShips_MemberShipDiscountCodes_MemberShipDiscountCodeId",
                table: "MemberShips");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_MemberShips_MemberShipId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberShips_MemberShipDiscountCodes_MemberShipDiscountCodeId",
                table: "MemberShips",
                column: "MemberShipDiscountCodeId",
                principalTable: "MemberShipDiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_MemberShips_MemberShipId",
                table: "Transactions",
                column: "MemberShipId",
                principalTable: "MemberShips",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberShips_MemberShipDiscountCodes_MemberShipDiscountCodeId",
                table: "MemberShips");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_MemberShips_MemberShipId",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberShips_MemberShipDiscountCodes_MemberShipDiscountCodeId",
                table: "MemberShips",
                column: "MemberShipDiscountCodeId",
                principalTable: "MemberShipDiscountCodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_MemberShips_MemberShipId",
                table: "Transactions",
                column: "MemberShipId",
                principalTable: "MemberShips",
                principalColumn: "Id");
        }
    }
}
