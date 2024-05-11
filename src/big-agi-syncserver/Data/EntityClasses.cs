using System.Text.Json.Serialization;

namespace big_agi_syncserver.Data
{
    public class Message
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // guid
        
        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }
        
        [JsonPropertyName("created")]
        public long Created { get; set; }
        
        [JsonPropertyName("purposeId")]
        public string? PurposeId { get; set; }
        
        [JsonPropertyName("role")]
        public string Role { get; set; }
        
        [JsonPropertyName("sender")]
        public string Sender { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; } // varchar(max)
        
        [JsonPropertyName("tokenCount")]
        public int TokenCount { get; set; }
        
        [JsonPropertyName("typing")]
        public bool Typing { get; set; }
        
        [JsonPropertyName("updated")]
        public long? Updated { get; set; }

        [JsonPropertyName("conversationId")]
        public string ConversationId { get; set; }
        
        [JsonPropertyName("conversation")]
        public Conversation Conversation { get; set; } = null!;
    }

    public class Conversation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // guid
        
        [JsonPropertyName("syncKey")]
        public string SyncKey { get; set; } // guid
        
        [JsonPropertyName("systemPurposeId")]
        public string? SystemPurposeId { get; set; } // e.g. DeveloperPreview
        
        [JsonPropertyName("created")]
        public long Created { get; set; }
        
        [JsonPropertyName("updated")]
        public long Updated { get; set; }
        
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = null!;
        
        [JsonPropertyName("userTitle")]
        public string? UserTitle { get; set; }
        
        [JsonPropertyName("autoTitle")]
        public string? AutoTitle { get; set; }
        
    }

    public class ConversationFolder
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // guid
        
        [JsonPropertyName("color")]
        public string Color { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }

        //public List<string> Conversations { get; set; } = [];
        [JsonPropertyName("conversationIds")]
        public List<string>? ConversationIds { get; set; }

    }
}
