using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Xml.Serialization;
using MyAlbum.Models.Xml;
using MyAlbum.Services;



namespace MyAlbum
{ 
    class Program
    {
        static void Main(string[] args)
        {

            String fileName;
            String outputName;

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a file path as an argument.");
                return;
            }

            fileName = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File not found: {fileName}");
                return;
            }

            // Deserialize XML file into XML Model objects
            var serializer = new XmlSerializer(typeof(XmlAlbum));
            using var stream = File.OpenRead(fileName);
            var xmlAlbum = (XmlAlbum)serializer.Deserialize(stream);

            // Create the pdfAlbum from xmlAlbum


            outputName = Path.ChangeExtension(fileName, ".pdf");
            outputName = outputName.Replace("Templates", "Output");
            if (!Directory.Exists(Path.GetDirectoryName(outputName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputName));
            }
            if (File.Exists(outputName))
            {
                for (int i = 0; i < 10000; i++)
                {
                    ;
                    if (!(File.Exists(outputName = fileName.Replace("Templates", "Output").Replace(Path.GetExtension(fileName), i.ToString() + ".pdf"))))
                        break;
                }
            }

            // Generate album file
            PdfFactory.CreatePdfFromXmlAlbum(xmlAlbum, outputName);

            var psi = new ProcessStartInfo
            {
                FileName = outputName,
                UseShellExecute = true
            };
            Process.Start(psi);
            //Process.Start(outputName);

#if DEBUG
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
#endif


        }
    }

}