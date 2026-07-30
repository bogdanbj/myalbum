namespace FontManager;

/// <summary>
/// Reads basic metadata from TrueType/OpenType font files.
/// Extracts font family name and style (regular, bold, italic, boldItalic).
/// </summary>
public static class TtfMetadataReader
{
    /// <summary>
    /// Read font family name and style from a TTF/OTF file.
    /// </summary>
    public static (string FamilyName, MyAlbum.Common.FontStyle Style) ReadFontInfo(string fontPath)
    {
        using var stream = File.OpenRead(fontPath);
        using var reader = new BinaryReader(stream);

        // Read offset table
        var sfntVersion = ReadUInt32BigEndian(reader);
        var numTables = ReadUInt16BigEndian(reader);
        reader.ReadBytes(6); // searchRange, entrySelector, rangeShift

        // Find 'name' and 'OS/2' tables
        uint nameTableOffset = 0;
        uint nameTableLength = 0;
        uint os2TableOffset = 0;

        for (int i = 0; i < numTables; i++)
        {
            var tag = new string(reader.ReadChars(4));
            reader.ReadUInt32(); // checksum
            var offset = ReadUInt32BigEndian(reader);
            var length = ReadUInt32BigEndian(reader);

            if (tag == "name")
            {
                nameTableOffset = offset;
                nameTableLength = length;
            }
            else if (tag == "OS/2")
            {
                os2TableOffset = offset;
            }
        }

        // Read family name from 'name' table
        string familyName = "";
        if (nameTableOffset > 0)
        {
            familyName = ReadNameTable(reader, stream, nameTableOffset);
        }

        // Read style from OS/2 table
        var style = MyAlbum.Common.FontStyle.Regular;
        if (os2TableOffset > 0)
        {
            style = ReadOS2Style(reader, stream, os2TableOffset);
        }

        return (familyName, style);
    }

    private static string ReadNameTable(BinaryReader reader, Stream stream, uint tableOffset)
    {
        stream.Position = tableOffset;

        var format = ReadUInt16BigEndian(reader);
        var count = ReadUInt16BigEndian(reader);
        var stringOffset = ReadUInt16BigEndian(reader);

        // Name ID 1 = Font Family, Name ID 4 = Full Name
        // Platform 3 (Windows), Encoding 1 (Unicode BMP) is preferred
        // Platform 1 (Mac), Encoding 0 (Roman) is fallback

        string? windowsName = null;
        string? macName = null;

        for (int i = 0; i < count; i++)
        {
            var platformId = ReadUInt16BigEndian(reader);
            var encodingId = ReadUInt16BigEndian(reader);
            var languageId = ReadUInt16BigEndian(reader);
            var nameId = ReadUInt16BigEndian(reader);
            var length = ReadUInt16BigEndian(reader);
            var offset = ReadUInt16BigEndian(reader);

            // We want Name ID 1 (Font Family)
            if (nameId != 1)
                continue;

            var savedPos = stream.Position;
            stream.Position = tableOffset + stringOffset + offset;
            var nameBytes = reader.ReadBytes(length);
            stream.Position = savedPos;

            if (platformId == 3 && encodingId == 1)
            {
                // Windows Unicode
                windowsName = System.Text.Encoding.BigEndianUnicode.GetString(nameBytes);
            }
            else if (platformId == 1 && encodingId == 0 && macName == null)
            {
                // Mac Roman
                macName = System.Text.Encoding.ASCII.GetString(nameBytes);
            }
        }

        return windowsName ?? macName ?? "";
    }

    private static MyAlbum.Common.FontStyle ReadOS2Style(BinaryReader reader, Stream stream, uint tableOffset)
    {
        stream.Position = tableOffset;

        // OS/2 table version
        var version = ReadUInt16BigEndian(reader);

        // Skip to fsSelection (offset 62 in OS/2 table)
        reader.ReadBytes(60); // skip xAvgCharWidth through usBreakChar
        var fsSelection = ReadUInt16BigEndian(reader);

        // fsSelection bits:
        // Bit 0 = ITALIC
        // Bit 5 = BOLD
        // Bit 6 = REGULAR

        bool isBold = (fsSelection & 0x0020) != 0;
        bool isItalic = (fsSelection & 0x0001) != 0;

        if (isBold && isItalic)
            return MyAlbum.Common.FontStyle.BoldItalic;
        if (isBold)
            return MyAlbum.Common.FontStyle.Bold;
        if (isItalic)
            return MyAlbum.Common.FontStyle.Italic;

        return MyAlbum.Common.FontStyle.Regular;
    }

    private static ushort ReadUInt16BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(2);
        return (ushort)((bytes[0] << 8) | bytes[1]);
    }

    private static uint ReadUInt32BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        return (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
    }
}
