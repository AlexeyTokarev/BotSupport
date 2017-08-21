using ApiAi;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace BotSupport.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string platform; //Площадка, по которой пользователь хочет получить консультацию ("223-ФЗ", "44-ФЗ", "615-ФЗ", "Имущество", "РТС-Маркет")
        private string role; // Какова роль пользователя ("Заказчик", "Поставщик")
        //private string type; // Кем является пользователь ("ИП", "ФЛ", "ЮЛ")
        private bool parametrs; // Быстрая проверка наличия всех параметров

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            try
            {
                if (ResetParametrs.Reset(activity?.Text))
                {
                    platform = null;
                    role = null;
                    //type = null;
                    parametrs = false;
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync(ex.Message + "номер 1");
            }

            if (parametrs == false)
            {
                if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(role)) // || string.IsNullOrEmpty(type)
                {
                    if (!string.IsNullOrWhiteSpace(activity?.Text))
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
                            // Проверка наличия, добавление или редактирование параметра "Площадка"
                            if (!string.IsNullOrEmpty(platform))
                            {
                                if ((platform != apiAiResponse.Platform) &&
                                    (!string.IsNullOrEmpty(apiAiResponse.Platform)))
                                {
                                    platform = apiAiResponse.Platform;
                                }
                            }
                            else
                            {
                                platform = apiAiResponse.Platform;
                            }

                            // Проверка наличия, добавление или редактирование параметра "Роль"
                            if (!string.IsNullOrEmpty(role))
                            {
                                if ((role != apiAiResponse.Role) && (!string.IsNullOrEmpty(apiAiResponse.Role)))
                                {
                                    role = apiAiResponse.Role;
                                }
                            }
                            else
                            {
                                role = apiAiResponse.Role;
                            }

                            //// Проверка наличия, добавление или редактирование параметра "Тип"
                            //if (!string.IsNullOrEmpty(type))
                            //{
                            //    if ((type != apiAiResponse.Type) && (!string.IsNullOrEmpty(apiAiResponse.Type)))
                            //    {
                            //        type = apiAiResponse.Type;
                            //    }
                            //}
                            //else
                            //{
                            //    type = apiAiResponse.Type;
                            //}
                        }
                    }
                    else
                    {
                        await context.PostAsync("Что-то пошло не так, повторите попытку");
                    }

                    // Идет проверка наличия всех заполненных и незаполненных параметров с последующим информированием пользователя
                    if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(role)
                    ) // || string.IsNullOrEmpty(type)
                    {
                        //await context.PostAsync(ParametrsDialog.CheckParametrs(platform, role, type));
                        string checkParametrs = ParametrsDialog.CheckParametrs(platform, role);

                        if (string.IsNullOrEmpty(platform))
                        {
                            try
                            {
                                CardDialog.PlatformCard(context, activity, checkParametrs);
                            }
                            catch (Exception ex)
                            {
                                await context.PostAsync(ex.Message + "номер 4");
                            }
                        }

                        if (string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(platform))
                        {
                            if (platform == "Имущество")
                            {
                                try
                                {
                                    CardDialog.RoleCardImuchestvo(context, activity, checkParametrs);
                                }
                                catch (Exception ex)
                                {
                                    await context.PostAsync(ex.Message + "номер 2");
                                }
                            }
                            else
                            {
                                try
                                {
                                    CardDialog.RoleCard(context, activity, checkParametrs);
                                }
                                catch (Exception ex)
                                {
                                    await context.PostAsync(ex.Message + "номер 3");
                                }
                            }
                        }
                    }
                    else
                    {
                        parametrs = true;
                        await context.PostAsync(
                            "Напишите теперь интересующую Вас тему. Для возврата в исходное состояние наберите слово \"сброс\"");
                        activity.Text = null;
                    }
                }
                else
                {
                    parametrs = true;
                    await context.PostAsync("Напишите теперь интересующую Вас тему.");
                }
            }

            if (!string.IsNullOrEmpty(activity?.Text) && parametrs == true)
            {
                var answer = new QnADialog().QnABotResponse(platform, activity.Text);

                // Проверка длины сообщения. Делается потому, как некоторые мессенджеры имеют ограничения на длину сообщения
                if (answer.Length > 3500)
                {
                    while (answer.Length > 3500)
                    {
                        var substringPoint = 3500;

                        // Данный цикл обрабатывает возможность корректного разделения больших сообщений на более мелкие
                        // Причем разделение проводится по предложениям (Ориентиром является точка)
                        while (answer[substringPoint] != '.')
                        {
                            substringPoint--;
                        }

                        var subanswer = answer.Substring(0, substringPoint + 1);

                        await context.PostAsync(subanswer);
                        answer = answer.Remove(0, substringPoint + 1);
                    }
                    await context.PostAsync(answer);
                }
                else
                {
                    await context.PostAsync(answer);
                }
            }
            //context.Wait(MessageReceivedAsync);

        }
    }
}