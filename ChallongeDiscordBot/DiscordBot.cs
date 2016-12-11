using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Legacy;

namespace ChallongeDiscordBot
{
    public class DiscordBot
    {
        private const string BOT_PREFIX = "nnpbot";
        private DiscordClient Bot { get; }
        private Profile BotProfile { get; set; } 

        private Dictionary<string, Channel> Channels;
        private Channel DefaultChannel => Server.DefaultChannel;
        private Server Server { get; set; }

        private string discordToken { get; }

        public DiscordBot(ChallongeDiscordBotConfig config)
        {
            Channels = new Dictionary<string, Channel>();
            discordToken = config.DiscordToken;
            Bot = new DiscordClient();

            AttachEvents();
        }

        public event EventHandler OnDiscordBotReady;

        private void BotOnReady(object sender, EventArgs eventArgs)
        {
            BotProfile = (sender as DiscordClient)?.CurrentUser;
            Console.WriteLine("Discord Bot: '" + BotProfile?.Name + "' is ready");
            OnDiscordBotReady?.Invoke(this, EventArgs.Empty);
        }

        private void AttachEvents()
        {
            Bot.Ready += BotOnReady;
            Bot.ServerAvailable += BotOnJoinedServer;
            Bot.JoinedServer += BotOnJoinedServer;
            Bot.MessageReceived += ClientOnMessageReceived;
        }

        private void BotOnJoinedServer(object sender, ServerEventArgs newServer)
        {
            Console.WriteLine($"{BotProfile?.Name} joined {newServer.Server.Name}");
            Server = newServer.Server; 

            foreach (Channel chan in newServer.Server.TextChannels)
            {
                Channels.Add(chan.Name.ToLower(), chan);
                Console.WriteLine($"Channel {chan.Name} registered");
            }
            
            //Unsubscribe for event. Only ONE server!
            Bot.ServerAvailable -= BotOnJoinedServer;
            Bot.JoinedServer -= BotOnJoinedServer;
        }
        
        public void StartDiscordBotThread()
        {
             new Task(() =>
             {
                 Bot.ExecuteAndWait(async () =>
                 {
                     await Bot.Connect(discordToken, TokenType.Bot);
                 });
             }).Start();
        }

        private static async void ClientOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            string userMessage;
            if (!MessageIsForMe(messageEventArgs, out userMessage))
                return;

            switch (userMessage)
            {
                case "hej":
                    await messageEventArgs.Channel.SendMessage($"Øhm.. Hej {messageEventArgs.User.Name}?");
                    return;

                default:
                    await messageEventArgs.Channel.SendMessage("Undskyld, jeg forstår ikke hvad du vil :confused:");
                    return;
            }
        }

        public void SendMessageInDefaultChannel(string rawMessage)
        {
            SendMessage(rawMessage, DefaultChannel);
        }

        public void SendMessage(string rawMessage, string channel)
        {
            channel = channel.ToLower();

            if (Channels.ContainsKey(channel))
                SendMessage(rawMessage, Channels[channel]);
        }

        private async void SendMessage(string rawMessage, Channel channel)
        {
            await channel.SendMessage(rawMessage);
        }

        public async Task<bool> CreateChannel(string channelName)
        {
            channelName = channelName.ToLower();

            if (Channels.ContainsKey(channelName))
                return true;

            return await Server.CreateChannel(channelName, ChannelType.Text) != null;
        }

        private static bool MessageIsForMe(MessageEventArgs input, out string userMessage)
        {
            userMessage = string.Empty;

            if (input.Message.IsAuthor) //Did i send the message?
                return false;

            string receivedText = input.Message.Text.Trim();

            bool privateMessage = input.Channel.IsPrivate;
            bool metionMe = input.Message.IsMentioningMe();
            bool startWithBotPrefix = receivedText.StartsWith(BOT_PREFIX);

            if (!metionMe && !startWithBotPrefix && !privateMessage)
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
            if (privateMessage)
            {
                userMessage = receivedText;
                return true;
            }
            return false;
        }
    }
}
