using System;
using System.Collections.Generic;
using System.Data.Entity;
using ChallongeDiscordBot.Entities;

namespace ChallongeDiscordBot
{
    
    public class DiscordChallongeDatabase : DbContext
    {
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DiscordChallongeDatabase>()); //this will delete the database on changes
            base.OnModelCreating(modelBuilder);
        }
    }
}
