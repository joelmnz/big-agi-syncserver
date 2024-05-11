using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace big_agi_syncserver.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixConversationFolderIDsName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Conversations",
                table: "ConversationFolders",
                newName: "ConversationIds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConversationIds",
                table: "ConversationFolders",
                newName: "Conversations");
        }
    }
}
