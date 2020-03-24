using PdfMerger.Models;
using PdfMerger.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PdfMerger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public MainViewModel ViewModel => App.ViewModel;

        private async void addFileButton_Click(object sender, RoutedEventArgs e)
        {
            var latestPdfFile = await ViewModel.LoadPdfFile();

            await DisplayPdf(latestPdfFile);
        }

        private async Task DisplayPdf(PdfFile pdf)
        {
            var pdfDocument = await PdfDocument.LoadFromFileAsync(pdf.File);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    await pdfViewer.DisplayPdfDoc(pdfDocument);
                });
        }

        private async void pdfListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pdfListView.SelectedItem is PdfFile pdf)
            {
                await DisplayPdf(pdf);
            }
        }

        private string rightClickedFilePath;

        private void pdfListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            pdfMenuFlyout.ShowAt(listView, e.GetPosition(listView));
            var a = ((FrameworkElement)e.OriginalSource).DataContext as PdfFile;
            rightClickedFilePath = a.File.Path;
        }

        private void removePdfFromList_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pdfFile in ViewModel.PdfFiles)
            {
                if (pdfFile.File.Path == rightClickedFilePath)
                {
                    ViewModel.PdfFiles.Remove(pdfFile);
                    break;
                }
            }
            rightClickedFilePath = null;
        }
    }
}
