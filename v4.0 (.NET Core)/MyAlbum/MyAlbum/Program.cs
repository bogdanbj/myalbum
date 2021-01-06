using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using System.Xml.Linq;

namespace MyAlbum
{
    class Program
    {
        //static XDocument XAlbum { get; set; }
        //static XElement errXml { get; set; }
        static Album album = new Album();

        static void Main(string[] args)
        {

            String fileName;
            String outputName;
            //Album album = new Album();

            if (args[0] == "TEST")
            {
                album.Test();
                album.Save("test.pdf");
                Process.Start("test.pdf");
                return;
            }

			try
			{
				fileName = args[0];
                XDocument XAlbum = XDocument.Load(fileName);
                // check xml version
                if (Convert.ToDouble(XAlbum.Root.Attribute("ver").Value) < 3.0) { throw new FormatException("XML file is not compatible with this version."); }

                album = new Album(XAlbum.Root);
                album.Parse();
                album.Draw();

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
                
                album.Save(outputName);

                
                var psi = new ProcessStartInfo
                {
                    FileName = outputName,
                    UseShellExecute = true
                };
                Process.Start(psi);
                //Process.Start(outputName);

#if DEBUG
                Console.WriteLine("Press any key to close...");
                //Console.ReadKey();
#endif

			}
			catch (Exception ex)
			{
                Console.WriteLine("*** ERROR **");
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(ex.Source);
                Console.WriteLine();
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                //Console.WriteLine(errXml);
                //Console.WriteLine();
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
            }
			
		}
    }
}
