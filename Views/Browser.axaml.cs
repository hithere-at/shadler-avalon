using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using Avalonia.Input;
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

    private void Search_Click(object sender, KeyEventArgs args)
    {
        if (sender is AutoCompleteBox queryBox && args.Key == Key.Enter)
        {
            currentQuery = queryBox.Text;
            SearchQuery(currentQuery);

        }
    }

    private void Search_TextChanged(object sender, TextChangedEventArgs args)
    {
        if (sender is AutoCompleteBox queryBox)
        {
            if (queryBox.Text == "")
            {
                ContentViewerFrame.BackStack.Clear();
                ContentViewerFrame.Content = null;
                ContentGrid.Children.Clear();
                shadlerContents.Clear();
            }

        }
    }

    private void ShowErrorMessage() {

        ContentGrid.Children.Add(new TextBlock
        {
            Text = "Uh oh. Something has gone really wrong. Please try again.",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 16
        });

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

            string queryUrl = currentContentType == "Anime" ? Anime.GetQueryUrl(query) : Manga.GetQueryUrl(query);

            HttpResponseMessage response = await client.GetAsync(queryUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseData);
                using (JsonDocument doc = JsonDocument.Parse(responseData))
                {
                    string type = currentContentType == "Anime" ? "shows" : "mangas";
                    int count = 0;

                    JsonElement root = doc.RootElement;

                    ContentViewerFrame.BackStack.Clear();
                    ContentViewerFrame.Content = null;
                    ContentGrid.Children.Clear();
                    shadlerContents.Clear();

                    JsonElement contentResults;

                    if (root.TryGetProperty("data", out contentResults))
                    {
                        contentResults = contentResults.GetProperty(type).GetProperty("edges");
                    } else
                    {
                        ShowErrorMessage();
                        return;
                    }

                    foreach (JsonElement content in contentResults.EnumerateArray())
                    {

                        string? id = content.GetProperty("_id").GetString();
                        string? title = content.GetProperty("name").GetString();

                        string? thumbnailUrl = content.GetProperty("thumbnail").GetString();
                        thumbnailUrl = !thumbnailUrl.StartsWith("https://")
                        ? "https://aln.youtube-anime.com/" + thumbnailUrl
                        : thumbnailUrl;
                        Console.WriteLine(thumbnailUrl);

                        string detailUrl = currentContentType == "Anime"
                        ? Anime.GetDetailUrl(content.GetProperty("_id").GetString())
                        : Manga.GetDetailUrl(content.GetProperty("_id").GetString());


                        JsonElement date = content.GetProperty("airedStart");
                        string year = date.TryGetProperty("year", out JsonElement yearElement)
                        ? yearElement.ToString()
                        : " ";

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
                ShowErrorMessage();
            }
        }
    }

    private void ContentMenu_Click(object sender, RoutedEventArgs args)
    {

        if (sender is MenuItem item)
        {
            if (item.Header?.ToString() == currentContentType)
            {
                return; // dont reload if the selected content type is the same as the current one

            }
            else
            {

                ContentViewerFrame.BackStack.Clear();
                ContentViewerFrame.Content = null;
                ContentGrid.Children.Clear();
                shadlerContents.Clear();

                switch (item.Header?.ToString())
                {
                    case "Anime":
                        currentContentType = "Anime";
                        ContentTypeDropDown.Content = "Anime";
                        break;

                    case "Manga":
                        currentContentType = "Manga";
                        ContentTypeDropDown.Content = "Manga";
                        break;

                    default:
                        return;
                }

            }

        }

    }

    private void SelectContent_Event(object sender, RoutedEventArgs args) {
        if (sender is Button shadlerContentButton) {

            Console.WriteLine("[INFO] SelectContent_Event callback fired");

            int index = int.Parse(shadlerContentButton.Tag.ToString());
            ContentViewerFrame.Navigate(typeof(ContentViewer), shadlerContents[index]);

            ContentViewerFrame.BackStack.Clear();
            ContentGrid.Children.Clear();

        }
    }

}
