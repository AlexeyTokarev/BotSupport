using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace QnA
{
    public class QnARequest
    {
        // Адрес для запроса к базе знаний QnA Maker
        private const string UrlAddress = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0";

        public static string QnAResponse(string knowledgebaseId, string qnamakerSubscriptionKey, string query)
        {
            string responseString;
            string qnaResult;

            // Вписать адрес для работы с классом URI
            var qnamakerUriBase = new Uri(UrlAddress);
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            // Добавление вопроса как части тела
            var postBody = $"{{\"question\": \"{query}\"}}";

            // Отсылаем ПОСТ запрос
            using (var client = new WebClient())
            {
                // Изменияем кодировку
                client.Encoding = System.Text.Encoding.UTF8;

                // Добавляем заголовок ключа подписи
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");

                try
                {
                    responseString = client.UploadString(builder.Uri, postBody);
                }
                catch
                {
                    return "Извините, произошла ошибка, попробуйте еще раз";
                }

                // Создам переменную, которая из класса ResponseModel возвращает нам PossibleAnswer
                var response = JsonConvert.DeserializeObject<Response>(responseString);

                var firstOrDefault = response.Answers.FirstOrDefault();

                qnaResult = firstOrDefault?.PossibleAnswer.ToString();
            }

            if (!string.IsNullOrEmpty(qnaResult))
            {
                if (qnaResult == "No good match found in the KB")
                {
                    return "Прошу прощения, но я не понял вопроса. Попробуйте перефразировать его.";
                }
                else
                {
                    return qnaResult.Replace(@"\n", Environment.NewLine);
                }
            }
            else
            {
                return "Извините, произошла ошибка, попробуйте еще раз";
            }
        }
    }
}
