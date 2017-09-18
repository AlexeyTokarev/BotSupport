using System.Text.RegularExpressions;
using System.Web;
using QnA;

namespace BotSupport.Dialogs
{
    /// <summary>
    /// Класс отвечает за работу с ботом QnA Maker
    /// </summary>
    public class QnADialog
    {
        private string knowledgebaseId; // Идентификатор базы знаний для бота QnA Maker
        private string qnamakerSubscriptionKey; // Использование ключа подписи в QnA Maker

        /// <summary>
        /// Данный метод назначает необходимые ключи для работы с ботом QnA Maker
        /// </summary>
        /// <param name="platform"></param>
        public void QnAMakerKey(string platform)
        {
            switch (platform)
            {
                case "223-ФЗ":
                    {
                        knowledgebaseId = "da1322df-d05f-4abc-9167-251e07720d65";
                        qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        //Первоначальный вариант базы знаний
                        //knowledgebaseId = "da50c6c1-0e1f-467f-b94a-f82c0b0e1ac7";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "44-ФЗ":
                    {
                        knowledgebaseId = "354c29e4-105e-445a-821a-95d5e1cd01a4";
                        qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        //Первоначальный вариант базы знаний
                        //knowledgebaseId = "254ec291-e15d-45d0-99c5-15593f38599c";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "615-ФЗ":
                    {
                        knowledgebaseId = "993b8e64-67af-43df-9803-d28b321b568c";
                        qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        //Первоначальный вариант базы знаний
                        //knowledgebaseId = "71728f39-34d9-4dbb-bfbe-85d8fb2dfa96";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "Имущество":
                    {
                        knowledgebaseId = "a9c5d4c3-7fdd-4b40-8d22-f461b4ccce79";
                        qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        //Первоначальный вариант базы знаний
                        //knowledgebaseId = "89b2fccf-f3bc-4021-8ab1-0496865a8ba2";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "РТС-Маркет":
                    {
                        knowledgebaseId = "3d6e4726-4355-4d7d-a231-43730b3b7231";
                        qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        //Первоначальный вариант базы знаний
                        //knowledgebaseId = "49bb744a-b569-43a1-a905-f9bdcdbc6aa4";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
            }
        }

        /// <summary>
        /// Данныый метод посылает запросы на бота QnA Maker и получает ответы на эти запросы
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="qnaResponse"></param>
        /// <returns></returns>
        public string QnABotResponse(string platform, string qnaResponse)
        {
            QnAMakerKey(platform);

            string qnaResult = QnARequest.QnAResponse(knowledgebaseId, qnamakerSubscriptionKey, qnaResponse);

            // Добавлена очистка от ненужных символов
            //Regex regex = new Regex(@"&#\d{3};");
            //qnaResult = regex.Replace(qnaResult, " ");

            return HttpUtility.HtmlDecode(qnaResult);
        }
    }
}