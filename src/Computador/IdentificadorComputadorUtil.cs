using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Snebur.Computador
{
    public static class IdentificadorComputadorUtil
    {
        private static string IdentificadorComputador { get; set; }

        //private static string Identifier(string wmiClass, string wmiProperty)
        //{
        //    string result = "";
        //    using (var managementClass = new ManagementClass(wmiClass))
        //    {
        //        using (var instances = managementClass.GetInstances())
        //        {
        //            foreach (var instance in instances)
        //            {
        //                if (string.IsNullOrEmpty(result))
        //                {
        //                    result = String.Concat(instance[wmiProperty], String.Empty);

        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue = "")
        {
            using (var managementClass = new ManagementClass(wmiClass))
            {
                using (ManagementObjectCollection instances = managementClass.GetInstances())
                {
                    foreach (ManagementObject instance in instances)
                    {
                        if (!string.IsNullOrEmpty(wmiMustBeTrue))
                        {

                            if (instance[wmiMustBeTrue].ToString() == "True")
                            {
                                return instance[wmiProperty].ToString();

                            }
                        }
                        else
                        {
                            return String.Concat(instance[wmiProperty], String.Empty);
                        }
                    }
                }
            }
            return string.Empty;
        }

        private static string GetHash(string s)
        {
            using (var sec = new MD5CryptoServiceProvider())
            {
                var enc = new ASCIIEncoding();
                byte[] bt = enc.GetBytes(s);
                return GetHexString(sec.ComputeHash(bt));
            }
        }

        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i <= bt.Length - 1; i++)
            {
                byte b = bt[i];
                var n = Convert.ToInt32(b);
                var n1 = n & 15;
                var n2 = (n >> 4) & 15;

                if (n2 > 9)
                {
                    s += Convert.ToChar((n2 - 10 + char.ConvertToUtf32("A", 0)));
                }
                else
                {
                    s += n2.ToString();
                }

                if (n1 > 9)
                {
                    s += Convert.ToChar((n1 - 10 + char.ConvertToUtf32("A", 0)));
                }
                else
                {
                    s += n1.ToString();
                }

                if ((i + 1) != bt.Length && (i + 1) % 2 == 0)
                {
                    s += "-";
                }
            }
            return s;
        }

        private static string CpuId()
        {
            string classeProcessador = "Win32_Processor";

            string retVal = Identifier(classeProcessador, "ProcessorId");
            if (string.IsNullOrEmpty(retVal))
            {
                retVal = Identifier(classeProcessador, "Name");
                if (string.IsNullOrEmpty(retVal))
                {
                    retVal = Identifier(classeProcessador, "Manufacturer");
                }
                retVal += Identifier(classeProcessador, "MaxClockSpeed");
            }

            return retVal;
        }

        private static string BiosId()
        {
            var classeBios = "Win32_BIOS";

            return Identifier(classeBios, "Manufacturer")
                 + Identifier(classeBios, "SMBIOSBIOSVersion")
                 + Identifier(classeBios, "IdentificationCode")
                 + Identifier(classeBios, "SerialNumber")
                 + Identifier(classeBios, "ReleaseDate")
                 + Identifier(classeBios, "Version");
        }

        private static string DiskId()
        {
            var classeDisco = "Win32_PhysicalMedia";

            return Identifier(classeDisco, "SerialNumber");
        }

        private static string BaseId()
        {
            var classeBaseBoard = "Win32_BaseBoard";

            return Identifier(classeBaseBoard, "Model")
                 + Identifier(classeBaseBoard, "Manufacturer")
                 + Identifier(classeBaseBoard, "Name")
                 + Identifier(classeBaseBoard, "SerialNumber");
        }

        private static string VideoId()
        {
            var classeVideo = "Win32_VideoController";
            return Identifier(classeVideo, "DriverVersion")
                 + Identifier(classeVideo, "Name");
        }

        private static string MacId()
        {
            return Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }

        public static string LicencaID(string cpuID, string biosID, string baseID)
        {

            if (string.IsNullOrEmpty(IdentificadorComputador))
            {
                IdentificadorComputador = GetHash($"CPU >> {cpuID};BIOS >> {biosID};BASE >> {baseID}");
            }
            return IdentificadorComputador;

        }

        public static InfoComputador RetornarInformacaoComputador()
        {
            var cpuID = CpuId();
            var biosID = BiosId();
            var diskID = DiskId();
            var baseID = BaseId();
            var videoID = VideoId();
            var macID = MacId();
            var licencaID = LicencaID(cpuID, biosID, baseID);

            var licenca = new InfoComputador
            {
                CPUID = cpuID,
                BIOSID = biosID,
                DiskID = diskID,
                BaseID = baseID,
                VideoID = videoID,
                MAC = macID,
                LicencaID = licencaID
            };
            return licenca;
        }

    }

    public class InfoComputador
    {
        public string CPUID { get; set; }
        public string BIOSID { get; set; }
        public string DiskID { get; set; }
        public string BaseID { get; set; }
        public string VideoID { get; set; }
        public string MAC { get; set; }
        public string IP { get; set; }
        public string Usuario { get; set; }
        public string LicencaID { get; set; }
    }
}
