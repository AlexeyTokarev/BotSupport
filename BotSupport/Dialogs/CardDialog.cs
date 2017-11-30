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
        public static void PlatformCard(IDialogContext context, Activity activity, string checkParametrs)
        {
            try
            {
                var replyToConversation = activity.CreateReply();
                replyToConversation.Attachments = new List<Attachment>();

                var cardButton = new List<CardAction>();
                //---------------------------
                var cardButtonFB1 = new List<CardAction>();
                var cardButtonFB2 = new List<CardAction>();
                //---------------------------
                var card1 = new CardAction()
                {
                    Value = "44-ФЗ",
                    Title = "44-ФЗ"
                };
                var card2 = new CardAction()
                {
                    Value = "223-ФЗ",
                    Title = "223-ФЗ"
                };                
                var card3 = new CardAction()
                {
                    Value = "615-ПП РФ",
                    Title = "615-ПП РФ"
                };
                var card4 = new CardAction()
                {
                    Value = "Имущественные торги",
                    Title = "Имущественные торги"
                };
                var card5 = new CardAction()
                {
                    Value = "РТС-Маркет",
                    Title = "РТС-Маркет"
                };

                cardButton.Add(card1);
                cardButton.Add(card2);
                cardButton.Add(card3);
                cardButton.Add(card4);
                cardButton.Add(card5);
                //---------------------------
                if (activity.ChannelId == "facebook")
                {
                    cardButtonFB1.Add(card1);
                    cardButtonFB1.Add(card2);
                    cardButtonFB1.Add(card3);
                    cardButtonFB2.Add(card4);
                    cardButtonFB2.Add(card5);

                    var heroFB1 = new HeroCard()
                    {
                        Buttons = cardButtonFB1,
                        Text = checkParametrs
                    };
                    var heroFB2 = new HeroCard()
                    {
                        Buttons = cardButtonFB2,
                        Text = "Вас также могут заинтересовать:"
                    };
                    var attachFB1 = heroFB1.ToAttachment();
                    var attachFB2 = heroFB2.ToAttachment();

                    replyToConversation.Attachments.Add(attachFB1);
                    //context.PostAsync(replyToConversation);
                    replyToConversation.Attachments.Add(attachFB2);
                    context.PostAsync(replyToConversation);
                    return;
                }
                //---------------------------
                var hero = new HeroCard()
                {
                    Buttons = cardButton,
                    Text = checkParametrs
                };
                var attach = hero.ToAttachment();

                replyToConversation.Attachments.Add(attach);
                context.PostAsync(replyToConversation);
            }
            catch (Exception e)
            {
                context.PostAsync(e.Message + " Error code 1.");
            }
        }

        /// <summary>
        /// Метод, определяющий роль пользователя по площадке 223-ФЗ, 44-ФЗ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="checkParametrs"></param>
        public static void RoleCard(IDialogContext context, Activity activity, string checkParametrs)
        {
            var replyToConversation = activity.CreateReply();
            replyToConversation.Attachments = new List<Attachment>();

            var cardButton = new List<CardAction>();
            var card1 = new CardAction()
            {
                Value = "Заказчик",
                Title = "Заказчик"
            };
            var card2 = new CardAction()
            {
                Value = "Участник",
                Title = "Участник"
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
            context.PostAsync(replyToConversation);
        }

        /// <summary>
        /// Метод, определяющий роль пользователя по площадке 615-ПП РФ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="checkParametrs"></param>
        public static void RoleCard615(IDialogContext context, Activity activity, string checkParametrs)
        {
            var replyToConversation = activity.CreateReply();
            replyToConversation.Attachments = new List<Attachment>();

            var cardButton = new List<CardAction>();
            var card1 = new CardAction()
            {
                Value = "ОВР",
                Title = "ОВР"
            };
            var card2 = new CardAction()
            {
                Value = "Заказчик",
                Title = "Заказчик"
            };
            var card3 = new CardAction()
            {
                Value = "Участник",
                Title = "Участник"
            };

            cardButton.Add(card1);
            cardButton.Add(card2);
            cardButton.Add(card3);

            var hero = new HeroCard()
            {
                Buttons = cardButton,
                Text = checkParametrs
            };

            var attach = hero.ToAttachment();
            if (attach == null) throw new ArgumentNullException(nameof(attach));

            replyToConversation.Attachments.Add(attach);
            context.PostAsync(replyToConversation);
        }

        /// <summary>
        /// Метод, определяющий роль пользователя по площадке "Имущество"
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="checkParametrs"></param>
        public static void RoleCardImuchestvo(IDialogContext context, Activity activity, string checkParametrs)
        {
            var replyToConversation = activity.CreateReply();
            replyToConversation.Attachments = new List<Attachment>();

            var cardButton = new List<CardAction>();
            var card1 = new CardAction()
            {
                Value = "Продавец/Арендодатель",
                Title = "Продавец/Арендодатель"
            };
            var card2 = new CardAction()
            {
                Value = "Претендент/Арендатор",
                Title = "Претендент/Арендатор"
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
            context.PostAsync(replyToConversation);
        }
        /// <summary>
        /// Метод, определяющий роль пользователя по площадке "РТС-Маркет"
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="checkParametrs"></param>
        public static void RoleCardRTS(IDialogContext context, Activity activity, string checkParametrs)
        {
            var replyToConversation = activity.CreateReply();
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
            context.PostAsync(replyToConversation);
        }

        public static void SatisfyingAnswer(IDialogContext context, Activity activity)
        {
            var replyToConversation = activity.CreateReply();
            replyToConversation.Attachments = new List<Attachment>();

            var cardButton = new List<CardAction>();
            var card1 = new CardAction()
            {
                Value = "Да",
                Title = "Да"
            };
            var card2 = new CardAction()
            {
                Value = "Нет",
                Title = "Нет"
            };
            cardButton.Add(card1);
            cardButton.Add(card2);

            var hero = new HeroCard()
            {
                Buttons = cardButton,
                Text = "Удовлетворил ли Вас ответ?"
            };

            var attach = hero.ToAttachment();
            if (attach == null) throw new ArgumentNullException(nameof(attach));

            replyToConversation.Attachments.Add(attach);
            context.PostAsync(replyToConversation);
        }
    }
}