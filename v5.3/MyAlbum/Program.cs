using System.IO;
using System.Reflection.Metadata;
using System.Xml.Serialization;
using MyAlbum.Models.Xml;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a file path as an argument.");
    return;
}

string filePath = Path.Combine(Directory.GetCurrentDirectory(), args[0]);

if (!File.Exists(filePath))
{
    Console.WriteLine($"File not found: {filePath}");
    return;
}

var serializer = new XmlSerializer(typeof(XmlAlbum));
using var stream = File.OpenRead(filePath);
var album = (XmlAlbum)serializer.Deserialize(stream);


Console.WriteLine("Press any key to exit...");
Console.ReadKey();