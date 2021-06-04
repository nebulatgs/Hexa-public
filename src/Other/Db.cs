using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Hexa.Database
{
    public class HexaContext : Microsoft.EntityFrameworkCore.DbContext
    {
        // public HexaContext(){}   ;// : base(new Func<DbConnection>(() =>
        // {
        //     var conn = DbProviderFactories.GetFactory("Npgsql").CreateConnection();
        //     conn.ConnectionString = Environment.GetEnvironmentVariable("DBSTRING");
        //     return conn;
        // })(), true){}
        public DbSet<PastUserState> PastUserStates { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DBSTRING"));
        }
    }

    public class PastUserState
    {
        [Key]
        public long PastUserStateId { get; set; }
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public int Discriminator { get; set; }
        public int Flags { get; set; }
        public string AvatarUrl { get; set; }
        public override string ToString()
        {
            return $"{Username}#{Discriminator}";
        }
    }
}