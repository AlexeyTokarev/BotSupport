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
        /// <param name="role"></param>
        public void QnAMakerKey(string platform, string role)
        {
            switch (platform)
            {
                case "223-ФЗ":
                    {
                        switch (role)
                        {
                            case "Заказчик":
                                {
                                    knowledgebaseId = "f00b1d47-a513-47d4-81f4-2d1cd66d6031";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "8083e3fd-0224-4114-801d-3ff9605baf8d";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "da50c6c1-0e1f-467f-b94a-f82c0b0e1ac7";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "44-ФЗ":
                    {
                        switch (role)
                        {
                            case "Заказчик":
                                {
                                    knowledgebaseId = "d4171504-e07b-45cf-89ff-179ceb96ae06";
                                    qnamakerSubscriptionKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "f8a08e80-e7e3-43a6-a2a8-a7689d216db2";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "254ec291-e15d-45d0-99c5-15593f38599c";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "615-ПП РФ":
                    {
                        switch (role)
                        {
                            case "Заказчик":
                                {
                                    knowledgebaseId = "0873ecf7-0944-4a19-9149-c642211ece14";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "a31cc3c4-1550-4594-bbe5-9f8dd7500a34";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                            case "ОВР":
                                {
                                knowledgebaseId = "589984e8-8462-4c64-bfb1-fbaaca8e4983";
                                qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "71728f39-34d9-4dbb-bfbe-85d8fb2dfa96";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "Имущественные торги":
                    {
                        switch (role)
                        {
                            case "Продавец":
                                {
                                    knowledgebaseId = "cba6012c-eb4f-4626-9bca-34b5d9ed22bb";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                            case "Покупатель":
                                {
                                    knowledgebaseId = "371b93b7-bb15-46bb-96bc-305cb3bea947";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "89b2fccf-f3bc-4021-8ab1-0496865a8ba2";
                        //qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "Электронный магазин ЗМО":
                    {
                        switch (role)
                        {
                            case "Заказчик":
                                {
                                    knowledgebaseId = "189b6314-63ab-415d-ba15-bdba7cf5c81d";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "04accdfe-85f1-45f5-898d-dca8a30b2683";
                                    qnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
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
        /// <param name="role"></param>
        /// <param name="qnaResponse"></param>
        /// <returns></returns>
        public string QnABotResponse(string platform, string role, string qnaResponse)
        {
            QnAMakerKey(platform, role);

            string qnaResult = QnARequest.QnAResponse(knowledgebaseId, qnamakerSubscriptionKey, qnaResponse);

            // Добавлена очистка от ненужных символов
            //Regex regex = new Regex(@"&#\d{3};");
            //qnaResult = regex.Replace(qnaResult, " ");

            return HttpUtility.HtmlDecode(qnaResult);
        }
    }
}
