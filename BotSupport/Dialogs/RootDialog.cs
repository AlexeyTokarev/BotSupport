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
        private bool parametrs = false; // Быстрая проверка наличия всех параметров
        private string knowledgebaseId; // Идентификатор базы знаний для бота QnA Maker
        private string qnamakerSubscriptionKey; // Использование ключа подписи в QnA Maker

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (parametrs == false)
            {
                //// calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;
                if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(type))
                {


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
                            // Проверка наличия, добавление или редактирование параметра "Площадка"
                            if (!string.IsNullOrEmpty(platform))
                            {
                                if ((platform != apiAiResponse.Platform) &&
                                    (!string.IsNullOrEmpty(apiAiResponse.Platform)))
                                    platform = apiAiResponse.Platform;
                            }
                            else
                            {
                                platform = apiAiResponse.Platform;
                            }

                            // Проверка наличия, добавление или редактирование параметра "Роль"
                            if (!string.IsNullOrEmpty(role))
                            {
                                if ((role != apiAiResponse.Role) && (!string.IsNullOrEmpty(apiAiResponse.Role)))
                                    role = apiAiResponse.Role;
                            }
                            else
                            {
                                role = apiAiResponse.Role;
                            }

                            // Проверка наличия, добавление или редактирование параметра "Тип"
                            if (!string.IsNullOrEmpty(type))
                            {
                                if ((type != apiAiResponse.Type) && (!string.IsNullOrEmpty(apiAiResponse.Type)))
                                    type = apiAiResponse.Type;
                            }
                            else
                            {
                                type = apiAiResponse.Type;
                            }
                        }
                    }

                    // return our reply to the user
                    //await context.PostAsync($"Текст твоего запроса => {activity.Text}");
                    else
                    {
                        await context.PostAsync("Что-то пошло не так, повторите попытку");
                    }

                    // Идет проверка наличия всех заполненных и незаполненных параметров с последующим информированием пользователя
                    if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(type))
                    {
                        await context.PostAsync(CheckParametrs());
                    }
                    else
                    {
                        parametrs = true;
                        await context.PostAsync("Напишите теперь интересующую Вас тему.");
                        activity.Text = null;
                        //await context.PostAsync(QnABotResponse(activity.Text));
                    }

                }
                else
                {
                    parametrs = true;
                    await context.PostAsync("Напишите теперь интересующую Вас тему.");
                }
            }

            if (!string.IsNullOrEmpty(activity.Text)&&parametrs==true)
            {
                await context.PostAsync(QnABotResponse(activity.Text));
            }
            
            context.Wait(MessageReceivedAsync);
        }

        /// <summary>
        /// Определяет, какие параметры у нас заполнены, а какие нет
        /// </summary>
        /// <returns></returns>
        string CheckParametrs()
        {
            // Не заполнены поля "Площадка", "Роль", "Тип"
            if (string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return "Прежде чем дать консультацию, я должен понимать, кем Вы являетесь. Дайте мне, пожалуйста, некоторую информацию о Вас (отвечайте на вопросы по очереди):\n" +
                    "1. Кто Вы: заказчик или поставщик?\n" +
                    "2. Какая площадка Вас интересует: 223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n" +
                    "3. К какому типу Вы относитесь: индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено: "Площадка"
            // Не заполнено: "Роль" и "Тип"
            if (!string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить некоторые детали:\n" +
                       "1. Кто Вы: заказчик или поставщик?\n" +
                       "2. К какому типу Вы относитесь: индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено:  "Роль"
            // Не заполнено: "Площадка" и "Тип"
            if (string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить некоторые детали:\n" +
                       "1. Какая площадка Вас интересует: 223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n" +
                       "2. К какому типу Вы относитесь: индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено: "Тип"
            // Не заполнено: "Площадка" и "Роль"
            if (string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить некоторые детали:\n" +
                       "1. Кто Вы: заказчик или поставщик?\n" +
                       "2. Какая площадка Вас интересует: 223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n";
            }

            // Заполнено: "Площадка" и "Роль"
            // Не заполнено: "Тип"
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return
                    "Для более полной информации я еще должен уточнить к какому типу Вы относитесь: " +
                    "индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено: "Площадка" и "Тип"
            // Не заполнено: "Роль" 
            if (!string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить кто Вы: заказчик или поставщик?\n";
            }

            // Заполнено: "Роль" и "Тип"
            // Не заполнено: "Площадка"
            if (string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(type))
            {
                return
                    "Для более полной информации я еще должен уточнить какая площадка Вас интересует: " +
                    "223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n";
            }

            return "Что-то пошло не так, попробуйте еще раз";
        }

        public void QnAMakerKey()
        {
            if (platform == "223-ФЗ")
            {
                knowledgebaseId = "da50c6c1-0e1f-467f-b94a-f82c0b0e1ac7";
                qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
            }

            if (platform == "44-ФЗ") {};
            if (platform == "615-ФЗ") {};
            if (platform == "Имущество") {};
            if (platform == "РТС-Маркет") {};
        }

        string QnABotResponse(string qnaResponse)
        {
            QnAMakerKey();
            string qnaResult = QnARequest.QnAResponse(knowledgebaseId, qnamakerSubscriptionKey, qnaResponse);
            return qnaResult;
        }
    }
}