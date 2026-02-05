//using MyAlbum.Models.Pdf;
using MyAlbum.Models.Layout;
using MyAlbum.Models.Xml;
using MyAlbum.Utils;
using PdfSharpCore.Fonts;
using System.Diagnostics;




namespace MyAlbum
{ 
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Register the custom font resolver
                GlobalFontSettings.FontResolver = new MyFontResolver();

                System.Diagnostics.Debug.WriteLine("FontResolver registered");

                Album album = new Album();
                if (args[0] == "TEST")
                {
                    album.Test();
                    album.Save("test.pdf");
                    Process.Start("test.pdf");
                    return;
                }

                String fileName = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
                string outputName = GetOutputName(fileName);

                // Deserialize XML file into XML Model objects
                var xmlAlbum = XmlFactory.Deserialize(fileName);

                // Create, draw and save the Album
                album.FromXml(xmlAlbum);
                album.Draw();
                album.Save(outputName);

                // Open the PDF file
                var psi = new ProcessStartInfo
                {
                    FileName = outputName,
                    UseShellExecute = true
                };
                Process.Start(psi);

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
        static string GetOutputName(string fileName)
        {
            string outputName;
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
            return outputName;
        }
    }

}