using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace Shadler.DataStructure
{
    public struct ShadlerGeneralContent
    {
        public string ContentType { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public Bitmap Thumbnail { get; set; }
        public string DetailUrl { get; set; }
        public ShadlerGeneralContent(string contentType, string id, string title, string year, Bitmap thumbnail, string detailUrl)
        {
            ContentType = contentType;
            Id = id;
            Title = title;
            Year = year;
            Thumbnail = thumbnail;
            DetailUrl = detailUrl;
        }
    };

    public struct ShadlerPlayerContent
    {
        public string ContentType { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Episode { get; set; }
        public List<string> AvailableEpisodes { get; set; }
        public string StreamsUrl { get; set; }

        public ShadlerPlayerContent()
        {
            ContentType = string.Empty;
            Id = string.Empty;
            Title = string.Empty;
            Year = string.Empty;
            Episode = string.Empty;
            AvailableEpisodes = new List<string>();
            StreamsUrl = string.Empty;
        }
        public ShadlerPlayerContent(string contentType, string id, string title, string year, string episode, string contentUrl)
        {
            ContentType = contentType;
            Id = id;
            Title = title;
            Year = year;
            Episode = episode;
            StreamsUrl = contentUrl;
        }
    }
   
}
