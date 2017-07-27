using ApiAiSDK;
using System;

namespace ApiAi
{
    public class Class1
    {
        private const string ClientAccessToken = "7451c558ed494ff6a510b32eb16ed7d2";

        static void Main(string[] args)
        {
            Console.Title = "Console-ApiAi-Bot";

            for (;;)
            {
                Console.Clear();
                Console.Write("Введите запрос: ");

                var responseString = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(responseString))
                {
                    Console.WriteLine("\nНапишите, пожалуйста, Ваш запрос!");
                    Console.ReadLine();
                    continue;
                }

                // Конфигурация агента Api.ai
                var config = new AIConfiguration(ClientAccessToken, SupportedLanguage.Russian);
                var apiAi = new ApiAiSDK.ApiAi(config);

                // Ответ
                var response = apiAi.TextRequest(responseString);
                if (response == null) continue;

                string platform = response.Result.Parameters["Platform"].ToString();
                string type = response.Result.Parameters["Type"].ToString();
                string role = response.Result.Parameters["Role"].ToString();

                if (!string.IsNullOrEmpty(platform)) Console.WriteLine($"Платформа: {platform}");
                else Console.WriteLine("Платформа: пусто");

                if (!string.IsNullOrEmpty(type)) Console.WriteLine($"Тип: {type}");
                else Console.WriteLine("Тип: пусто");

                if (!string.IsNullOrEmpty(role)) Console.WriteLine($"Роль: {role}");
                else Console.WriteLine("Роль: пусто");
                //Console.WriteLine(response.Result.Parameters.Values);
                //Console.WriteLine("Ответ: {0}", response.Result.Fulfillment.Speech);

                Console.ReadLine();
            }
        }
    }
}
