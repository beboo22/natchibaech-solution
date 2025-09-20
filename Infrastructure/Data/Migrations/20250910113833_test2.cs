using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderDiscountCodes_OrderDiscountCodeId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderDiscountCodes_OrderDiscountCodeId",
                table: "Orders",
                column: "OrderDiscountCodeId",
                principalTable: "OrderDiscountCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderDiscountCodes_OrderDiscountCodeId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderDiscountCodes_OrderDiscountCodeId",
                table: "Orders",
                column: "OrderDiscountCodeId",
                principalTable: "OrderDiscountCodes",
                principalColumn: "Id");
        }
    }
}
