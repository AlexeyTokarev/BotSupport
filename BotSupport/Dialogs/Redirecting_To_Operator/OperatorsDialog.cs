using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace BotSupport.Dialogs.Redirecting_To_Operator
{
    public class OperatorsDialog
    {
        static ConversationResourceResponse convId = null;
        
        public static void StartOperatorsDialog(IAwaitable<object> result, string platform, string role, string userQuestion)
        {
            var activity =  result as Activity;

            var operatorsAccount = new ChannelAccount("429719242"); //(OperatorsClass.Id, OperatorsClass.Name);
            var userAccount = new ChannelAccount(activity.From.Id, activity.From.Name); //("mlh89j6hg7k", "Bot");
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            if (convId == null)
            {
                try
                {
                    var conversationId =
                         connector.Conversations.CreateDirectConversation(operatorsAccount, userAccount);
                    convId = conversationId;
                }
                catch
                {
                    throw new InvalidOperationException();
                }
            }

            string textForOperator = $"Площадка: {platform}\nРоль: {role}\nВопрос: {userQuestion}";

            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = userAccount;
            message.Recipient = operatorsAccount;
            message.Conversation = new ConversationAccount(id: convId.Id);
            message.Text = textForOperator;
            connector.Conversations.SendToConversation((Activity)message);
        }
    }
}
