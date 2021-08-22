using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BuyANZCoupon.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponId = table.Column<string>(nullable: false),
                    CouponCode = table.Column<string>(nullable: true),
                    userId = table.Column<Guid>(nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CodeType = table.Column<int>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
