using Avalonia.Controls;
using FluentAvalonia.UI.Controls;

namespace Shadler;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        ContentFrame.Navigate(typeof(Views.Browser));

    }

    private void NavBar_SelectionChanged(object sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (sender is NavigationView) {
            if (args.SelectedItemContainer != null && args.SelectedItemContainer.Tag != null) {

                switch(args.SelectedItemContainer.Tag)
                {
                    case "Views.Browser":
                        ContentFrame.Navigate(typeof(Views.Browser));
                        break;
                }
            }
        }
    }
}
