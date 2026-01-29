using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Interactivity;
using Avalonia.Input;
using FluentAvalonia.UI.Navigation;
using FluentAvalonia.UI.Controls;
using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Shadler.UI;
using Shadler.Utilities;
using Shadler.DataStructure;

namespace Shadler.Views;

public partial class ContentViewer : UserControl
{
    int pageIndex = 0;
    List<List<string>> episodePages = new List<List<string>>();
    ShadlerPlayerContent playerContent = new ShadlerPlayerContent();

    public ContentViewer()
    {
        this.InitializeComponent();
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);
    }

    private void clearContents() {
        ContentTitle.Text = "";
        ContentYear.Text = "";
        (ContentThumbnail.Source as IDisposable)?.Dispose();
        ContentThumbnail.Source = null;
        ContentDescription.Text = "";
        EpisodeSelector.Children.Clear();
        episodePages.Clear();
    }

    private async void OnNavigatedTo(object sender, NavigationEventArgs args)
    {

        if (args.Parameter is ShadlerGeneralContent currentContent)
        {

            // clear and early load whatever we have
            clearContents();
            ContentTitle.Text = currentContent.Title;
            ContentYear.Text = currentContent.Year;
            ContentThumbnail.Source = currentContent.Thumbnail;
            ContentDescription.Text = "Loading descriptions...";

            using (HttpClient client = new HttpClient())
            {
                ShadlerHttp.SetDefaultHeader(client);

                HttpResponseMessage response = await client.GetAsync(currentContent.DetailUrl);

                if (!(response.IsSuccessStatusCode))
                {
                    ContentDetails.Children.Clear();
                    ContentDetails.Children.Add(new TextBlock
                    {
                        Text = "Failed to load content details.",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    });

                    return;
                }

                string responseData = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseData))
                {
                    string type = currentContent.ContentType == "Anime" ? "show" : "manga";
                    string detailKey = currentContent.ContentType == "Anime" ? "availableEpisodesDetail" : "availableChaptersDetail";
                    int count = 0;

                    JsonElement root = doc.RootElement;
                    string contentDescription = root
                    .GetProperty("data")
                    .GetProperty(type)
                    .GetProperty("description").ToString();

                    ContentDescription.Text = WebUtility.HtmlDecode(contentDescription).Replace("<br>", "");

                    JsonElement episodeStrings = root
                    .GetProperty("data")
                    .GetProperty(type)
                    .GetProperty(detailKey)
                    .GetProperty("sub");

                    int episodesLength = episodeStrings.GetArrayLength() - 1;
                    List<string> pageHelper = new List<string>();
                    List<string> availableEpisodes = new List<string>();

                    // prepare to create the episode/chapter button, we tried moving this to the top of the function but its not working?

                    foreach (JsonElement episode in episodeStrings.EnumerateArray())
                    {
                        string ep = episode.ToString();
                        availableEpisodes.Add(ep);
                        pageHelper.Add(ep);
                        Console.WriteLine($"episodeJson: {ep}");

                        if ((count != 0 && count % 15 == 0) || count == episodesLength)
                        {
                            Console.WriteLine("intresting");
                            episodePages.Add(new List<string>(pageHelper));
                            pageHelper.Clear();
                        }

                        count++;
                    }

                    foreach (string episodeString in episodePages[0])
                    {
                        Console.WriteLine($"episodeString: {episodeString}");
                        Grid episodeButton = ShadlerUIElement.CreateShadlerEpisodeButton(episodeString, PlayButton_Click);
                        EpisodeSelector.Children.Add(episodeButton);
                    }

                    playerContent.AvailableEpisodes = new List<string>(availableEpisodes);

                }
            }

            playerContent.ContentType = currentContent.ContentType;
            playerContent.Id = currentContent.Id;
            playerContent.Title = currentContent.Title;
            playerContent.Year = currentContent.Year;

        }
    }

    private void ContentPage_KeyPressed(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.Enter)
        {
            var pageBox = sender as TextBox;

            if (int.TryParse(pageBox?.Text, out pageIndex))
            {
                pageIndex -= 1;
                EpisodeSelector.Children.Clear();

                foreach (string episodeString in episodePages[pageIndex])
                {
                    EpisodeSelector.Children.Add(ShadlerUIElement.CreateShadlerEpisodeButton(episodeString, PlayButton_Click));
                }

                pageBox.Text = pageIndex.ToString();

            }
            else
            {
                return;
            }
        }
    }

    private void NextPageClick(object sender, RoutedEventArgs args)
    {
        pageIndex += 1;
        EpisodeSelector.Children.Clear();

        foreach (string episodeString in episodePages[pageIndex])
        {
            EpisodeSelector.Children.Add(ShadlerUIElement.CreateShadlerEpisodeButton(episodeString, PlayButton_Click));
        }
    }

    private void PreviousPageClick(object sender, RoutedEventArgs args)
    {
        pageIndex -= 1;
        EpisodeSelector.Children.Clear();

        foreach (string episodeString in episodePages[pageIndex])
        {
            EpisodeSelector.Children.Add(ShadlerUIElement.CreateShadlerEpisodeButton(episodeString, PlayButton_Click));
        }
    }

    private void PlayButton_Click(object sender, RoutedEventArgs args)
    {
        // ContentDetails.Children.Clear();

        var button = sender as Button;

        string episodeString = button.Tag.ToString();
        playerContent.Episode = episodeString;

        if (playerContent.ContentType == "Anime")
        {
            playerContent.StreamsUrl = Anime.GetStreamUrl(playerContent.Id, episodeString);
            Console.WriteLine($"[INFO] PlayButton_Click callback fired: contentType -> {playerContent.ContentType} ; StreamsUrl -> {playerContent.StreamsUrl}");
            //PlayerViewerFrame.Navigate(typeof(AnimePlayer), playerContent);
        }
        else if (playerContent.ContentType == "Manga")
        {
            playerContent.StreamsUrl = Manga.GetStreamUrl(playerContent.Id, episodeString);
            Console.WriteLine($"[INFO] PlayButton_Click callback fired: contentType -> {playerContent.ContentType} ; StreamsUrl -> {playerContent.StreamsUrl}");
            //PlayerViewerFrame.Navigate(typeof(MangaReader), playerContent);
        }
    }

    private void DownloadButton_Click(object sender, RoutedEventArgs args)
    {
        return; // TODO: implement downloader
    }

}
