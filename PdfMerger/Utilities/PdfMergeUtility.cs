using System.Collections.Generic;
using System.IO;
using System.Text;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMerger.Utilities
{
    public class PdfMergeUtility
    {
        public PdfMergeUtility()
        {
            // Increase the number of encodings supported
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public byte[] MergePdfs(List<byte[]> documentsToMerge)
        {
            var outputDocument = ConcatenatePdfs(documentsToMerge);
            return SaveDocumentToByteArray(outputDocument);
        }

        private PdfDocument ConcatenatePdfs(List<byte[]> documentsToConcatenate)
        {
            var outputDocument = new PdfDocument();

            foreach (var file in documentsToConcatenate)
            {
                using (var stream = new MemoryStream(file))
                {
                    var inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

                    int pageCount = inputDocument.PageCount;
                    for (int i = 0; i < pageCount; i++)
                    {
                        var page = inputDocument.Pages[i];
                        outputDocument.AddPage(page);
                    }

                    inputDocument.Close();
                }
            }

            return outputDocument;
        }

        private byte[] SaveDocumentToByteArray(PdfDocument document)
        {
            var stream = new MemoryStream();
            document.Save(stream);
            stream.Close();
            return stream.ToArray();
        }
    }
}
