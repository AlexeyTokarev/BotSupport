using ApiAiSDK;
using System;
using System.Linq;

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
                result.Platform = response.Result.Parameters["Platform"].ToString();
            }

            if (response.Result.Parameters.ContainsKey("Role"))
            {
                result.Role = response.Result.Parameters["Role"].ToString();
            }

            if (response.Result.Parameters.ContainsKey("Type"))
            {
                result.Type = response.Result.Parameters["Type"].ToString();
            }

            return result;
        }
    }
}
