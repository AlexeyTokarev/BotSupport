using ApiAi;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using GoogleTablesWorking;

namespace BotSupport.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string _platform;       //Площадка, по которой пользователь хочет получить консультацию ("223-ФЗ", "44-ФЗ", "615-ФЗ", "Имущество", "РТС-Маркет")
        private string _role;           // Какова роль пользователя ("Заказчик", "Поставщик")
        private bool _parametrs;        // Быстрая проверка наличия всех параметров
        private bool _answerExistence;  // Проверка наличия ответов 
        private string _userQuestion;   // Вопрос пользователя
        private string _answer;         // Ответ пользователя
        private bool _correct;          // Проверка корректности выданного ответа

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

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
                        await context.PostAsync("Подождите, пожалуйста, Ваш ответ обрабатывается");
                        try
                        {
                            AddQuestionInGoogleSheet.SendError(_platform, _role, _userQuestion, _answer, _correct);
                        }
                        catch (Exception ex)
                        {
                            await context.PostAsync(ex.Message);
                        }
                        await context.PostAsync("Благодарю, Ваш ответ очень помог нам");
                        _userQuestion = null;
                        _answer = null;
                        _answerExistence = false;
                        _correct = false;
                        Thread.Sleep(1500);
                        await context.PostAsync("Если Вас еще что-то интересует, напишите тему");
                        return;
                    }
                    if (activity.Text.ToLower() == "нет")
                    {
                        await context.PostAsync("Подождите, пожалуйста, Ваш ответ обрабатывается");
                        try
                        {
                            AddQuestionInGoogleSheet.SendError(_platform, _role, _userQuestion, _answer, _correct);
                        }
                        catch (Exception ex)
                        {
                            await context.PostAsync(ex.Message);
                        }
                        await context.PostAsync("Большое спасибо. Ваше сообщение передано в службу технической поддержки. Приносим извинения за неудобство");

                        _answer = null;
                        _userQuestion = null;
                        Thread.Sleep(1500);
                        await context.PostAsync("Если Вас еще что-то интересует, напишите тему");
                        _answerExistence = false;
                        return;
                    }
                }
            }
            
            try
            {
                if (ResetParametrs.Reset(activity?.Text))
                {
                    _platform = null;
                    _role = null;
                    _parametrs = false;
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync(ex.Message);
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
                            await context.PostAsync("Что-то пошло не так, повторите попытку");
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
                    if (string.IsNullOrEmpty(_platform) || string.IsNullOrEmpty(_role)) 
                    {
                        string checkParametrs = ParametrsDialog.CheckParametrs(_platform, _role);

                        if (string.IsNullOrEmpty(_platform))
                        {
                            try
                            {
                                CardDialog.PlatformCard(context, activity, checkParametrs);
                            }
                            catch (Exception ex)
                            {
                                await context.PostAsync(ex.Message);
                            }
                        }

                        if (string.IsNullOrEmpty(_role) && !string.IsNullOrEmpty(_platform))
                        {
                            if (_platform == "Имущество")
                            {
                                try
                                {
                                    CardDialog.RoleCardImuchestvo(context, activity, checkParametrs);
                                    return;
                                }
                                catch (Exception ex)
                                {
                                    await context.PostAsync(ex.Message);
                                }
                            }
                            else
                            {
                                try
                                {
                                    CardDialog.RoleCard(context, activity, checkParametrs);
                                    return;
                                }
                                catch (Exception ex)
                                {
                                    await context.PostAsync(ex.Message);
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
                _userQuestion = activity.Text;
                _answer = new QnADialog().QnABotResponse(_platform, _userQuestion + " " + _role);

                if (_answer == "Прошу прощения, но я не понял вопроса. Попробуйте перефразировать его.")
                {
                    await context.PostAsync(_answer);
                    _answerExistence = false;
                    return;
                }

                // Проверка длины сообщения. Делается потому, как некоторые мессенджеры имеют ограничения на длину сообщения
                if (_answer.Length > 3500)
                {
                    // Создание копии ответа, для корректного занесения в таблицу ответов 
                    string copyAnswer = _answer;
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

                        await context.PostAsync(subanswer);
                        copyAnswer = copyAnswer.Remove(0, substringPoint + 1);
                    }
                    await context.PostAsync(copyAnswer);
                    _answerExistence = true;
                }
                else
                {
                    await context.PostAsync(_answer);
                    _answerExistence = true;
                }
                Thread.Sleep(1500);
                CardDialog.SatisfyingAnswer(context, activity);
            }
            //context.Wait(MessageReceivedAsync);
        }
    }
}