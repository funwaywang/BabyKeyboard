using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BabyKeyboard
{
    public partial class MainWindow : Window
    {
        private Random Rand = new Random(DateTime.Now.Second * DateTime.Now.Millisecond);
        private List<string> Pictures = new List<string>();
        private BitmapImage SecondImage = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            PlayOne();
            InitializePicturesAsync();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            PlayOne();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            PlayOne();
        }

        private void PlayOne()
        {
            bool hasPicture = false;
            if (Settings.Default.ShowPictures)
            {
                if (SecondImage != null)
                {
                    Background = new ImageBrush(SecondImage);
                    hasPicture = true;
                }
                else if(Pictures.Any())
                {
                    var picture = Pictures[Rand.Next(Pictures.Count)];
                    if (File.Exists(picture))
                    {
                        Background = new ImageBrush(new BitmapImage(new Uri(picture, UriKind.Absolute)));
                        hasPicture = true;
                    }
                }

                Task.Factory.StartNew(ReadySecondImage);
            }

            if(!hasPicture)
            {
                Background = new SolidColorBrush(ColorHelper.RandomColor());
                Foreground = new SolidColorBrush(ColorHelper.RandomColor());
            }
            else
            {
                Foreground = Brushes.Transparent;
            }

            //SystemSounds.Beep.Play();
        }

        private void ReadySecondImage()
        {
            SecondImage = null;

            if (Pictures.Any())
            {
                var picture = Pictures[Rand.Next(Pictures.Count)];
                if (File.Exists(picture))
                {
                    SecondImage = new BitmapImage(new Uri(picture, UriKind.Absolute));
                }
            }
        }

        private void InitializePicturesAsync()
        {
            var extensions = new HashSet<string>(new string[] { ".jpg", ".png", ".jpeg", ".bmp", ".tiff" }, StringComparer.OrdinalIgnoreCase);
            foreach (var dir in Settings.Default.PictureDirectories)
            {
                foreach (var file in Directory.EnumerateFiles(dir.Directory, "*.*", dir.WithSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    if (extensions.Contains(System.IO.Path.GetExtension(file)))
                    {
                        Task.Factory.StartNew(() => { TryAddPicture(file); });
                    }
                }
            }

            foreach (var file in Settings.Default.Pictures)
            {
                Task.Factory.StartNew(() => { TryAddPicture(file); });
            }
        }

        private void TryAddPicture(string file)
        {
            if(File.Exists(file))
            {
                try
                {
                    var image = new BitmapImage(new Uri(file, UriKind.Absolute));
                    if(image.Width > 400 && image.Height > 400)
                    {
                        Pictures.Add(file);

                        if (SecondImage == null && Settings.Default.ShowPictures)
                        {
                            //Task.Factory.StartNew(ReadySecondImage);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void MenuConfigurationClick(object sender, RoutedEventArgs e)
        {
            var configDialog = new ConfigDialog()
            {
                Owner = this
            };

            configDialog.ShowDialog();
        }
    }
}
