using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cleverscape.UTorrentClient.WebClient
{
    internal static class Utilities
    {
        internal static string FormatFileSize(long SizeBytes)
        {
            return FormatFileSize(SizeBytes, FileSizeFormat.SizeBytes);
        }

        internal static string FormatFileSize(long SizeBytes, FileSizeFormat Format)
        {
            string Suffix = "";
            string SuffixSingular = "";
            long SizeToUse = SizeBytes;
            switch (Format)
            {
                case FileSizeFormat.SizeBytes:
                    Suffix = "B";
                    SuffixSingular = "Bytes";
                    break;
                case FileSizeFormat.SizeBits:
                    Suffix = "b";
                    SuffixSingular = "bits";
                    SizeToUse = SizeToUse * 8;
                    break;
                case FileSizeFormat.SpeedBytes:
                    Suffix = "B/s";
                    SuffixSingular = "Bytes/s";
                    break;
                case FileSizeFormat.SpeedBits:
                    Suffix = "b/s";
                    SuffixSingular = "bits/s";
                    SizeToUse = SizeToUse * 8;
                    break;
                default:
                    break;
            }

            if (SizeToUse >= 1125899906842624)
            {
                Decimal size = Decimal.Divide(SizeToUse, 1125899906842624);
                return String.Format("{0:##.##} P{1}", size, Suffix);
            }
            if (SizeToUse >= 1099511627776)
            {
                Decimal size = Decimal.Divide(SizeToUse, 1099511627776);
                return String.Format("{0:##.##} T{1}", size, Suffix);
            }
            if (SizeToUse >= 1073741824)
            {
                Decimal size = Decimal.Divide(SizeToUse, 1073741824);
                return String.Format("{0:##.##} G{1}", size, Suffix);
            }
            else if (SizeToUse >= 1048576)
            {
                Decimal size = Decimal.Divide(SizeToUse, 1048576);
                return String.Format("{0:##.##} M{1}", size, Suffix);
            }
            else if (SizeToUse >= 1024)
            {
                Decimal size = Decimal.Divide(SizeToUse, 1024);
                return String.Format("{0:##.##} K{1}", size, Suffix);
            }
            else if (SizeToUse > 0 & SizeToUse < 1024)
            {
                Decimal size = SizeToUse;
                return String.Format("{0:##.##} {1}", size, SuffixSingular);
            }
            else
            {
                return String.Format("0 {0}", SuffixSingular);
            }
        }

        internal enum FileSizeFormat
        {
            SizeBytes,
            SizeBits,
            SpeedBytes,
            SpeedBits
        }
    }
}
