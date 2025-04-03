using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchangeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExchangeRates",
                table: "ExchangeRates");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Wallets",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Wallets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ExchangeRates",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExchangeRates",
                table: "ExchangeRates",
                columns: new[] { "BaseCurrency", "DateReceived" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExchangeRates",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Wallets");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ExchangeRates",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExchangeRates",
                table: "ExchangeRates",
                column: "Id");
        }
    }
}
