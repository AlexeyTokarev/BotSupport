using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QnA
{
    public class QnARequest
    {
        private const string Service = "/qnamaker";

        public static async Task<string> QnAResponse(string urlAddress, string knowledgebaseId, string endPointKey, string question)
        {
            string responseInJson;
            string qnaResult;

            var method = "/knowledgebases/" + knowledgebaseId + "/generateAnswer/";
            var uri = urlAddress + Service + method;
            var postBody = $"{{\"question\": \"{question}\"}}";

            try
            {
                responseInJson = await Post(uri, endPointKey, postBody);
            }
            catch
            {
                return "Извините, произошла ошибка, попробуйте еще раз";
            }

            var response = JsonConvert.DeserializeObject<Response>(responseInJson);
            var firstOrDefault = response.Answers.FirstOrDefault();
            qnaResult = firstOrDefault?.PossibleAnswer.ToString();

            if (!string.IsNullOrEmpty(qnaResult))
            {
                if (qnaResult == "No good match found in the KB" || qnaResult == "No good match found in KB.")
                {
                    return "Прошу прощения, но я не понял вопроса. Попробуйте перефразировать его.";
                }
                return qnaResult.Replace(@"\n", Environment.NewLine);
            }
            return "Извините, произошла ошибка, попробуйте еще раз";
        }

        private static async Task<string> Post(string uri, string endPointKey, string body)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    request.Headers.Add("Authorization", "EndpointKey " + endPointKey);

                    var response = await client.SendAsync(request);
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
