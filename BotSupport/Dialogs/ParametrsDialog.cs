﻿namespace BotSupport.Dialogs
{
    /// <summary>
    /// Класс служит помощником в определении недостающих параметров с последующей подсказкой пользователю
    /// </summary>
    public class ParametrsDialog
    {
        /// <summary>
        /// Метод определяет, какие параметры у нас заполнены, а какие нет
        /// </summary>
        /// <returns></returns>
        public static string CheckParametrs(string platform, string role, string type)
        {
            // Не заполнены поля "Площадка", "Роль", "Тип"
            if (string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return "Прежде чем дать консультацию, я должен понимать, кем Вы являетесь. Дайте мне, пожалуйста, некоторую информацию о Вас:\n" +
                       "1. Кто Вы: заказчик или поставщик?\n" +
                       "2. Какая площадка Вас интересует: 223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n" +
                       "3. К какому типу Вы относитесь: индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено: "Площадка"
            // Не заполнено: "Роль" и "Тип"
            if (!string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить некоторые детали:\n" +
                       "1. Кто Вы: заказчик или поставщик?\n" +
                       "2. К какому типу Вы относитесь: индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено:  "Роль"
            // Не заполнено: "Площадка" и "Тип"
            if (string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить некоторые детали:\n" +
                       "1. Какая площадка Вас интересует: 223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n" +
                       "2. К какому типу Вы относитесь: индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено: "Тип"
            // Не заполнено: "Площадка" и "Роль"
            if (string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить некоторые детали:\n" +
                       "1. Кто Вы: заказчик или поставщик?\n" +
                       "2. Какая площадка Вас интересует: 223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n";
            }

            // Заполнено: "Площадка" и "Роль"
            // Не заполнено: "Тип"
            if (!string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(role) && string.IsNullOrEmpty(type))
            {
                return
                    "Для более полной информации я еще должен уточнить к какому типу Вы относитесь: " +
                    "индивидуальный предприниматель, физическое лицо или юридическое лицо?\n";
            }

            // Заполнено: "Площадка" и "Тип"
            // Не заполнено: "Роль" 
            if (!string.IsNullOrEmpty(platform) && string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(type))
            {
                return "Для более полной информации я еще должен уточнить кто Вы: заказчик или поставщик?\n";
            }

            // Заполнено: "Роль" и "Тип"
            // Не заполнено: "Площадка"
            if (string.IsNullOrEmpty(platform) && !string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(type))
            {
                return
                    "Для более полной информации я еще должен уточнить какая площадка Вас интересует: " +
                    "223-ФЗ, 44-ФЗ, 615-ФЗ, Имущество или РТС-Маркет?\n";
            }

            return "Что-то пошло не так, попробуйте еще раз";
        }
    }
}