using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using big_agi_syncserver.Data;
using Microsoft.EntityFrameworkCore;

namespace big_agi_syncserver;

public static partial class Endpoints
{
    public static async Task<long> FullSync(
        HttpRequest request,
        ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var syncKey = request.RouteValues["syncKey"]?.ToString();

        if (!IsSyncKeyValid(syncKey))
            throw new ValidationException("syncKey is not valid");

        var syncData = await request.ReadFromJsonAsync<FullSyncDto>(cancellationToken);
        if (syncData is null)
            throw new ValidationException("syncData is not valid");

        long updateCount = 0;

        try
        {
            if (syncData.Conversations is { Count: 0 })
                return 0;

            // sync/overwrite all conversations
            foreach (var conversation in syncData.Conversations)
            {
                // exists, check last updated etc and overwrite if no conflicts
                var existingConversation = await context.Conversations
                    .AsTracking()
                    .Include(x => x.Messages)
                    .FirstOrDefaultAsync(x =>
                            x.SyncKey == syncKey && x.Id == conversation.Id,
                        cancellationToken);

                if (existingConversation is not null)
                {
                    // check last updated
                    if (existingConversation.Updated >= conversation.Updated)
                        continue;

                    // overwrite
                    existingConversation.Updated = conversation.Updated;
                    existingConversation.UserTitle = conversation.UserTitle;
                    existingConversation.AutoTitle = conversation.AutoTitle;
                    existingConversation.SystemPurposeId = conversation.SystemPurposeId;

                    // check existing messages
                    foreach (var existingMessage in existingConversation.Messages)
                    {
                        // update / overwrite?
                        var newMessage = conversation.Messages.FirstOrDefault(x => x.Id == existingMessage.Id);
                        if (newMessage is not null)
                        {
                            if (existingMessage.Updated > newMessage.Updated)
                                continue;

                            existingMessage.Updated = newMessage.Updated;
                            existingMessage.Text = newMessage.Text;
                            existingMessage.TokenCount = newMessage.TokenCount;
                        }
                        else
                        {
                            // message not found, delete it?
                            context.Messages.Remove(existingMessage);
                        }
                    }

                    // check for new messages
                    var existingMessageIDs = existingConversation.Messages.Select(x => x.Id).ToList();
                    // find all messages in conversation.Messages that are not in existingMessageIDs
                    var newMessages = conversation.Messages
                        .Where(x => !existingMessageIDs.Contains(x.Id))
                        .ToList();

                    // add new messages
                    newMessages.ForEach(x => x.Conversation = existingConversation);
                    context.Messages.AddRange(newMessages);
                }
                else
                {
                    conversation.SyncKey = syncKey;
                    context.Conversations.Add(conversation);
                    // bulk add the messages
                    conversation.Messages.ForEach(x => x.Conversation = conversation);
                    conversation.Messages.AddRange(conversation.Messages);
                }

                updateCount++;
            }

            // try updating folders...
            if (syncData.Folders?.Folders is { Count: > 0 })
            {
                // there aren't many folders, just bulk load
                var existingFolders = await context.ConversationFolders
                    .AsTracking()
                    .ToListAsync(cancellationToken);

                // updating existing folders
                foreach (var folder in syncData.Folders.Folders)
                {
                    var existingFolder = existingFolders.FirstOrDefault(x => x.Id == folder.Id);
                    if (existingFolder is not null)
                    {
                        existingFolder.Title = folder.Title;
                        existingFolder.Color = folder.Color;
                        existingFolder.ConversationIds = folder.ConversationIds;
                    }
                    else
                    {
                        //folder.SyncKey = syncKey;
                        context.ConversationFolders.Add(folder);
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return updateCount;
    }


    public class FullSyncDto
    {
        [JsonPropertyName("conversations")]
        public List<Conversation> Conversations { get; set; } = [];

        [JsonPropertyName("folders")]
        public SyncFolder Folders { get; set; } = new();

        // there is a modules object but we won't store that for now
        [JsonPropertyName("modules")]
        public Models Modules { get; set; } = new Models();

        // modules: [ sources: [ id,lable,setup,vId ]]

        public class SyncFolder
        {
            public bool enableFolders { get; set; }

            [JsonPropertyName("folders")]
            public List<ConversationFolder> Folders { get; set; } = [];
        }

        public class ModelSource
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public ModelSetup Setup { get; set; }
            public string vId { get; set; }
        }

        public class ModelSetup
        {
            public string oaiKey { get; set; }
            public string oaiHost { get; set; }
            public string groqKey { get; set; }
        }

        public class Models
        {
            public List<ModelSource> Sources { get; set; }
        }
    }
}