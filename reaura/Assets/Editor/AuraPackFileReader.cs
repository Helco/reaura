using System;
using System.IO;

public class AuraPackFileReader
{
    private static readonly uint xorKey = 0x556D6EEC;
    private static readonly byte xorKeyByte = 0xB5;
    private static readonly System.Text.Encoding encoding = System.Text.Encoding.ASCII;

    private BinaryReader reader;

    public AuraPackFileReader(Stream stream)
    {
        reader = new BinaryReader(stream, encoding, true);
    }

    public byte[] ReadRaw(int length) => reader.ReadBytes(length);

    public uint ReadU32() => reader.ReadUInt32() ^ xorKey;

    public byte[] ReadBuffer(int length)
    {
        var memStream = new MemoryStream(length);
        var memWriter = new BinaryWriter(memStream);
        for (;  length >= 4; length -= 4)
            memWriter.Write(ReadU32());
        for (; length > 0; length--)
            memWriter.Write((byte)(reader.ReadByte() ^ xorKeyByte));
        memWriter.Close();
        return memStream.ToArray();
    }

    public string ReadString(int maxLength)
    {
        var buffer = ReadBuffer(maxLength);
        int length = Array.IndexOf(buffer, (byte)0);
        if (length < 0)
            length = maxLength;
        return encoding.GetString(buffer, 0, length);
    }

    public string[] ReadFileList()
    {
        uint fileCount = ReadU32();
        if (fileCount > 1000)
            throw new InvalidDataException("Too many files in archive");
        string[] fileNames = new string[fileCount];
        for (uint i = 0; i < fileCount; i++)
            fileNames[i] = ReadString(128);
        return fileNames;
    }
}
