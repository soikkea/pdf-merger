using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PdfMerger.Models;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PdfMerger.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public ObservableCollection<PdfFile> PdfFiles { get; }
            = new ObservableCollection<PdfFile>();

        public async Task<PdfFile> LoadPdfFile()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".pdf");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                return AddPdf(file);
            }

            return null;
        }

        private PdfFile AddPdf(StorageFile file)
        {
            var pdfFile = new PdfFile
            {
                File = file
            };

            PdfFiles.Add(pdfFile);

            return pdfFile;
        }
    }
}
