namespace SkillTableExpanded.Utils;

public class BigEndianBinaryReader : IDisposable
{
    private readonly BinaryReader _reader;
    
    public BigEndianBinaryReader(Stream input)
    {
        _reader = new BinaryReader(input);
    }

    public byte[] ReadBytes(int count) => _reader.ReadBytes(count);
    public byte[] ReadBytesAndReverse(int count) => Reverse(_reader.ReadBytes(count));
    public byte ReadByte() => _reader.ReadByte();
    public short ReadInt16() => BitConverter.ToInt16(Reverse(_reader.ReadBytes(2)));
    public ushort ReadUInt16() => BitConverter.ToUInt16(Reverse(_reader.ReadBytes(2)));
    public int ReadInt32() => BitConverter.ToInt32(Reverse(_reader.ReadBytes(4)));
    public uint ReadUInt32() => BitConverter.ToUInt32(Reverse(_reader.ReadBytes(4)));
    
    private byte[] Reverse(byte[] arr)
    {
        var reversedArr = new byte[arr.Length];
        var lastIndex = arr.Length - 1;

        for (var i = 0; i <= lastIndex; i++)
        {
            reversedArr[i] = arr[lastIndex - i];
        }

        return reversedArr;
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}