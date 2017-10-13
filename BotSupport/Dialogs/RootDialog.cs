using ApiAi;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using BotSupport.Dialogs.Redirecting_To_Operator;
using GoogleTablesWorking;

namespace BotSupport.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string _platform;       // Площадка, по которой пользователь хочет получить консультацию ("223-ФЗ", "44-ФЗ", "615-ФЗ", "Имущество", "РТС-Маркет")
        private string _role;           // Какова роль пользователя ("Заказчик", "Поставщик")
        private bool _parametrs;        // Быстрая проверка наличия всех параметров
        private bool _answerExistence;  // Проверка наличия ответов 
        private string _userQuestion;   // Вопрос пользователя
        private string _answer;         // Ответ пользователя
        private bool _correct;          // Проверка корректности выданного ответа

        // Для работы с перенаправлением сообщений оператору
        private static bool _operatorsConversation;
        static ConversationResourceResponse convId;
        private static string _userId = String.Empty; // Id пользователя. Предназначено для участка кода, работающего с перенаправлением сообщений оператору



        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Проверка на разговор с оператором
            if (_operatorsConversation)
            {
                await ToOperator(context, activity);
                if (ResetParametrs.Reset(activity?.Text))
                {
                    MakeReset();
                    await context.PostAsync("До свидания");
                }
                return;
            }
            //-----------------------------------

            if (_answerExistence)
            {
                if (activity.Text.ToLower() != "да" && activity.Text.ToLower() != "нет")
                {
                    await context.PostAsync("Ответьте, пожалуйста, на вопрос. Нам очень важно Ваше мнение.");
                    Thread.Sleep(1500);
                    CardDialog.SatisfyingAnswer(context, activity);
                    return;
                }
                else
                {
                    if (activity.Text.ToLower() == "да")
                    {
                        _correct = true;
                        //await context.PostAsync("Подождите, пожалуйста, Ваш ответ обрабатывается");
                        await context.PostAsync("Благодарю, Ваш ответ очень помог нам");
                        Thread.Sleep(1500);
                        await context.PostAsync("Если Вас еще что-то интересует, напишите тему");

                        try
                        {
                            AddQuestionInGoogleSheet.SendError(_platform, _role, _userQuestion, _answer, _correct);
                        }
                        catch
                        {
                            //_userQuestion = null;
                            //_answer = null;
                            //_answerExistence = false;
                            //_correct = false;
                            //return;//await context.PostAsync("Возникли проблемы с обработкой Вашего ответа");
                        }
                        _userQuestion = null;
                        _answer = null;
                        _answerExistence = false;
                        _correct = false;
                        return;
                    }
                    if (activity.Text.ToLower() == "нет")
                    {
                        //await context.PostAsync("Подождите, пожалуйста, Ваш ответ обрабатывается");

                        await context.PostAsync("Большое спасибо. Ваше сообщение передано в службу технической поддержки. Приносим извинения за неудобство");
                        Thread.Sleep(1500);
                        await context.PostAsync("Если Вас еще что-то интересует, напишите тему");

                        try
                        {
                            AddQuestionInGoogleSheet.SendError(_platform, _role, _userQuestion, _answer, _correct);
                        }
                        catch
                        {
                            //return; //await context.PostAsync("Возникли проблемы с обработкой Вашего ответа");
                        }
                        _answer = null;
                        _userQuestion = null;
                        _answerExistence = false;
                        return;
                    }
                }
            }

            try
            {
                if (ResetParametrs.Reset(activity?.Text))
                {
                    MakeReset();
                }
            }
            catch
            {
                await context.PostAsync("Возникли проблемы с работой сервиса. Приносим извинения за неудобство");
            }

            if (_parametrs == false)
            {
                if (string.IsNullOrEmpty(_platform) || string.IsNullOrEmpty(_role))
                {
                    if (!string.IsNullOrWhiteSpace(activity?.Text))
                    {

                        var apiAiResponse = ApiAiRequest.ApiAiBotRequest(activity.Text);

                        // Если есть ошибки
                        if (apiAiResponse.Errors != null && apiAiResponse.Errors.Count > 0)
                        {
                            await context.PostAsync("Что-то пошло не так");
                        }

                        // Если нет ошибок
                        else
                        {
                            // Проверка наличия, добавление или редактирование параметра "Площадка"
                            if (!string.IsNullOrEmpty(_platform))
                            {
                                if ((_platform != apiAiResponse.Platform) &&
                                    (!string.IsNullOrEmpty(apiAiResponse.Platform)))
                                {
                                    _platform = apiAiResponse.Platform;
                                }
                            }
                            else
                            {
                                _platform = apiAiResponse.Platform;
                            }

                            // Проверка наличия, добавление или редактирование параметра "Роль"
                            if (!string.IsNullOrEmpty(_role))
                            {
                                if ((_role != apiAiResponse.Role) && (!string.IsNullOrEmpty(apiAiResponse.Role)))
                                {
                                    _role = apiAiResponse.Role;
                                }
                            }
                            else
                            {
                                _role = apiAiResponse.Role;
                            }
                        }

                    }
                    else
                    {
                        await context.PostAsync("Что-то пошло не так");
                    }

                    // Идет проверка наличия всех заполненных и незаполненных параметров с последующим информированием пользователя
                    if (string.IsNullOrEmpty(_platform) || string.IsNullOrEmpty(_role))
                    {
                        string checkParametrs = ParametrsDialog.CheckParametrs(_platform, _role);

                        if (string.IsNullOrEmpty(_platform))
                        {
                            try
                            {
                                CardDialog.PlatformCard(context, activity, checkParametrs);
                            }
                            catch
                            {
                                await context.PostAsync("Что-то пошло не так");
                            }
                        }

                        if (string.IsNullOrEmpty(_role) && !string.IsNullOrEmpty(_platform))
                        {
                            if (_platform == "Имущественные торги")
                            {
                                try
                                {
                                    CardDialog.RoleCardImuchestvo(context, activity, checkParametrs);
                                    return;
                                }
                                catch
                                {
                                    await context.PostAsync("Что-то пошло не так");
                                }
                            }
                            if (_platform == "РТС-Маркет")
                            {
                                try
                                {
                                    CardDialog.RoleCardRTS(context, activity, checkParametrs);
                                    return;
                                }
                                catch
                                {
                                    await context.PostAsync("Что-то пошло не так");
                                }
                            }
                            if (_platform == "615-ПП РФ")
                            {
                                try
                                {
                                    CardDialog.RoleCard615(context, activity, checkParametrs);
                                    return;
                                }
                                catch
                                {
                                    await context.PostAsync("Что-то пошло не так");
                                }
                            }
                            else
                            {
                                try
                                {
                                    CardDialog.RoleCard(context, activity, checkParametrs);
                                    return;
                                }
                                catch
                                {
                                    await context.PostAsync("Что-то пошло не так");
                                }
                            }
                        }
                    }
                    else
                    {
                        _parametrs = true;
                        await context.PostAsync(
                            "Напишите теперь интересующую Вас тему. Для возврата в исходное состояние наберите слово \"сброс\"");
                        activity.Text = null;
                    }
                }
                else
                {
                    _parametrs = true;
                    await context.PostAsync("Напишите теперь интересующую Вас тему.");
                }
            }

            if (!string.IsNullOrEmpty(activity?.Text) && _parametrs)
            {
                try
                {
                    _userQuestion = activity.Text;
                    try
                    {
                        _answer = new QnADialog().QnABotResponse(_platform, _role, _userQuestion);
                    }
                    catch
                    {
                        await context.PostAsync("Что-то пошло не так");
                    }

                    if (_answer == "Прошу прощения, но я не понял вопроса. Попробуйте перефразировать его.")
                    {
                        await context.PostAsync(_answer);
                        _answerExistence = false;

                        // Для включения/выключения функции перенаправления оператору
                        // await ToOperator(context, activity); 
                        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                        return;
                    }

                    string strRegex = @"(\!\[alt text\])\((ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9А-Яа-я \-\.\?\,\'\/\\\+&amp;%\$#_]*)?([ ])?(\""?[a-zA-Z0-9А-Яа-я]*\""?)?\)([;\.,\!\?])?";
                    Regex myRegex = new Regex(strRegex);
                    string imageSubanswer = String.Empty;

                    // Создание копии ответа, для корректного занесения в таблицу ответов 
                    string copyAnswer = _answer;

                    // Проверка длины сообщения. Делается потому, как некоторые мессенджеры имеют ограничения на длину сообщения
                    if (_answer.Length > 3500)
                    {
                        bool wasImages = false;
                        int startPoint = 0;
                        while (copyAnswer.Length > 3500)
                        {
                            var substringPoint = 3500;

                            // Данный цикл обрабатывает возможность корректного разделения больших сообщений на более мелкие
                            // Причем разделение проводится по предложениям (Ориентиром является точка)
                            while (copyAnswer[substringPoint] != '.')
                            {
                                substringPoint--;
                            }

                            var subanswer = copyAnswer.Substring(0, substringPoint + 1);
                            bool img = false;


                            foreach (Match myMatch in myRegex.Matches(subanswer))
                            {
                                if (!myMatch.Success)
                                {
                                    await context.PostAsync(subanswer);
                                    continue;
                                }
                                wasImages = true;
                                imageSubanswer = subanswer.Substring(startPoint, (myMatch.Index - startPoint) + myMatch.Length);

                                await context.PostAsync(imageSubanswer);
                                startPoint = myMatch.Index + myMatch.Length;

                                img = true;
                            }

                            if (!wasImages) await context.PostAsync(subanswer.Substring(startPoint));
                            _answerExistence = true;
                            if (img) copyAnswer = copyAnswer.Remove(0, startPoint);
                            else copyAnswer = copyAnswer.Remove(0, substringPoint + 1);
                        }

                        startPoint = 0;

                        foreach (Match myMatch in myRegex.Matches(copyAnswer))
                        {
                            if (!myMatch.Success)
                            {
                                await context.PostAsync(copyAnswer.Substring(startPoint));
                                continue;
                            }

                            imageSubanswer = copyAnswer.Substring(startPoint, (myMatch.Index - startPoint) + myMatch.Length);
                            wasImages = true;
                            await context.PostAsync(imageSubanswer);
                            startPoint = myMatch.Index + myMatch.Length;
                        }
                        if (!wasImages) await context.PostAsync(copyAnswer);
                        else await context.PostAsync(copyAnswer.Substring(startPoint));

                        _answerExistence = true;
                    }
                    else
                    {
                        int startPoint = 0;
                        foreach (Match myMatch in myRegex.Matches(copyAnswer))
                        {
                            if (!myMatch.Success)
                            {
                                await context.PostAsync(_answer);
                                continue;
                            }
                            imageSubanswer = copyAnswer.Substring(startPoint, (myMatch.Index - startPoint) + myMatch.Length);

                            await context.PostAsync(imageSubanswer);
                            startPoint = myMatch.Index + myMatch.Length;
                        }
                        await context.PostAsync(copyAnswer.Substring(startPoint));

                        _answerExistence = true;
                    }

                    Thread.Sleep(1500);
                    CardDialog.SatisfyingAnswer(context, activity);
                }
                catch
                {
                    await context.PostAsync("Что-то пошло не так");
                }
            }
        }


        //---------------------------------------------УЧАСТОК КОДА С ПЕРЕНАПРАВЛЕНИЕМ СООБЩЕНИЙ ОПЕРАТОРУ-------------------------------------

        public async Task ToOperator(IDialogContext context, Activity activity)
        {
            try
            {
                _operatorsConversation = true;

                string operatorId = "429719242";
                bool toOperator = true;

                if (activity.From.Id == operatorId)
                {
                    toOperator = false;

                    if (String.IsNullOrEmpty(_userId))
                    {
                        await context.PostAsync("Ни одного пользователя не подключено к оператору");
                        return;
                    }
                }
                else
                {
                    _userId = activity.From.Id;
                }

                var serverAccount = new ChannelAccount(activity.Recipient.Id, activity.Recipient.Name);
                var operatorAccount = new ChannelAccount(operatorId);
                var userAccount = new ChannelAccount(_userId);

                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                if (toOperator)
                {
                    var conversationId =
                        connector.Conversations.CreateDirectConversation(serverAccount, operatorAccount);
                    convId = conversationId;
                }
                else
                {
                    var conversationId =
                        connector.Conversations.CreateDirectConversation(serverAccount, userAccount);
                    convId = conversationId;
                }

                string textForOperator = $"Площадка: {_platform}\n\nРоль: {_role}\n\nСообщение: {activity.Text}";

                IMessageActivity message = Activity.CreateMessageActivity();

                message.From = serverAccount;

                if (toOperator)
                {
                    message.Recipient = operatorAccount;
                    message.Text = textForOperator;
                }
                else
                {
                    message.Recipient = userAccount;
                    message.Text = activity.Text;
                }

                message.Conversation = new ConversationAccount(id: convId.Id);

                await connector.Conversations.SendToConversationAsync((Activity)message);
            }
            catch
            {
                await context.PostAsync("Что-то пошло не так");
            }
            context.Wait(MessageReceivedAsync);
        }

        public void MakeReset()
        {
            _platform = null;
            _role = null;
            _parametrs = false;
            _operatorsConversation = false;
            _userQuestion = null;
            _answer = null;
        }
    }
}