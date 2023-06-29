namespace ChatBotCoachWebsite.Models
{
    public class MessageAndConversationModel
    {
        public MessageModel MessageModel { get; set; }
        public List<OpenAI_API.Chat.ChatMessage> ChatConversation { get; set; }
    }
}
