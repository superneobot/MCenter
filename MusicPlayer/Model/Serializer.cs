using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;

namespace MusicPlayer.Model;

public static class Serializer
{
    public static void Save(ObservableCollection<MusicFile> source, string path)
    {
        var json = JsonConvert.SerializeObject(source, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static ObservableCollection<MusicFile> Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<ObservableCollection<MusicFile>>(json);
    }
}