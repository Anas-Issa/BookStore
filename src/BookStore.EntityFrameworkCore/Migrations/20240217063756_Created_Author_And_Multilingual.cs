using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace BookStore.Migrations;

/// <inheritdoc />
public partial class Created_Author_And_Multilingual : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "AuthorId",
            table: "AppBooks",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateTable(
            name: "AppAuthors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ShortBio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppAuthors", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AppBookTranslations",
            columns: table => new
            {
                BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Language = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppBookTranslations", x => new { x.BookId, x.Language });
                table.ForeignKey(
                    name: "FK_AppBookTranslations_AppBooks_BookId",
                    column: x => x.BookId,
                    principalTable: "AppBooks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AppBooks_AuthorId",
            table: "AppBooks",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_AppAuthors_Name",
            table: "AppAuthors",
            column: "Name");

        migrationBuilder.AddForeignKey(
            name: "FK_AppBooks_AppAuthors_AuthorId",
            table: "AppBooks",
            column: "AuthorId",
            principalTable: "AppAuthors",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_AppBooks_AppAuthors_AuthorId",
            table: "AppBooks");

        migrationBuilder.DropTable(
            name: "AppAuthors");

        migrationBuilder.DropTable(
            name: "AppBookTranslations");

        migrationBuilder.DropIndex(
            name: "IX_AppBooks_AuthorId",
            table: "AppBooks");

        migrationBuilder.DropColumn(
            name: "AuthorId",
            table: "AppBooks");
    }
}
