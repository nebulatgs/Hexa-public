using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DbSet<GuildSetting> GuildSettings { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Program.DBSTRING);
        }
    }

    // public class GuildSetting
    // {
    //     [Key]
    //     public int SettingID { get; set; }
    //     public ulong GuildId { get; set; }
    //     public int SettingType { get; set; }
    //     public string Value { get; set; }
    //     public override string ToString()
    //     {
    //         return $"{SettingID}: {GuildId} - {SettingType}:{Value}";
    //     }
    //     public string GetIdentifier()
    //     {
    //         return ToString();
    //     }
    //     // public override bool Equals(object obj)
    //     // {
    //     //     return obj is GuildSetting message &&
    //     //             GuildId == message.GuildId;
    //     // }

    //     // public override int GetHashCode()
    //     // {
    //     //     return HashCode.Combine(GuildId);
    //     // }
    // }

    public class GuildSetting
    {
        [Key]
        public int id { get; set; }
        public ulong GuildId { get; set; }
        public int SettingID { get; set; }
        public string Value { get; set; }
        [ForeignKey("SettingID")]
        public Setting Setting { get; set; }
        public override string ToString()
        {
            return $"{id}: {GuildId} - {Setting}:{Value}";
        }
        public string GetIdentifier()
        {
            return ToString();
        }
    }
    public class Setting
    {
        [Key]
        public int SettingID { get; set; }
        public string Name { get; set; }
        public string Description { get; set;}
        public string Type { get; set; }
        public List<string> Aliases { get; set;}
        public string Default { get; set; }
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
        public bool IsBot { get; set; }
        public override string ToString()
        {
            return $"{Username}#{Discriminator}";
        }
        public string GetIdentifier()
        {
            return $"{UserId}; {Username}#{Discriminator}: {Flags}, {AvatarUrl}, {IsBot}";
        }
    }
}