using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotSupport.Dialogs
{
    [Serializable]
    public class StartConversation : IDialog<object>
    {
        private static readonly IDictionary<string, bool> state = new Dictionary<string, bool>();

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(Resume);
            return Task.CompletedTask;
        }

        private async Task Resume(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = (Activity)await result;
            bool sentGreeting;
            if (!state.TryGetValue(activity.Conversation.Id, out sentGreeting))
            {
                state[activity.Conversation.Id] = true;
                await context.PostAsync("Здравствуйте! Я Бот Электронной площадки РТС-тендер.");
                Thread.Sleep(1500);
                CardDialog.PlatformCard(context, activity, ParametrsDialog.CheckParametrs(null,null));
                context.Done<object>(null);
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                await context.Forward(new RootDialog(), async (dialogContext, res) => dialogContext.Done(await res), activity, CancellationToken.None);
            }
        }
    }

}