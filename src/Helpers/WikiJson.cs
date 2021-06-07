using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hexa.Helpers
{
    // public class Continue
    // {
    //     [JsonPropertyName("sroffset")]
    //     public int Sroffset { get; set; }

    //     [JsonPropertyName("continue")]
    //     public string Continues { get; set; }
    // }

    // public class Searchinfo
    // {
    //     [JsonPropertyName("totalhits")]
    //     public int Totalhits { get; set; }
    // }

    // public class Search
    // {
    //     [JsonPropertyName("ns")]
    //     public int Ns { get; set; }

    //     [JsonPropertyName("title")]
    //     public string Title { get; set; }

    //     [JsonPropertyName("pageid")]
    //     public int Pageid { get; set; }

    //     [JsonPropertyName("size")]
    //     public int Size { get; set; }

    //     [JsonPropertyName("wordcount")]
    //     public int Wordcount { get; set; }

    //     [JsonPropertyName("snippet")]
    //     public string Snippet { get; set; }

    //     [JsonPropertyName("timestamp")]
    //     public DateTime Timestamp { get; set; }
    // }

    // public class Query
    // {
    //     [JsonPropertyName("searchinfo")]
    //     public Searchinfo Searchinfo { get; set; }

    //     [JsonPropertyName("search")]
    //     public List<Search> Search { get; set; }
    // }

    // public class WikiResponse
    // {
    //     [JsonPropertyName("batchcomplete")]
    //     public string Batchcomplete { get; set; }

    //     [JsonPropertyName("continue")]
    //     public Continue Continue { get; set; }

    //     [JsonPropertyName("query")]
    //     public Query Query { get; set; }
    // }
    public class WikiPage
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}