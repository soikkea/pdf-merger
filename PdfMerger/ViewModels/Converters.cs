using System.Collections;

namespace PdfMerger.ViewModels
{
    public static class Converters
    {
        public static bool IsNotEmpty(ICollection list) => list?.Count > 0;
    }
}
