// using System.Collections.Generic;
// using System.Text.Json.Serialization;

// namespace Hexa.Helpers
// {
//     public class Application
//     {
//         [JsonPropertyName("id")]
//         public string Id { get; set; }

//         [JsonPropertyName("name")]
//         public string Name { get; set; }

//         [JsonPropertyName("icon")]
//         public string Icon { get; set; }

//         [JsonPropertyName("description")]
//         public string Description { get; set; }

//         [JsonPropertyName("summary")]
//         public string Summary { get; set; }

//         [JsonPropertyName("hook")]
//         public bool Hook { get; set; }

//         [JsonPropertyName("bot_public")]
//         public bool BotPublic { get; set; }

//         [JsonPropertyName("bot_require_code_grant")]
//         public bool BotRequireCodeGrant { get; set; }

//         [JsonPropertyName("verify_key")]
//         public string VerifyKey { get; set; }

//         [JsonPropertyName("flags")]
//         public int Flags { get; set; }
//     }

//     public class User
//     {
//         [JsonPropertyName("id")]
//         public string Id { get; set; }

//         [JsonPropertyName("username")]
//         public string Username { get; set; }

//         [JsonPropertyName("avatar")]
//         public string Avatar { get; set; }

//         [JsonPropertyName("discriminator")]
//         public string Discriminator { get; set; }

//         [JsonPropertyName("public_flags")]
//         public int PublicFlags { get; set; }
//     }

//     public class Bot
//     {
//         [JsonPropertyName("id")]
//         public string Id { get; set; }

//         [JsonPropertyName("username")]
//         public string Username { get; set; }

//         [JsonPropertyName("avatar")]
//         public string Avatar { get; set; }

//         [JsonPropertyName("discriminator")]
//         public string Discriminator { get; set; }

//         [JsonPropertyName("public_flags")]
//         public int PublicFlags { get; set; }

//         [JsonPropertyName("bot")]
//         public bool _Bot { get; set; }

//         [JsonPropertyName("approximate_guild_count")]
//         public int ApproximateGuildCount { get; set; }
//     }

//     public class Guild
//     {
//         [JsonPropertyName("id")]
//         public string Id { get; set; }

//         [JsonPropertyName("name")]
//         public string Name { get; set; }

//         [JsonPropertyName("icon")]
//         public string Icon { get; set; }

//         [JsonPropertyName("permissions")]
//         public string Permissions { get; set; }
//     }
//     public class GuildCountJson
//     {
//         [JsonPropertyName("application")]
//         public Application Application { get; set; }

//         [JsonPropertyName("user")]
//         public User User { get; set; }

//         [JsonPropertyName("authorized")]
//         public bool Authorized { get; set; }

//         [JsonPropertyName("bot")]
//         public Bot Bot { get; set; }

//         [JsonPropertyName("guilds")]
//         public List<Guild> Guilds { get; set; }
//     }
// }