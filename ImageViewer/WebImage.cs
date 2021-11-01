using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageViewer
{
    class WebImage
    {
        // Data
        private string s_uri;
        private BitmapImage s_image;
        private static HttpClient Client = new HttpClient();

        // Constructors
        public WebImage(string imageUri)
        {
            s_uri = imageUri;
        }

        // Methods
        public BitmapImage GetImage()
        {
            return s_image;
        }

        public string GetImageTitle()
        {
            // Regex Source: https://stackoverflow.com/questions/37807568/get-only-the-image-filename-from-a-url-with-regex
            Regex regex = new Regex(@"[\w-]+\.(jpg|png|gif|PNG|JPG|GIF)");
            return regex.Match(s_uri).Value;
        }

        public int GetImageHeight()
        {
            if (s_image != null)
            {
                return s_image.PixelHeight;
            }
            return 0;
        }

        public int GetImageWidth()
        {
            if (s_image != null)
            {
                return s_image.PixelWidth;
            }
            return 0;
        }
        
        /*
         * Makes a GET request to a provided uri leading to an image.
         * If successful returns true, else returns false.
         */
        public async Task<bool> LoadImage()
        {
            try
            {
                Uri imgUri = new Uri(s_uri);
                HttpResponseMessage res = await Client.GetAsync(s_uri);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    s_image = new BitmapImage(imgUri);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
