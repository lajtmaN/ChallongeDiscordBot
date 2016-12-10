using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Main;

namespace ChallongeDiscordBot
{
    public class ChallongeClient
    {
        private const int UPDATE_RATE_SECONDS = 30;

        private ChallongeDataLoader Loader { get; }
        private Timer LoaderTimer { get; set; }

        public ChallongeClient(ChallongeDiscordBotConfig config)
            : this(new ChallongeDataLoader(config.ApiKey, config.Subdomain, config.CreatedAfter)) { }

        public ChallongeClient(ChallongeDataLoader pLoader)
        {
            Loader = pLoader;
        }

        public event OnNewMatchStartedEvent OnNewMatchStarted;
        public event OnTournamentStartedEvent OnTournamentStarted;
        
        public void StartLoaderThread()
        {
            LoaderTimer = new Timer(e => test1(), null, TimeSpan.Zero, TimeSpan.FromSeconds(UPDATE_RATE_SECONDS));
        }

        private async void test1()
        {
            IStartedTournament tournament = await Loader.LoadTournament("csgo");
            List<IOpenMatch> openMatches = await tournament.GetAllActiveMatches();

            foreach (IOpenMatch match in openMatches)
                OnNewMatchStarted?.Invoke(this, new OnNewMatchStartedArgs(match, tournament));
        }
    }
    
    public delegate void OnNewMatchStartedEvent(object sender, OnNewMatchStartedArgs args);
    public delegate void OnTournamentStartedEvent(object sender, OnTournamentStartedEventArgs args);

    public class OnTournamentStartedEventArgs
    {
        public OnTournamentStartedEventArgs(IStartedTournament tournament)
        {
            Tournament = tournament;
        }
        public IStartedTournament Tournament { get; }
    }

    public class OnNewMatchStartedArgs : EventArgs
    {
        public OnNewMatchStartedArgs(IOpenMatch match, IStartedTournament tournament)
        {
            Match = match;
            Tournament = tournament;
        }
        public IOpenMatch Match { get; }
        public IStartedTournament Tournament { get; }
    }
}
