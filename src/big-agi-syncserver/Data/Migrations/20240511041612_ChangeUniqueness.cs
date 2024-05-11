using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace big_agi_syncserver.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Messages_ConversationId_Id",
                table: "Messages",
                columns: new[] { "ConversationId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Conversations_SyncKey_Id",
                table: "Conversations",
                columns: new[] { "SyncKey", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Messages_ConversationId_Id",
                table: "Messages");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Conversations_SyncKey_Id",
                table: "Conversations");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");
        }
    }
}
