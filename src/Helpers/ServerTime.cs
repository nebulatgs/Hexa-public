using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace Hexa.Helpers
{
    public static class ServerTime
    {
        public static TimeSpan GetServerTimeDifference()
        {
            return GetServerTime() - DateTime.Now;
        }
        public static void FetchServerTimeDifference(){
            ServerTimeDifference = GetServerTimeDifference();
        }
        public static TimeSpan ServerTimeDifference{ get; private set; }
        private static DateTime GetServerTime()
        {
            var client = new TcpClient("time.nist.gov", 13);
            using (var streamReader = new StreamReader(client.GetStream()))
            {
                var response = streamReader.ReadToEnd();
                var utcDateTimeString = response.Substring(7, 17);
                var localDateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                return localDateTime;
            }
        }
    }
}