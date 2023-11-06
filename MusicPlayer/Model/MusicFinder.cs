using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;

namespace MusicPlayer.Model;

public static class MusicFinder
{
    static int _allpages = 0;
    static string Address = "https://ru2.hitmo.top";

    public async static void Find(string text, ObservableCollection<MusicFile> source,
        MusicCenterState state)
    {
        source.Clear();
        switch (state)
        {
            case MusicCenterState.Поиск:
                await FindMusic(text, source);
                break;
            default:
                break;
        }
    }

    public async static Task Find(ObservableCollection<MusicFile> source, MusicCenterState state)
    {
        source.Clear();
        switch (state)
        {
            case MusicCenterState.Популярное:
                await FindTopOfTheDay(source);
                break;
            case MusicCenterState.Новое:
                await FindNewTracks(source);
                break;
            default:
                break;
        }
    }

    public static async Task FindNewTracks(ObservableCollection<MusicFile> source)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var address = $"{Address}/collections/11552";
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(address);
        try
        {
            var cells = document.QuerySelector("ul[class='mainSongs songs songsListen ajaxSection_content']");
            if (cells != null)
            {
                var founds = cells.QuerySelectorAll("li[class='item  ']");
                foreach (var item in founds)
                {
                    var artist = item.QuerySelector("span[class='artist']").TextContent;
                    var title = item.QuerySelector("span[class='track']").TextContent;
                    var poster = Address + item.QuerySelector("div[class='playlistImg']").QuerySelector("img")
                        .GetAttribute("data-src").Replace("small", "large");
                    var remote = item.QuerySelector("div[class='play']").GetAttribute("data-url");
                    var duration = item.QuerySelector("div[class='duration']").TextContent.Trim();
                    var id = item.GetAttribute("data-id");
                    var file = new MusicFile(title, artist, poster, remote, id, duration);
                    source.Add(file);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Ничего не найдено!", "Music Center", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }
        catch
        {
        }
    }

    public static async Task FindTopOfTheDay(ObservableCollection<MusicFile> source)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var address = $"{Address}/top-today";
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(address);
        try
        {
            var cells = document.QuerySelector("ul[class='mainSongs unstyled songs songsListen favoriteConf']");
            if (cells != null)
            {
                var founds = cells.QuerySelectorAll("li[class='item  ']");
                foreach (var item in founds)
                {
                    var artist = item.QuerySelector("span[class='artist']").TextContent;
                    var title = item.QuerySelector("span[class='track']").TextContent;
                    var poster = Address + item.QuerySelector("div[class='playlistImg']").QuerySelector("img")
                        .GetAttribute("data-src").Replace("small", "large");
                    var remote = item.QuerySelector("div[class='play']").GetAttribute("data-url");
                    var duration = item.QuerySelector("div[class='duration']").TextContent.Trim();
                    var id = item.GetAttribute("data-id");
                    var file = new MusicFile(title, artist, poster, remote, id, duration);
                    source.Add(file);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Ничего не найдено!", "Music Center", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }
        catch
        {
        }
    }

    private static async Task FindMusic(string searchText, ObservableCollection<MusicFile> source)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var text = searchText.Replace(" ", "+");
        var address = $"{Address}/search?q={text}";
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(address);
        try
        {
            var cells = document.QuerySelector("ul[class='songs mainSongs ajaxSection_content']");
            if (cells != null)
            {
                var founds = cells.QuerySelectorAll("li[class='item  ']");
                foreach (var item in founds)
                {
                    var artist = item.QuerySelector("span[class='artist']").TextContent;
                    var title = item.QuerySelector("span[class='track']").TextContent;
                    var poster = Address + item.QuerySelector("div[class='playlistImg']").QuerySelector("img")
                        .GetAttribute("data-src").Replace("small", "large");
                    var remote = item.QuerySelector("div[class='play']").GetAttribute("data-url");
                    var duration = item.QuerySelector("div[class='duration']").TextContent.Trim();
                    var id = item.GetAttribute("data-id");
                    var file = new MusicFile(title, artist, poster, remote, id, duration);
                    source.Add(file);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Ничего не найдено!", "Music Center", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }
        catch
        {
        }
    }

    private static async Task GetPages(string searchText)
    {
        try
        {
            var config = Configuration.Default.WithDefaultLoader();
            var text = searchText.Replace(" ", "+");
            var address = $"{Address}/{0}?q={text}";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            //
            var paginator = document.QuerySelector("section[class='pagination']");
            if (paginator != null)
            {
                var pages = paginator.QuerySelectorAll("li");
                var lastPage = pages.Take(pages.Length).LastOrDefault();
                var countStr = lastPage.InnerHtml;
                int count = 1;
                int.TryParse(string.Join("", countStr.Where(c => char.IsDigit(c))), out count);
                _allpages = count;
            }
            else
            {
                _allpages = 0;
            }
        }
        catch
        {
            _allpages = 0;
        }
    }
}