using System.Xml.Serialization;
using MyAlbum.Models.Xml;

namespace MyAlbum.Utils
{
    public static class XmlFactory
    {
        /// <summary>
        /// Deserializes an XML file into an XmlAlbum model.
        /// </summary>
        /// <param name="filePath">Path to the XML file</param>
        /// <returns>Deserialized XmlAlbum object</returns>
        /// <exception cref="ArgumentException">Thrown when file path is null or empty</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when file does not exist</exception>
        /// <exception cref="InvalidOperationException">Thrown when XML deserialization fails</exception>
        public static XmlAlbum Deserialize(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"XML file not found: {filePath}", filePath);

            try
            {
                var serializer = new XmlSerializer(typeof(XmlAlbum));
                using var stream = File.OpenRead(filePath);
                var xmlAlbum = (XmlAlbum)serializer.Deserialize(stream);

                if (xmlAlbum == null)
                    throw new InvalidOperationException("Deserialization resulted in null XmlAlbum.");

                return xmlAlbum;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize XML file '{filePath}': {ex.Message}", ex);
            }
        }
        public static XmlAlbum Deserialize(string filePath, AlbumStyles styles)
        {
            var album = Deserialize(filePath);
            //if (album.Styles == null && styles != null)
            if (styles != null)
            {
                album.Styles = styles;
            }
            return album;
        }

        public static AlbumStyles DeserializeStyles(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Styles XML file not found: {filePath}", filePath);

            try
            {
                var serializer = new XmlSerializer(typeof(AlbumStyles), new XmlRootAttribute("styles"));
                //var serializer = new XmlSerializer(typeof(AlbumStyles));
                using var stream = File.OpenRead(filePath);
                var styles = (AlbumStyles)serializer.Deserialize(stream);

                if (styles == null)
                    throw new InvalidOperationException("Deserialization resulted in null AlbumStyles.");

                return styles;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize styles XML file '{filePath}': {ex.Message}", ex);
            }
        }


    }
}
