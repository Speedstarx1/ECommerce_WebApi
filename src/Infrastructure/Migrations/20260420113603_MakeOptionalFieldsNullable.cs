using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeOptionalFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "users",
                type: "varchar(50)",
                nullable: true,
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldCollation: "case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "users",
                type: "varchar(255)",
                nullable: true,
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldCollation: "case_insensitive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "users",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true,
                oldCollation: "case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "users",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldCollation: "case_insensitive");
        }
    }
}
