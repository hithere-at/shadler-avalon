using Avalonia.Controls;
using System.Collections.Generic;
using Shadler.Utils;
using Shadler.DataStructure;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using System.Data;

namespace Shadler.Views;

public sealed partial class MangaReader : UserControl {

    List<string> availableChapters;
    List<string> pageUrls = new List<string>();

    int chapterIndex;
    int pageIndex;

    string contentId;
    string title;
    string year;

    public MangaReader() {
        this.InitializeComponent();

    }



}
