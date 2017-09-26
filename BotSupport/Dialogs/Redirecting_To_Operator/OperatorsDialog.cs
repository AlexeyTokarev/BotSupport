using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace BotSupport.Dialogs.Redirecting_To_Operator
{
    [Serializable]
    public class OperatorsDialog : IDialog<object>
    {
        static ConversationResourceResponse convId = null;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)//, string platform, string role, string userQuestion)
        {
            var activity = await result as Activity;

            var operatorsAccount = new ChannelAccount("429719242"); //(OperatorsClass.Id, OperatorsClass.Name);
            var userAccount = new ChannelAccount(activity.From.Id, activity.From.Name); //("mlh89j6hg7k", "Bot");
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            if (convId == null)
            {
                try
                {
                    var conversationId =
                        await connector.Conversations.CreateDirectConversationAsync(operatorsAccount, userAccount);
                    convId = conversationId;
                }
                catch
                {
                    throw new InvalidOperationException();
                }
            }

            string textForOperator = "Кукусики";//$"Площадка: {platform}\nРоль: {role}\nВопрос: {userQuestion}";

            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = userAccount;
            message.Recipient = operatorsAccount;
            message.Conversation = new ConversationAccount(id: convId.Id);
            message.Text = textForOperator;
            await connector.Conversations.SendToConversationAsync((Activity)message);

            context.Wait(MessageReceivedAsync);
        }
    }
}
