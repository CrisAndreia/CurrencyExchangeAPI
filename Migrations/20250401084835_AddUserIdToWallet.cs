using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchangeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Wallets",
                newName: "UserId");

            migrationBuilder.CreateTable(
                name: "WalletBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WalletId = table.Column<int>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletBalance_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletBalance_WalletId",
                table: "WalletBalance",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletBalance");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Wallets",
                newName: "Currency");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Wallets",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Wallets",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
