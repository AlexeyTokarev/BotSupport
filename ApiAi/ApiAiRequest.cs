using ApiAiSDK;
using System;

namespace ApiAi
{
    public class ApiAiRequest
    {
        private const string ClientAccessToken = "7451c558ed494ff6a510b32eb16ed7d2";

        public static ApiAiResult ApiAiBotRequest(string request)
        {
            var result = new ApiAiResult();

            if (String.IsNullOrWhiteSpace(request))
            {
                result.Errors.Add("Напишите, пожалуйста, Ваш запрос! Что-то пошло не так");
                return result;
            }

            // Конфигурация агента Api.ai
            var config = new AIConfiguration(ClientAccessToken, SupportedLanguage.Russian);
            var apiAi = new ApiAiSDK.ApiAi(config);

            // Ответ
            var response = apiAi.TextRequest(request);

            if (response == null || response.Result == null || response.Result.Parameters == null)
            {
                result.Errors.Add("Напишите, пожалуйста, Ваш запрос! Что-то пошло не так");
                return result;
            }

            if (response.Result.Parameters.ContainsKey("Platform"))
            {
                string platform = response.Result.Parameters["Platform"].ToString();

                if (platform == "223-ФЗ" || platform == "44-ФЗ" || platform == "615-ПП РФ" || platform == "Имущественные торги" ||
                    platform == "Электронный магазин ЗМО")
                {
                    result.Platform = platform;
                }
                else
                {
                    result.Platform = null;
                }
            }

            if (response.Result.Parameters.ContainsKey("Role"))
            {
                string role = response.Result.Parameters["Role"].ToString();

                if (role == "Поставщик" || role == "Заказчик" || role == "Продавец" || role == "Покупатель" || role == "ОВР")
                {
                    result.Role = role;
                }
                else
                {
                    result.Role = null;
                }
            }

            if (response.Result.Parameters.ContainsKey("Type"))
            {
                string type = response.Result.Parameters["Type"].ToString();

                if (type == "ИП" || type == "ФЛ" || type == "ЮЛ")
                {
                    result.Type = type;
                }
                else
                {
                    result.Type = null;
                }
            }

            return result;
        }
    }
}
