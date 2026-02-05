using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Xml.Serialization;
using MyAlbum.Models.Pdf;
using MyAlbum.Models.Xml;
using MyAlbum.Services;



namespace MyAlbum
{ 
    class Program
    {
        static void Main(string[] args)
        {

            //if (args[0] == "TEST")
            //{
            //    album.Test();
            //    album.Save("test.pdf");
            //    Process.Start("test.pdf");
            //    return;
            //}
            
            String fileName;
            String outputName;

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a file path as an argument.");
                return;
            }

            fileName = Path.Combine(Directory.GetCurrentDirectory(), args[0]);

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

            try
            {
                // Deserialize XML file into XML Model objects
                var xmlAlbum = XmlFactory.Deserialize(fileName);


                // Generate album file
                PdfFactory.CreatePdfFromXmlAlbum(xmlAlbum, outputName);

                
                // Open the PDF file
                var psi = new ProcessStartInfo
                {
                    FileName = outputName,
                    UseShellExecute = true
                };
                Process.Start(psi);
                //Process.Start(outputName);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"XML deserialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

#if DEBUG
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
#endif


        }
    }

}