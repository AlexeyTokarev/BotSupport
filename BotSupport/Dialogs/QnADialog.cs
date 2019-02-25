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
                                    knowledgebaseId = "f47d5d4d-020c-4bfa-a1b8-ff7e6dd9006d";
                                    endpointKey = "8b92d007-3e61-4b97-9c16-78b43c1137c9";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "e404ca60-4053-40ab-b7b7-a4e6253d098c";
                                    endpointKey = "64ef379e-fe25-41f8-a6ab-9631f104a5af";
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
                                    knowledgebaseId = "ab2ff4af-54aa-4ae9-88d3-fed812c59867";
                                    endpointKey = "0f4b263c-c124-48ad-a643-46cca0183079";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "70080988-383e-447d-812f-d57ebbefa08e";
                                    endpointKey = "2f589635-ace0-449c-b1f7-3809417dbd1b";
                                    break;
                                }
                            case "ОВР":
                                {
                                knowledgebaseId = "bb3a09d3-1eed-4762-9e93-75ee0f48847b";
                                endpointKey = "64ef379e-fe25-41f8-a6ab-9631f104a5af";
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
                                    knowledgebaseId = "37a04429-a3ae-4b79-be22-75f05522ee6e";
                                    endpointKey = "0f4b263c-c124-48ad-a643-46cca0183079";
                                    break;
                                }
                            case "Покупатель":
                                {
                                    knowledgebaseId = "340c3a9f-7631-47a8-b1ce-03396e159c85";
                                    endpointKey = "c00ebcf1-f0cb-46f4-bdc5-73652ae5c3d8";
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
                                    knowledgebaseId = "c282ad4b-f315-42dd-a553-36dbc1424ccf";
                                    endpointKey = "8b92d007-3e61-4b97-9c16-78b43c1137c9";
                                    break;
                                }
                            case "Поставщик":
                                {
                                    knowledgebaseId = "7327b727-f804-4115-a13a-83d81b81d3cf";
                                    endpointKey = "8a592c05-f40d-4159-b6c5-25ad06d5034a";
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
