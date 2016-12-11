using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Legacy;

namespace ChallongeDiscordBot
{
    public class DiscordBot
    {
        public const string BOT_PREFIX = "$";
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
        public event UserCheckedInEvent OnUserCheckedIn;

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
            Bot.UsingCommands(x =>
            {
                x.PrefixChar = '$';
                x.HelpMode = HelpMode.Public;
            });
            CreateCommands();
        }

        private void CreateCommands()
        {
            #region Check In
            
            Bot.GetService<CommandService>().CreateCommand("checkin")
                .Description("Meld dig som klar til at spille en turnering")
                .Parameter("Holdnavn")
                .Parameter("Pladsnummer")
                .Do(async e =>
                {
                    using (var db = new DiscordChallongeDatabase())
                    {
                        var tourn = db.Tournaments.FirstOrDefault(x => x.ShortName == e.Channel.Name);
                        if (tourn == null)
                            await e.Channel.SendMessage("Du kan ikke checke ind i denne kanal, da den ikke er relateret til en turnering.");
                        else if (tourn.Participants.Any(x => x.DisplayName == e.GetArg("Holdnavn") && x.TournamentID == tourn.ID))
                        {
                            int seatNum;
                            if (int.TryParse(e.GetArg("Pladsnummer"), out seatNum) && seatNum > 0 && seatNum < 400) //TODO check if the seat has been taken - and maybe add website username
                            {
                                var args = new UserCheckedInEventArgs
                                {
                                    TournamentID = tourn.ID,
                                    SeatNum = seatNum,
                                    TeamName = e.GetArg("Holdnavn"),
                                    User = e.User
                                };
                                OnUserCheckedIn?.Invoke(this, args);
                                await e.Channel.SendMessage($"Super {e.User.NicknameMention}! Du har nu meldt din ankomst. Jeg skal nok give lyd når I skal spille :wink:");
                            }
                            else
                                await e.Channel.SendMessage($"{e.GetArg("Pladsnr.")} er ikke et gyldigt pladsnr.");
                        }
                        else
                            await e.Channel.SendMessage($"Holdet '{e.GetArg("Holdnavn")}' ser ikke ud til at være tilmeldt {tourn.ShortName} turneringen");
                    }
                });

            #endregion
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
                    await messageEventArgs.Channel.SendMessage($"Hej {messageEventArgs.User.Name} :nerd:");
                    return;
                case "checkin":
                    await messageEventArgs.Channel.SendMessage($"For at checke ind, skal du skrive `{BOT_PREFIX}checkin <Holdnavn> <Pladsnummer>`");
                    return;
                default:
                    if (userMessage.StartsWith("checkin")) return; //hack to stop it writing when running commands
                    if (userMessage.StartsWith("help")) return; //hack to stop it writing when running commands
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

    public delegate void UserCheckedInEvent(object sender, UserCheckedInEventArgs args);

    public class UserCheckedInEventArgs
    {
        public int TournamentID { get; set; }
        public User User { get; set; }
        public int SeatNum { get; set; }
        public string TeamName { get; set; }
    }
}
