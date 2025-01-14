using SmarterLead.API.AgencyLeadsManager.Entities;
using System.Globalization;
using System.IO.Compression;

namespace SmarterLead.API.AgencyLeadsManager
{
    public class FileManager
    {
        public static void ZipCSV(string filepath, string destPath)
        {
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }

            ZipFile.CreateFromDirectory(filepath, destPath);
        }

        //public static List<ZohoLeadEntity> ReadCSVFromZip(byte[] fileBytes)
        //{
        //    List<ZohoLeadEntity> records = new List<ZohoLeadEntity>();
        //    try
        //    {
        //        using (MemoryStream zipStream = new MemoryStream(fileBytes))
        //        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
        //        {
        //            var csvEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

        //            if (csvEntry != null)
        //            {
        //                using (StreamReader reader = new StreamReader(csvEntry.Open()))
        //                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //                {
        //                    records = csv.GetRecords<ZohoLeadEntity>().ToList();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //
        //    }

        //    return records;

        //}

        public static void CleanDirectory(string dirPath)
        {
            if (!string.IsNullOrEmpty(dirPath) && Directory.Exists(dirPath))
            {

                Directory.Delete(dirPath, true);

                Directory.CreateDirectory(dirPath);

            }
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        public static bool DoesFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

    }
}
