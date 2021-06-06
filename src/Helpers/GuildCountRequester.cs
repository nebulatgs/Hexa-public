// using System;
// using System.IO;
// using System.Net;
// using System.Text.Json;

// namespace Hexa.Helpers
// {
//     public static class GuildCountRequester
//     {
//         public static GuildCountJson Request(ulong id)
//         {
//             var url = $"https://discord.com/api/v9/oauth2/authorize?client_id={id}&scope=bot";

//             var httpRequest = (HttpWebRequest)WebRequest.Create(url);
//             httpRequest.Headers["authorization"] = "NjMwMDI5MjYxNDMyMDI5MTg0.YLqU-g.SaagN7KzBlL31hw5i9dTSxuAwek";
//             httpRequest.ContentType = "application/json";


//             var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
//             using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
//             {
//                 var result = streamReader.ReadToEnd();
//                 return JsonSerializer.Deserialize<GuildCountJson>(result);
//                 //  JsonConvert.DeserializeObject<GuildCountJson>(result);
//             }
//         }
//     }
// }