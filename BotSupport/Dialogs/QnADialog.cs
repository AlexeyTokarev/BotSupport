using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private string endpointKey; // Использование ключа подписи в QnA Maker

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
                                    knowledgebaseId = "d4171504-e07b-45cf-89ff-179ceb96ae06";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "fb284162-92fa-4abf-a572-dbb9902ed237";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "da50c6c1-0e1f-467f-b94a-f82c0b0e1ac7";
                        //endpointKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "44-ФЗ":
                    {
                        switch (role)
                        {
                            case "Заказчик":
                                {
                                    knowledgebaseId = "d4171504-e07b-45cf-89ff-179ceb96ae06";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "fb284162-92fa-4abf-a572-dbb9902ed237";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "254ec291-e15d-45d0-99c5-15593f38599c";
                        //endpointKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "615-ПП РФ":
                    {
                        switch (role)
                        {
                            case "Заказчик":
                                {
                                    knowledgebaseId = "d4171504-e07b-45cf-89ff-179ceb96ae06";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "fb284162-92fa-4abf-a572-dbb9902ed237";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                            case "ОВР":
                                {
                                knowledgebaseId = "d4171504-e07b-45cf-89ff-179ceb96ae06";
                                endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "71728f39-34d9-4dbb-bfbe-85d8fb2dfa96";
                        //endpointKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "Имущественные торги":
                    {
                        switch (role)
                        {
                            case "Продавец":
                                {
                                    knowledgebaseId = "d4171504-e07b-45cf-89ff-179ceb96ae06";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                            case "Покупатель":
                                {
                                    knowledgebaseId = "fb284162-92fa-4abf-a572-dbb9902ed237";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "89b2fccf-f3bc-4021-8ab1-0496865a8ba2";
                        //endpointKey = "850a8ac4def146498ab7e2161cd87c9d";
                        break;
                    }
                case "Электронный магазин ЗМО":
                    {
                        switch (role)
                        {
                            case "Заказчик":
                                {
                                    knowledgebaseId = "d4171504-e07b-45cf-89ff-179ceb96ae06";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "fb284162-92fa-4abf-a572-dbb9902ed237";
                                    endpointKey = "e1ef79e9-ae78-47fe-bace-5758e56d494a";
                                    break;
                                }
                        }
                        //Первоначальный вариант общей базы знаний
                        //knowledgebaseId = "49bb744a-b569-43a1-a905-f9bdcdbc6aa4";
                        //endpointKey = "850a8ac4def146498ab7e2161cd87c9d";
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
        public async Task<string> QnABotResponse(string platform, string role, string qnaResponse)
        {
            QnAMakerKey(platform, role);

            string qnaResult = await QnARequest.QnAResponse(knowledgebaseId, endpointKey, qnaResponse);

            // Добавлена очистка от ненужных символов
            //Regex regex = new Regex(@"&#\d{3};");
            //qnaResult = regex.Replace(qnaResult, " ");

            return HttpUtility.HtmlDecode(qnaResult);
        }
    }
}
