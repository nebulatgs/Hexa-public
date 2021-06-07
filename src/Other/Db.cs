using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Postgrest.Attributes;
using Supabase;

namespace Hexa.Database
{
    [Table("GuildSettings")]
    public class GuildSetting : SupabaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("GuildId")]
        public ulong GuildId { get; set; }

        [Column("SettingID")]
        public int SettingID { get; set; }

        [Column("Value")]
        public string Value { get; set; }

        [JsonIgnore]
        public SettingDef Setting { get; set; }
    }

    [Table("GuildSettings")]
    public class GuildSettingAddable : SupabaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }

        [Column("GuildId")]
        public ulong GuildId { get; set; }

        [Column("SettingID")]
        public int SettingID { get; set; }

        [Column("Value")]
        public string Value { get; set; }

        public GuildSetting ToGuildSetting()
        {
            return new GuildSetting()
            {
                id = this.id,
                GuildId = this.GuildId,
                SettingID = this.SettingID,
                Value = this.Value
            };
        }
    }

    [Table("Settings")]
    public class SettingDef : SupabaseModel
    {
        [PrimaryKey("SettingID", false)]
        public int SettingID { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Type")]
        public string Type { get; set; }

        [Column("Aliases")]
        public List<string> Aliases { get; set; }

        [Column("Default")]
        public string Default { get; set; }

        // public override bool Equals(object obj)
        // {
        //     return obj is GuildSetting message &&
        //             GuildId == message.GuildId;
        // }

        // public override int GetHashCode()
        // {
        //     return HashCode.Combine(GuildId);
        // }
    }

    [Table("PastUserStates")]
    public class PastUserState : SupabaseModel
    {
        [PrimaryKey("PastUserStateId", false)]
        public int PastUserStateId { get; set; }

        [Column("UserId")]
        public ulong UserId { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("Discriminator")]
        public int Discriminator { get; set; }

        [Column("Flags")]
        public int Flags { get; set; }

        [Column("AvatarUrl")]
        public string AvatarUrl { get; set; }

        // public override bool Equals(object obj)
        // {
        //     return obj is GuildSetting message &&
        //             GuildId == message.GuildId;
        // }

        // public override int GetHashCode()
        // {
        //     return HashCode.Combine(GuildId);
        // }
    }
}