using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace big_agi_syncserver.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConversationFolders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Conversations = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SyncKey = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    SystemPurposeId = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated = table.Column<long>(type: "INTEGER", nullable: false),
                    UserTitle = table.Column<string>(type: "TEXT", nullable: true),
                    AutoTitle = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Avatar = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Created = table.Column<long>(type: "INTEGER", nullable: false),
                    PurposeId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Sender = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    TokenCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Typing = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated = table.Column<long>(type: "INTEGER", nullable: true),
                    ConversationId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationFolders");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Conversations");
        }
    }
}
