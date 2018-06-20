using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyKeyboard
{
    public class PictureDirectory
    {
        public string Directory { get; set; }

        public bool WithSubDirectories { get; set; } = true;
    }

    public class Settings
    {
        public static Settings Default { get; private set; } = new Settings();

        public static string FileName => Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(BabyKeyboard), "Settings.json");

        public static void Load()
        {
            if(File.Exists(FileName))
            {
                using (var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(sr.ReadToEnd());
                        if(settings != null)
                        {
                            Default = settings;
                        }
                    }
                }
            }
        }

        public static void Save()
        {
            if (Default == null)
                return;

            var dir = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(Default, Newtonsoft.Json.Formatting.Indented);
            using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                }
            }
        }

        public bool ShowPictures { get; set; } = true;

        public int MinPictureWidth { get; set; } = 400;

        public int MinPictureHeight { get; set; } = 400;

        public List<PictureDirectory> PictureDirectories = new List<PictureDirectory>();

        public List<string> Pictures { get; set; } = new List<string>();
    }
}
