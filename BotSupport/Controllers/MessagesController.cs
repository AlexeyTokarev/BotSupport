﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Rest;

namespace BotSupport
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Microsoft.Bot.Connector.Activity activity)
        {
            if  (activity.From.Id == null) activity.From.Id = "qweasd";

            switch (activity.Type)
            {
                case ActivityTypes.ConversationUpdate:
                    {
                        await Conversation.SendAsync(activity, () => new Dialogs.StartConversation());
                        break;
                    }
                case ActivityTypes.Message:
                    {
                        await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                        break;
                    }
                default: HandleSystemMessage(activity); break;
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
            
            // -------Original code-------
            //if (activity.Type == ActivityTypes.Message)
            //{
            //    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            //}
            //else
            //{
            //    HandleSystemMessage(activity);
            //}

            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //return response;
        }

        private Microsoft.Bot.Connector.Activity HandleSystemMessage(Microsoft.Bot.Connector.Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}