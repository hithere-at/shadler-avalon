using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Shadler.UI;
using Shadler.Utilities;
using Shadler.DataStructure;

namespace Shadler.Views;

public partial class Browser : UserControl
{
    string currentContentType = "Anime";
    string currentQuery = string.Empty;
    List<ShadlerGeneralContent> shadlerContents = new List<ShadlerGeneralContent>();

    public Browser()
    {
        this.InitializeComponent();

    }

    private void Search_TextChanged(object sender, TextChangedEventArgs args)
    {
        if (sender is AutoCompleteBox queryBox)
        {
            currentQuery = queryBox.Text;
            SearchQuery(currentQuery);
        }
    }

    private async void SearchQuery(string query)
    {

        if (string.IsNullOrEmpty(query))
        {
            ContentViewerFrame.BackStack.Clear();
            ContentViewerFrame.Content = null;
            ContentGrid.Children.Clear();
            shadlerContents.Clear();
            return;
        }

        using (HttpClient client = new HttpClient())
        {
            ShadlerHttp.SetDefaultHeader(client);

            string queryUrl;

            if (currentContentType == "Anime")
            {
                queryUrl = Anime.GetQueryUrl(query);
            }
            else
            {
                queryUrl = Manga.GetQueryUrl(query);
            }

            HttpResponseMessage response = await client.GetAsync(queryUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseData))
                {
                    string what = currentContentType == "Anime" ? "shows" : "mangas";
                    int count = 0;

                    JsonElement root = doc.RootElement;
                    JsonElement contentResults;

                    ContentViewerFrame.BackStack.Clear();
                    ContentViewerFrame.Content = null;
                    ContentGrid.Children.Clear();
                    shadlerContents.Clear();

                    if (root.TryGetProperty("data", out contentResults))
                    {
                        contentResults = contentResults.GetProperty(what).GetProperty("edges");
                    }
                    else
                    {
                        // silently fail if the data doesnt exist because API not API'ing
                        return;
                    }

                    foreach (JsonElement content in contentResults.EnumerateArray())
                    {

                        string? id = content.GetProperty("_id").GetString();
                        string? title = content.GetProperty("name").GetString();
                        string year;

                        string? thumbnailUrl = content.GetProperty("thumbnail").GetString();
                        thumbnailUrl = !thumbnailUrl.StartsWith("https://")
                        ? "https://aln.youtube-anime.com/" + thumbnailUrl
                        : thumbnailUrl;
                        Console.WriteLine(thumbnailUrl);

                        string detailUrl = currentContentType == "Anime"
                        ? Anime.GetDetailUrl(content.GetProperty("_id").GetString())
                        : Manga.GetDetailUrl(content.GetProperty("_id").GetString());


                        JsonElement date = content.GetProperty("airedStart");

                        if (date.TryGetProperty("year", out JsonElement yearElement))
                        {
                            year = yearElement.ToString();
                        }
                        else
                        {
                            year = " ";
                        }

                        Bitmap thumbnailImage;

                        try
                        {
                            var imageStream = await client.GetByteArrayAsync(thumbnailUrl);
                            thumbnailImage = new Bitmap(new MemoryStream(imageStream));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error has occured, ", e);
                            thumbnailImage = new Bitmap("Assets/icon.png");
                        }

                        Button currContentButton = ShadlerUIElement.CreateShadlerContent(title, year, thumbnailImage, count.ToString());
                        ShadlerGeneralContent currContent = new ShadlerGeneralContent(currentContentType, id, title, year, thumbnailImage, detailUrl);

                        currContentButton.Click += SelectContent_Event;
                        ContentGrid.Children.Add(currContentButton);
                        shadlerContents.Add(currContent);

                        count++;
                    }
                }
            }
            else
            {
                ContentGrid.Children.Add(new TextBlock
                {
                    Text = "Uh oh. Something has gone really wrong. Please try again.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16
                });
            }
        }
    }

    private void SelectContent_Event(object sender, RoutedEventArgs args) {
        if (sender is Button) {
            // TODO
            Console.WriteLine("[INFO] SelectContent_Event callback fired");
        }
    }

}
