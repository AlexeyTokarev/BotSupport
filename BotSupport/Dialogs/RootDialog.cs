using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ApiAi;
using QnA;

namespace BotSupport.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string platform; //Площадка, по которой пользователь хочет получить консультацию ("223-ФЗ", "44-ФЗ", "615-ФЗ", "Имущество", "РТС-Маркет")
        private string role; // Какова роль пользователя ("Заказчик", "Поставщик")
        private string type; // Кем является пользователь ("ИП", "ФЛ", "ЮЛ")

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //// calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;
            if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(type))
            {
                await context.PostAsync("Необходимая информация еще не собрана!");

                if (!string.IsNullOrWhiteSpace(activity.Text))
                {
                    var apiAiResponse = ApiAiRequest.ApiAiBotRequest(activity.Text);
                    
                    // Если есть ошибки
                    if (apiAiResponse.Errors != null && apiAiResponse.Errors.Count > 0)
                    {
                        await context.PostAsync("Что-то пошло не так, повторите попытку");
                    } 
                    
                    // Если нет ошибок
                    else 
                    {
                        if (!string.IsNullOrEmpty(platform))
                        {
                            if (platform != apiAiResponse.Platform.ToString())
                                platform = apiAiResponse.Platform;
                        }
                        else
                        {
                            platform = apiAiResponse.Platform;
                        }

                        if (string.IsNullOrEmpty(role)) role = apiAiResponse.Role;
                        if (string.IsNullOrEmpty(type)) type = apiAiResponse.Type;


                    }
                }
                // return our reply to the user
                //await context.PostAsync($"Текст твоего запроса => {activity.Text}");
                else
                {
                    await context.PostAsync("Что-то пошло не так, повторите попытку");
                }
            }
            else await context.PostAsync($"Площадка: {platform} Роль: {role} Тип: {type}");
            context.Wait(MessageReceivedAsync);
        }
    }
}