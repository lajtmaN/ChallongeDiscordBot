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
        private ChallongeDataLoader loader { get; }

        public ChallongeClient(ChallongeDiscordBotConfig config)
            : this(new ChallongeDataLoader(config.ApiKey, config.Subdomain, config.CreatedAfter)) { }

        public ChallongeClient(ChallongeDataLoader pLoader)
        {
            loader = pLoader;
        }

        public event OnNewMatchStartedEvent OnNewMatchStarted;


        public void StartLoaderThread()
        {
            test1();
            //new Task(() =>
            //{
            //    new Timer(e => test1(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            //}).Start();
        }

        private async void test1()
        {
            IStartedTournament tournament = await loader.LoadTournament("csgo");
            List<IOpenMatch> openMatches = await tournament.GetAllActiveMatches();

            foreach (IOpenMatch match in openMatches)
                OnNewMatchStarted?.Invoke(this, new OnNewMatchStartedArgs(match, tournament));
        }
    }

    public delegate void OnNewMatchStartedEvent(object sender, OnNewMatchStartedArgs args);

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
