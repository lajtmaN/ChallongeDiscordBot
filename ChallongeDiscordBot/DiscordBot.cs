using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            //TODO maybe use addCheck and make all the checking there
            string paramHold = "Holdnavn";
            string paramSeat = "Pladsnummer";
            Bot.GetService<CommandService>().CreateCommand("checkin")
                .Description("Meld dig som klar til at spille en turnering")
                .Parameter(paramHold)
                .Parameter(paramSeat)
                .Do(async e =>
                {
                    using (var db = new DiscordChallongeDatabase())
                    {
                        var tourn = await db.Tournaments.FirstOrDefaultAsync(x => x.ShortName == e.Channel.Name);
                        if (tourn == null)
                            await e.Channel.SendMessage("Du kan ikke checke ind i denne kanal, da den ikke er relateret til en turnering.");
                        else if (!tourn.CheckInOpen)
                            await e.Channel.SendMessage("Du kan ikke checke ind til den turneringen endnu. Vi åbner checkin kort tid før tilmeldingen åbner.");
                        else if (tourn.Participants.Any(x => x.DisplayName == e.GetArg(paramHold) && x.TournamentID == tourn.ID))
                        {
                            int seatNum;
                            if (int.TryParse(e.GetArg(paramSeat), out seatNum) && seatNum > 0 && seatNum < 400) //TODO check if the seat has been taken - and maybe add website username
                            {
                                var args = new UserCheckedInEventArgs
                                {
                                    TournamentID = tourn.ID,
                                    SeatNum = seatNum,
                                    TeamName = e.GetArg(paramHold),
                                    User = e.User
                                };
                                OnUserCheckedIn?.Invoke(this, args);
                                await e.Channel.SendMessage($"Super {e.User.NicknameMention} :ok_hand: Du har nu meldt din ankomst. Jeg skal nok give lyd når I skal spille :wink:");
                            }
                            else 
                                await e.Channel.SendMessage($"{e.GetArg(paramSeat)} er ikke et gyldigt pladsnr.");
                        }
                        else
                            await e.Channel.SendMessage($"Holdet '{e.GetArg(paramHold)}' ser ikke ud til at være tilmeldt {tourn.ShortName} turneringen. Tilmeld jer turneringen på http://www.nordicnetparty.dk/index.php?option=com_nnp-challonge og derefter checkin her igen.");
                    }
                });

            #endregion

            #region Info
            Bot.GetService<CommandService>().CreateCommand("info")
                .Description("Grundlæggende information om turneringen")
                .Do(async e =>
                {
                    using (var db = new DiscordChallongeDatabase())
                    {
                        var tourn = await db.Tournaments.FirstOrDefaultAsync(x => x.ShortName == e.Channel.Name);
                        if (tourn == null)
                            await e.Channel.SendMessage("Denne kanal er ikke relateret til en turnering.");
                        else
                        {
                            string message = $"{tourn.Name}.";
                            if (!tourn.CheckInOpen)
                                message += Environment.NewLine + "Det er ikke muligt at checke ind til denne turnering endnu. Vi åbner checkin kort tid før tilmeldingen åbner.";
                            message += Environment.NewLine + "Læs regler og se brackets her: " + tourn.FullLink;

                            await e.Channel.SendMessage(message);
                        }

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

            if (userMessage.StartsWith("hej"))
            {
                await messageEventArgs.Channel.SendMessage($"Hej {messageEventArgs.User.Name} :nerd:");
            }
            else if (userMessage.StartsWith("checkin") && userMessage.Count(x => x == ' ') < 2) //not all parameters supplied
            {
                await messageEventArgs.Channel.SendMessage($"For at checke ind, skal du skrive `{BOT_PREFIX}checkin <Holdnavn> <Pladsnummer>`");
            }
            else if (userMessage == "help" || userMessage.StartsWith("checkin") || userMessage == "info") { } //handled with commands
            else
            { 
                await messageEventArgs.Channel.SendMessage("Undskyld, jeg forstår ikke hvad du vil :confused:");
            }
        }

        public void SendMessageInDefaultChannel(string rawMessage)
        {
            SendMessage(rawMessage, DefaultChannel);
        }

        public async void SendMessage(string rawMessage, string channel)
        {
            channel = channel.ToLower();

            if (CreateChannel(channel) && Channels.ContainsKey(channel))
                SendMessage(rawMessage, Channels[channel]);
        }

        private async void SendMessage(string rawMessage, Channel channel)
        {
            await channel.SendMessage(rawMessage);
        }

        private object CreateChannelLock = new Object();
        public bool CreateChannel(string channelName)
        {
            channelName = channelName.ToLower();

            if (Channels.ContainsKey(channelName))
                return true;

            lock (CreateChannelLock)
            {
                Channel newChan = Server.CreateChannel(channelName, ChannelType.Text).Result;
                bool success = newChan != null;
                if (success)
                    Channels.Add(channelName, newChan);

                return success;
            }
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
