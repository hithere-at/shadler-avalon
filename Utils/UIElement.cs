using System;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Layout;

namespace Shadler.UI
{
    public static class ShadlerUIElement
    {
        public static Button CreateShadlerContent(string title, string year, Bitmap image, string tag)
        {
            // main items
            // content banner
            Image contentImage = new Image
            {
                Source = image,
                Stretch = Stretch.UniformToFill,
            };

            // border
            Border contentImageContainer = new Border
            {
                Width = 204,
                Height = 240,
                ClipToBounds = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                CornerRadius = new CornerRadius(8, 8, 0, 0),
                Margin = new Thickness(0, -10, 0, 9),
            };

            contentImageContainer.Child = contentImage;

            // content title
            // very stupid hack because manga author cant make a proper title lmaooooooo
            // seriously, what the heck is ryoushin no shakkin wo katagawari shite morau jouken bla bla :sob: (good manga btw, very sweet)
            // 
            string shortTitle = (title.Length > 26) ? title.Substring(0, 23) + "..." : title;

            TextBlock contentTitle = new TextBlock
            {
                Text = shortTitle,
                TextWrapping = TextWrapping.NoWrap,
                FontSize = 15,
                Margin = new Thickness(4, 0, 4, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // content release year
            TextBlock contentYear = new TextBlock
            {
                Text = year,
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Center,
                Opacity = 0.6
            };

            StackPanel baseLayout = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            baseLayout.Children.Add(contentImageContainer);
            baseLayout.Children.Add(contentTitle);
            baseLayout.Children.Add(contentYear);

            MenuFlyout ctxMenu = new MenuFlyout
            {
                Placement = PlacementMode.Bottom
            };

            MenuItem openContentOptions = new MenuItem
            {
                Icon = new FluentAvalonia.UI.Controls.SymbolIcon { Symbol = FluentAvalonia.UI.Controls.Symbol.Favorite },
                Header = "Add to favorites"
            };

            ctxMenu.Items.Add(openContentOptions);

            Button button = new Button
            {
                Width = 200,
                Height = 300,
                Content = baseLayout,
                ContextFlyout = ctxMenu,
                Margin = new Thickness(0, 0, 12, 12),
                Padding = new Thickness(0, 0, 0, 0),
                Tag = tag
            };

            return button;
        }


        public static Grid CreateShadlerEpisodeButton(string episode, EventHandler<RoutedEventArgs> playButtonEvent)
        {
            Grid episodeViewerGrid = new Grid
            {
                Margin = new Thickness(0, 6, 0, 0),
                Background = new SolidColorBrush(0x0FFFFFFF)
            };

            episodeViewerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });
            episodeViewerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            episodeViewerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Auto) });

            TextBlock episodeString = new TextBlock
            {
                Margin = new Thickness(12, 0, 0, 0),
                Text = "Episode " + episode,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            Button playButton = new Button
            {
                Tag = episode,
                Margin = new Thickness(0, 12, 12, 12),
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = new FluentAvalonia.UI.Controls.SymbolIcon { Symbol = FluentAvalonia.UI.Controls.Symbol.Play }
            };

            Button downloadButton = new Button
            {
                Tag = episode,
                Margin = new Thickness(0, 12, 12, 12),
                HorizontalAlignment = HorizontalAlignment.Right,
                IsEnabled = false, // download feature is not implemented yet
                Content = new FluentAvalonia.UI.Controls.SymbolIcon { Symbol = FluentAvalonia.UI.Controls.Symbol.Download }
            };

            playButton.Click += playButtonEvent;

            Grid.SetColumn(episodeString, 0);
            Grid.SetColumn(playButton, 1);
            Grid.SetColumn(downloadButton, 2);

            episodeViewerGrid.Children.Add(episodeString);
            episodeViewerGrid.Children.Add(playButton);
            episodeViewerGrid.Children.Add(downloadButton);

            return episodeViewerGrid;

        }


    }
}
