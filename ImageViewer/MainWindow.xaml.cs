using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebImage Image;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateDetailsPanel(WebImage image)
        {
            string imageTitle = image.GetImageTitle();
            if (imageTitle.Length > 0)
            {
                image_type.Content = $"FILETYPE: {imageTitle.Substring(imageTitle.Length - 4)}";
            } 
            else
            {
                image_type.Content = $"FILETYPE:";
            }
            image_title.Content = $"TITLE: {imageTitle.Substring(0, imageTitle.Length - 4)}";
            image_width.Content = $"WIDTH: {image.GetImageWidth()}px";
            image_height.Content = $"HEIGHT: {image.GetImageHeight()}px";
        }

        private async Task<WebImage> RequestImage(string uri)
        {
            WebImage newImage = new WebImage(uri);
            try
            {
                error_label.Visibility = Visibility.Hidden;
                bool loaded = await newImage.LoadImage();
                if (loaded)
                {
                    image_preview.Source = newImage.GetImage();
                }
                else
                {
                    error_label.Visibility = Visibility.Visible;
                }
                return newImage;
            }
            catch (Exception e)
            {
                error_label.Visibility = Visibility.Visible;
                return null;
            }
        }

        private async void Submit_Preview(object sender, RoutedEventArgs e)
        {
            Image = await RequestImage(image_url.Text);
            if (Image != null)
            {
                Image.GetImage().DownloadCompleted += (s, evtArgs) =>
                {
                    UpdateDetailsPanel(Image);
                };
            }
        }

        private void Download_Image(object sender, RoutedEventArgs e)
        {
            if (Image != null && !Image.GetImage().IsDownloading)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = Image.GetImageTitle().Substring(Image.GetImageTitle().Length - 4);
                dialog.DefaultExt = ".png";
                dialog.Filter = "Picture (.png)|*.png";
                //TODO: Support more formats other than PNG.

                bool? dialogOpened = dialog.ShowDialog();

                if (dialogOpened == true)
                {
                    // BitmapImage saving source: https://stackoverflow.com/questions/35804375/how-do-i-save-a-bitmapimage-from-memory-into-a-file-in-wpf-c 
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(Image.GetImage()));
                    Stream imageStream = new FileStream(dialog.FileName, FileMode.Create);
                    encoder.Save(imageStream);
                }
            }
        }

    }
}
