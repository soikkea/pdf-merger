using System;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PdfMerger.Views.Controls
{
    public sealed partial class PdfViewer : UserControl
    {
        public PdfViewer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Display a PdfDocument in the control
        /// </summary>
        /// <param name="pdf">PdfDocument to be displayed</param>
        /// <returns></returns>
        public async Task DisplayPdfDoc(PdfDocument pdf)
        {
            var items = PagesContainer.Items;
            items.Clear();

            if (pdf == null) return;

            for (uint i = 0; i < pdf.PageCount; i++)
            {
                using (var page = pdf.GetPage(i))
                {
                    var bitmap = await PageToBitmapAsync(page);
                    var image = new Image
                    {
                        Source = bitmap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 4, 0, 0),
                        MaxWidth = 800
                    };
                    items.Add(image);
                }
            }
        }

        /// <summary>
        /// Convert a PdfPage to a BitmapImage
        /// </summary>
        /// <param name="page">PdfPage to be converted</param>
        /// <returns>BitmapImage</returns>
        private static async Task<BitmapImage> PageToBitmapAsync(PdfPage page)
        {
            var image = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                await image.SetSourceAsync(stream);
            }

            return image;
        }
    }
}
