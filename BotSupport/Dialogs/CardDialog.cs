using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotSupport.Dialogs
{
    public class CardDialog
    {
        /// <summary>
        /// Метод, определяющий площадку, на которой собирается работать пользователь
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="checkParametrs"></param>
        public static async void PlatformCard(IDialogContext context, Activity activity, string checkParametrs)
        {
            var replyToConversation1 = activity.CreateReply();//(Activity)context.MakeMessage();
            replyToConversation1.Attachments = new List<Attachment>();

            var replyToConversation2 = activity.CreateReply();//(Activity)context.MakeMessage();
            replyToConversation2.Attachments = new List<Attachment>();
            var cardButton1 = new List<CardAction>();
            var cardButton2 = new List<CardAction>();
            var card1 = new CardAction()
            {
                Value = "223-ФЗ",
                Title = "223-ФЗ"
            };
            var card2 = new CardAction()
            {
                Value = "44-ФЗ",
                Title = "44-ФЗ"
            };
            var card3 = new CardAction()
            {
                Value = "615-ФЗ",
                Title = "615-ФЗ"
            };
            var card4 = new CardAction()
            {
                Value = "Имущество",
                Title = "Имущество"
            };
            var card5 = new CardAction()
            {
                Value = "РТС-Маркет",
                Title = "РТС-Маркет"
            };

            cardButton1.Add(card1);
            cardButton1.Add(card2);
            cardButton1.Add(card3);
            cardButton2.Add(card4);
            cardButton2.Add(card5);

            var hero1 = new HeroCard()
            {
                Buttons = cardButton1,
                Text = checkParametrs
            };
            var hero2 = new HeroCard()
            {
                Buttons = cardButton2
            };
            var attach1 = hero1.ToAttachment();
            var attach2 = hero2.ToAttachment();
            //if (attach == null) throw new ArgumentNullException(nameof(attach));

            replyToConversation1.Attachments.Add(attach1);
            replyToConversation2.Attachments.Add(attach2);
            await context.PostAsync(replyToConversation1);
            await context.PostAsync(replyToConversation2);
        }

        /// <summary>
        /// Метод, определяющий роль пользователя по площадке 223-ФЗ, 44-ФЗ, 615-ФЗ, РТС-Маркет
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="checkParametrs"></param>
        public static async void RoleCard(IDialogContext context, Activity activity, string checkParametrs)
        {
            var replyToConversation = activity.CreateReply();//(Activity)context.MakeMessage();
            replyToConversation.Attachments = new List<Attachment>();

            var cardButton = new List<CardAction>();
            var card1 = new CardAction()
            {
                Value = "Заказчик",
                Title = "Заказчик"
            };
            var card2 = new CardAction()
            {
                Value = "Поставщик",
                Title = "Поставщик"
            };
            cardButton.Add(card1);
            cardButton.Add(card2);

            var hero = new HeroCard()
            {
                Buttons = cardButton,
                Text = checkParametrs
            };

            var attach = hero.ToAttachment();
            if (attach == null) throw new ArgumentNullException(nameof(attach));

            replyToConversation.Attachments.Add(attach);
            await context.PostAsync(replyToConversation);
        }

        /// <summary>
        /// Метод, определяющий роль пользователя по площадке "Имущество"
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="checkParametrs"></param>
        public static async void RoleCardImuchestvo(IDialogContext context, Activity activity, string checkParametrs)
        {
            var replyToConversation = activity.CreateReply();
            replyToConversation.Attachments = new List<Attachment>();

            var cardButton = new List<CardAction>();
            var card1 = new CardAction()
            {
                Value = "Продавец",
                Title = "Продавец"
            };
            var card2 = new CardAction()
            {
                Value = "Покупатель",
                Title = "Покупатель"
            };
            cardButton.Add(card1);
            cardButton.Add(card2);

            var hero = new HeroCard()
            {
                Buttons = cardButton,
                Text = checkParametrs
            };

            var attach = hero.ToAttachment();
            if (attach == null) throw new ArgumentNullException(nameof(attach));

            replyToConversation.Attachments.Add(attach);
            await context.PostAsync(replyToConversation);
        }
    }
}