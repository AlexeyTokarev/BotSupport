using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace QnA
{
    public class QnARequest
    {
        // Идентификатор базы знаний
        const string KnowledgebaseId = "da50c6c1-0e1f-467f-b94a-f82c0b0e1ac7";

        // Использование ключа подписи в QnA Maker
        const string QnamakerSubscriptionKey = "850a8ac4def146498ab7e2161cd87c9d";

        // Адрес для запроса к базе знаний QnA Maker
        private const string UrlAddress = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0";

        static void Main(string[] args)
        {
            Console.Title = "Console-Bot";
            for (;;)
            {
                Console.Clear();
                string responseString;
                Console.Write("Введите запрос: ");

                // Запрос пользователя
                var query = Console.ReadLine();

                // Вписать адрес для работы с классом URI
                var qnamakerUriBase = new Uri(UrlAddress);
                var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{KnowledgebaseId}/generateAnswer");

                // Добавление вопроса как части тела
                var postBody = $"{{\"question\": \"{query}\"}}";

                // Отсылаем ПОСТ запрос
                using (var client = new WebClient())
                {
                    // Изменияем кодировку
                    client.Encoding = System.Text.Encoding.UTF8;

                    // Добавляем заголовок ключа подписи
                    client.Headers.Add("Ocp-Apim-Subscription-Key", QnamakerSubscriptionKey);
                    client.Headers.Add("Content-Type", "application/json");

                    try
                    {
                        responseString = client.UploadString(builder.Uri, postBody);
                    }
                    catch
                    {
                        continue;
                    }

                    // Создам переменную, которая из класса ResponseModel возвращает нам PossibleAnswer
                    var response = JsonConvert.DeserializeObject<Response>(responseString);

                    var firstOrDefault = response.Answers.FirstOrDefault();

                    if (firstOrDefault != null)
                    {
                        Console.WriteLine(firstOrDefault.PossibleAnswer);
                    }
                    else
                    {
                        continue;
                    }
                }

                Console.ReadLine();
            }
        }
    }
}
