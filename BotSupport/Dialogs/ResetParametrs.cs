using System.Collections.Generic;

namespace BotSupport.Dialogs
{
    public class ResetParametrs
    {
        public static bool Reset(string query)
        {
            var resetPhrases = new List<string>()
            {
                "restart",
                "/start",
                "новый вопрос",
                "сброс",
                "заново",
                "reset",
                "новый вопрос",
                "еще вопрос",
                "еще один вопрос",
                "перезапуск",
                "перезапустить",
                "отмена",
                "отменить",
                "обновить",
                "сбросить",
                "пока",
                "до свидания",
                "досвидания",
                "всего доброго"
            };

            return resetPhrases.Contains(query.ToLower());
        }
    }
}