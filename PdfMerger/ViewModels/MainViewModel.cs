using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PdfMerger.Models;
using PdfMerger.Utilities;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PdfMerger.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PdfMergeUtility pdfMergeUtility = new PdfMergeUtility();

        public ObservableCollection<PdfFile> PdfFiles { get; }
            = new ObservableCollection<PdfFile>();

        public StorageFile LatestMergedFile { get; private set; }

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

        public async Task<bool> MergePdfs()
        {
            var fileList = PdfFiles
                .Select(pf => pf.File)
                .ToList();

            var byteList = new List<byte[]>();

            foreach (var file in fileList)
            {
                byte[] byteArray;
                using (var fileStream = await file.OpenStreamForReadAsync())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        byteArray = memoryStream.ToArray();
                    }
                }
                byteList.Add(byteArray);
            }

            var mergedPdfBytes = pdfMergeUtility.MergePdfs(byteList);

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "Merged Pdf"
            };
            savePicker.FileTypeChoices.Add("Pdf", new List<string>() { ".pdf" });

            var mergedFile = await savePicker.PickSaveFileAsync();
            if (mergedFile != null)
            {
                // Prevent updates to the remote version of the file until
                // are changes are finished and CompleteUpdatesAsync is called
                CachedFileManager.DeferUpdates(mergedFile);

                await FileIO.WriteBytesAsync(mergedFile, mergedPdfBytes);

                // Let Windows know that changes to the file are finished so
                // that the remote version of the file can be updated.
                var status = await CachedFileManager.CompleteUpdatesAsync(mergedFile);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    LatestMergedFile = mergedFile;
                    return true;
                }

            }
            return false;
        }

        public PdfFile LoadLatestMerge()
        {
            if (LatestMergedFile == null)
            {
                return null;
            }

            PdfFiles.Clear();
            return AddPdf(LatestMergedFile);
        }
    }
}
