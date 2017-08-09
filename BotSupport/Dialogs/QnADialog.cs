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
                        knowledgebaseId = "da50c6c1-0e1f-467f-b94a-f82c0b0e1ac7";
                        qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }

                case "44-ФЗ": { break; }
                case "615-ФЗ": { break; }
                case "Имущество":
                    {
                        knowledgebaseId = "89b2fccf-f3bc-4021-8ab1-0496865a8ba2";
                        qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d"; break;
                    }
                case "РТС-Маркет": { break; }
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