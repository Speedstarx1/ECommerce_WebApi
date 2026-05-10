using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_picture_url = table.Column<string>(type: "text", nullable: true, collation: "case_insensitive"),
                    first_name = table.Column<string>(type: "varchar(50)", nullable: false, collation: "case_insensitive"),
                    last_name = table.Column<string>(type: "varchar(50)", nullable: false, collation: "case_insensitive"),
                    email = table.Column<string>(type: "varchar(100)", nullable: false, collation: "case_insensitive"),
                    phone_number = table.Column<string>(type: "varchar(50)", nullable: false, collation: "case_insensitive"),
                    password_hash = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    hash_salt = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    user_type_enum = table.Column<string>(type: "varchar(50)", nullable: false),
                    address = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    gender = table.Column<string>(type: "varchar(50)", nullable: false),
                    user_type = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, collation: "case_insensitive"),
                    reference_number = table.Column<string>(type: "varchar(20)", nullable: true, collation: "case_insensitive"),
                    Reference_Number = table.Column<string>(type: "varchar(20)", nullable: true, collation: "case_insensitive"),
                    created_by = table.Column<string>(type: "varchar(255)", nullable: false, collation: "case_insensitive"),
                    modified_by = table.Column<string>(type: "varchar(255)", nullable: true, collation: "case_insensitive"),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_reference_number",
                table: "users",
                column: "reference_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Reference_Number",
                table: "users",
                column: "Reference_Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
