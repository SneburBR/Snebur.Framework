namespace System
{
    public class DoubleByteSpan
    {
        public const ulong TOTAL_BYTES_KB = 1024;
        public const ulong TOTAL_BYTES_MB = TOTAL_BYTES_KB * 1024;
        public const ulong TOTAL_BYTES_GB = TOTAL_BYTES_MB * 1024;
        public const ulong TOTAL_BYTES_TB = TOTAL_BYTES_GB * 1024;
        public const ulong TOTAL_BYTES_PB = TOTAL_BYTES_TB * 1024;
        public const ulong TOTAL_BYTES_HB = TOTAL_BYTES_PB * 1024;

        public const double TOTAL_BYTES_SB = TOTAL_BYTES_HB * 1024d;
        public const double TOTAL_BYTES_YB = TOTAL_BYTES_SB * 1024d;

        public double TotalBytesInterno { get; private set; }
         
        public double TotalMegaBytes
        {
            get
            {
                return this.TotalBytesInterno / TOTAL_BYTES_MB;
            }
        }

        public double TotalKilobytes
        {
            get
            {
                return this.TotalBytesInterno / TOTAL_BYTES_KB;
            }
        }

        public double TotalGibaBytes
        {
            get
            {
                return this.TotalBytesInterno / TOTAL_BYTES_GB;
            }
        }

        public double TotalTeraBytes
        {
            get
            {
                return this.TotalBytesInterno / (double)TOTAL_BYTES_TB;
            }
        }

        public DoubleByteSpan(double totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }

        public DoubleByteSpan(int totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }
        public DoubleByteSpan(long totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }

        public DoubleByteSpan(ulong totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }

        public static DoubleByteSpan FromBytes(ulong totalBytes)
        {
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromKiloBytes(double totalKiloBytes)
        {
            var totalBytes = Convert.ToUInt64(totalKiloBytes * TOTAL_BYTES_KB);
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromMegaBytes(double totalMegaBytes)
        {
            var totalBytes = Convert.ToInt64(totalMegaBytes * TOTAL_BYTES_MB);
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromGigaBytes(double totalGigaBytes)
        {
            var totalBytes = Convert.ToUInt64(totalGigaBytes * TOTAL_BYTES_GB);
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromTeraBytes(double totalTeraBytes)
        {
            var totalBytes = Convert.ToUInt64(totalTeraBytes * TOTAL_BYTES_TB);
            return new ByteSpan(totalBytes);
        }
    }
    public class DoubleByteSpan
    {
        public const ulong TOTAL_BYTES_KB = 1024;
        public const ulong TOTAL_BYTES_MB = TOTAL_BYTES_KB * 1024;
        public const ulong TOTAL_BYTES_GB = TOTAL_BYTES_MB * 1024;
        public const ulong TOTAL_BYTES_TB = TOTAL_BYTES_GB * 1024;
        public const ulong TOTAL_BYTES_PB = TOTAL_BYTES_TB * 1024;
        public const ulong TOTAL_BYTES_HB = TOTAL_BYTES_PB * 1024;

        public const double TOTAL_BYTES_SB = TOTAL_BYTES_HB * 1024d;
        public const double TOTAL_BYTES_YB = TOTAL_BYTES_SB * 1024d;

        public double TotalBytesInterno { get; private set; }

        public double TotalMegaBytes
        {
            get
            {
                return this.TotalBytesInterno / TOTAL_BYTES_MB;
            }
        }

        public double TotalKilobytes
        {
            get
            {
                return this.TotalBytesInterno / TOTAL_BYTES_KB;
            }
        }

        public double TotalGibaBytes
        {
            get
            {
                return this.TotalBytesInterno / TOTAL_BYTES_GB;
            }
        }

        public double TotalTeraBytes
        {
            get
            {
                return this.TotalBytesInterno / (double)TOTAL_BYTES_TB;
            }
        }

        public DoubleByteSpan(double totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }

        public DoubleByteSpan(int totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }
        public DoubleByteSpan(long totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }

        public DoubleByteSpan(ulong totalBytes)
        {
            this.TotalBytesInterno = totalBytes;
        }

        public static DoubleByteSpan FromBytes(ulong totalBytes)
        {
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromKiloBytes(double totalKiloBytes)
        {
            var totalBytes = Convert.ToUInt64(totalKiloBytes * TOTAL_BYTES_KB);
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromMegaBytes(double totalMegaBytes)
        {
            var totalBytes = Convert.ToInt64(totalMegaBytes * TOTAL_BYTES_MB);
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromGigaBytes(double totalGigaBytes)
        {
            var totalBytes = Convert.ToUInt64(totalGigaBytes * TOTAL_BYTES_GB);
            return new ByteSpan(totalBytes);
        }

        public static DoubleByteSpan FromTeraBytes(double totalTeraBytes)
        {
            var totalBytes = Convert.ToUInt64(totalTeraBytes * TOTAL_BYTES_TB);
            return new ByteSpan(totalBytes);
        }
    }
}