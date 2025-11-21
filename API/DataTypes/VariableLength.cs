namespace API.DataTypes;

public class VariableLength
{
    // ReSharper disable InconsistentNaming
    private const int SEGMENT_BITS = 0x7F;
    private const int CONTINUE_BIT = 0x80;
    // ReSharper restore InconsistentNaming
    
    public static byte[] Encode(int value)
    {
        List<byte> bytes = new List<byte>();
        int count = 0;

        do
        {
            if (count >= 5)
                throw new OverflowException("ULEB128 encoding exceeded 5 bytes");

            byte b = (byte)(value & SEGMENT_BITS);
            value >>>= 7;

            if (value != 0)
                b |= CONTINUE_BIT;

            bytes.Add(b);
            count++;
        }
        while (value != 0);

        return bytes.ToArray();
    }

    public static byte[] Encode(long value)
    {
        List<byte> bytes = new List<byte>();
        int count = 0;

        do
        {
            if (count >= 10)
                throw new OverflowException("ULEB128 encoding exceeded 10 bytes");

            byte b = (byte)(value & SEGMENT_BITS);
            value >>= 7;

            if (value != 0)
                b |= CONTINUE_BIT;

            bytes.Add(b);
            count++;
        }
        while (value != 0);

        return bytes.ToArray();
    }
}