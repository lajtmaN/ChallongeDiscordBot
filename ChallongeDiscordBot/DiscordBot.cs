using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace ChallongeDiscordBot
{
    public class DiscordBot
    {
        private const string BOT_PREFIX = "nnpbot";
        private DiscordClient bot;
        private string discordToken { get; }

        public DiscordBot(ChallongeDiscordBotConfig config)
        {
            discordToken = config.DiscordToken;
            bot = new DiscordClient();

            AttachEvents();
        }

        private void AttachEvents()
        {
            bot.Ready += (s, a) => Console.WriteLine("Discord Bot: '" + (s as DiscordClient)?.CurrentUser.Name + "' is ready");
            bot.MessageReceived += ClientOnMessageReceived;
        }

        public void StartDiscordBotThread()
        {
             /*new Task(() =>
             {
                 bot.ExecuteAndWait(async () =>
                 {
                     await bot.Connect(discordToken, TokenType.Bot);
                 });
             }).Start();*/
        }

        private static async void ClientOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            string userMessage;
            if (!MessageIsForMe(messageEventArgs, out userMessage))
                return;

            switch (userMessage)
            {
                case "hej":
                    await messageEventArgs.Channel.SendMessage($"Øhm.. hej {messageEventArgs.User.Name}?");
                    return;

                default:
                    await messageEventArgs.Channel.SendMessage("Undskyld, jeg forstod ikke hvad du ville?");
                    return;
            }
        }


        public void SendMessage(string rawMessage, string channel)
        {
            
        }

        public static bool MessageIsForMe(MessageEventArgs input, out string userMessage)
        {
            userMessage = String.Empty;

            if (input.Message.IsAuthor) //Did i send the message?
                return false;

            string receivedText = input.Message.Text.Trim();

            bool metionMe = input.Message.IsMentioningMe();
            bool startWithBotPrefix = receivedText.StartsWith(BOT_PREFIX);

            if (!metionMe && !startWithBotPrefix)
                return false;

            if (metionMe)
            {
                userMessage = Regex.Replace(input.Message.RawText, @"<@\d+> ", String.Empty);
                return true;
            }
            if (startWithBotPrefix)
            {
                userMessage = receivedText.Substring(BOT_PREFIX.Length).Trim().ToLowerInvariant();
                return true;
            }
            return false;
        }
    }
}
