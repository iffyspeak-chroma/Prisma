using System.Buffers.Binary;
using System.Text;
using API.DataTypes;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace API.Networking
{
    public class Packet : IDisposable
    {
        private List<byte> _buffer;
        private byte[]? _readableBuffer;
        private int _readPos;

        // ReSharper disable InconsistentNaming
        private const int SEGMENT_BITS = 0x7F;
        private const int CONTINUE_BIT = 0x80;
        // ReSharper restore InconsistentNaming

        /// <summary>Creates a new empty packet (without an ID).</summary>
        public Packet()
        {
            _buffer = new List<byte>(); // Initialize buffer
            _readPos = 0; // Set readPos to 0
        }

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="id">The packet ID.</param>
        public Packet(int id)
        {
            _buffer = new List<byte>(); // Initialize buffer
            _readPos = 0; // Set readPos to 0

            Write(id); // Write packet id to the buffer
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public Packet(byte[] data)
        {
            _buffer = new List<byte>(); // Initialize buffer
            _readPos = 0; // Set readPos to 0

            SetBytes(data);
        }

        #region Functions

        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        private void SetBytes(byte[] data)
        {
            Write(data);
            _readableBuffer = _buffer.ToArray();
        }

        /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
        /// <param name="useVariableLength">The bytes to add to the packet.</param>
        public void WriteLength(bool useVariableLength = true)
        {
            if (!useVariableLength)
            {
                _buffer.InsertRange(0,
                    BitConverter.GetBytes(_buffer.Count)); // Insert the byte length of the packet at the very beginning
                return;
            }
            
            _buffer.InsertRange(0, VariableLength.Encode(_buffer.Count));
        }

        public void SkipBytes(int amount)
        {
            _readPos += amount;
        }

        /// <summary>Inserts the given int at the start of the buffer.</summary>
        /// <param name="value">The int to insert.</param>
        /// <param name="asVarInt">Write as a variable length.</param>
        public void InsertInt(int value, bool asVarInt = true)
        {
            if (!asVarInt)
            {
                _buffer.InsertRange(0, BitConverter.GetBytes(value)); // Insert the int at the start of the buffer
                return;
            }
            
            _buffer.InsertRange(0, VariableLength.Encode(value));
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[]? ToArray()
        {
            _readableBuffer = _buffer.ToArray();
            return _readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        private int Length()
        {
            return _buffer.Count; // Return the length of buffer
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - _readPos; // Return the remaining length (unread)
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="shouldReset">Whether to reset the packet.</param>
        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                _buffer.Clear(); // Clear buffer
                _readableBuffer = null;
                _readPos = 0; // Reset readPos
            }
            else
            {
                _readPos -= 4; // "Unread" the last read int
            }
        }

        /// <summary>Moves the "read head" of the packet.</summary>
        /// <param name="position">Where to set the "read head"</param>
        public void SetReadPos(int position)
        {
            _readPos = position;
        }

        #endregion

        #region Write Data

        /// <summary>Adds a byte to the packet.</summary>
        /// <param name="value">The byte to add.</param>
        public void Write(byte value)
        {
            _buffer.Add(value);
        }

        /// <summary>Adds an array of bytes to the packet.</summary>
        /// <param name="value">The byte array to add.</param>
        public void Write(byte[] value)
        {
            _buffer.AddRange(value);
        }

        /// <summary>Adds a short to the packet.</summary>
        /// <param name="value">The short to add.</param>
        public void Write(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds an int to the packet.</summary>
        /// <param name="value">The int to add.</param>
        /// <param name="asVarInt">Should the value be added as a variable length integer</param>
        public void Write(int value, bool asVarInt = true)
        {
            if (!asVarInt)
            {
                _buffer.AddRange(BitConverter.GetBytes(value));
                return;
            }
            
            _buffer.AddRange(VariableLength.Encode(value));
        }

        /// <summary>Adds a long to the packet.</summary>
        /// <param name="value">The long to add.</param>
        /// <param name="asVarLong">If the value should be written as a variable length</param>
        public void Write(long value, bool asVarLong = true)
        {
            if (!asVarLong)
            {
                _buffer.AddRange(BitConverter.GetBytes(value));
                return;
            }
            
            _buffer.AddRange(VariableLength.Encode(value));
        }

        /// <summary>Adds a float to the packet.</summary>
        /// <param name="value">The float to add.</param>
        public void Write(float value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds a bool to the packet.</summary>
        /// <param name="value">The bool to add.</param>
        public void Write(bool value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds a string to the packet.</summary>
        /// <param name="value">The string to add.</param>
        public void Write(string value)
        {
            Write(value.Length); // Add the length of the string to the packet
            _buffer.AddRange(Encoding.UTF8.GetBytes(value)); // Add the string itself
        }
        
        /// <summary>Adds a Guid to the packet</summary>
        /// <param name="guid">The Guid to add.</param>
        /// <param name="flipped">Write with bytes flipped</param>
        public void Write(Guid guid, bool flipped = true)
        {
            byte[] bytes = guid.ToByteArray();

            if (flipped)
            {
                Array.Reverse(bytes, 0, 4);
                Array.Reverse(bytes, 4, 2);
                Array.Reverse(bytes, 6, 2); 
            }

            _buffer.AddRange(bytes);
        }
        
        /// <summary>Adds a double to the packet.</summary>
        /// <param name="value">The string to add.</param>
        public void Write(double value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        #endregion

        #region Read Data

        /// <summary>Reads a byte from the packet.</summary>
        /// <param name="moveReadPos">Whether to move the buffer's read position.</param>
        public byte ReadByte(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                byte value = _readableBuffer[_readPos]; // Get the byte at readPos' position
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (moveReadPos)
                {
                    // If moveReadPos is true
                    _readPos += 1; // Increase readPos by 1
                }

                return value; // Return the byte
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        /// <summary>Reads an array of bytes from the packet.</summary>
        /// <param name="length">The length of the byte array.</param>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes
                byte[] value = _buffer.GetRange(_readPos, length)
                    .ToArray(); // Get the bytes at readPos position with a range of _length
                if (moveReadPos)
                {
                    // If moveReadPos is true
                    _readPos += length; // Increase readPos by _length
                }

                return value; // Return the bytes
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        /// <summary>Reads a short from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        /// <param name="flipped">If the bytes were sent in reverse order.</param>
        public short ReadShort(bool moveReadPos = true, bool flipped = false)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes

                short value = BitConverter.ToInt16(_readableBuffer, _readPos); // Convert the bytes to a short

                if (moveReadPos)
                {
                    // If moveReadPos is true and there are unread bytes
                    _readPos += 2; // Increase readPos by 2
                }

                if (flipped)
                {
                    value = BinaryPrimitives.ReverseEndianness(value);
                }

                return value; // Return the short
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public int ReadInt(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes

                int value = BitConverter.ToInt32(_readableBuffer, _readPos); // Convert the bytes to an int

                if (moveReadPos)
                {
                    // If moveReadPos is true
                    _readPos += 4; // Increase readPos by 4
                }

                return value; // Return the int
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public int ReadVarInt(bool moveReadPos = true)
        {
            int value = 0;
            int position = 0;
            byte currentByte;

            while (true) {
                currentByte = ReadByte(moveReadPos);
                value |= (currentByte & SEGMENT_BITS) << position;

                if ((currentByte & CONTINUE_BIT) == 0) break;

                position += 7;

                if (position >= 32) throw new InvalidDataException("VarInt is too big");
            }

            return value;
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public long ReadLong(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes

                long value = BitConverter.ToInt64(_readableBuffer, _readPos); // Convert the bytes to a long

                if (moveReadPos)
                {
                    // If moveReadPos is true
                    _readPos += 8; // Increase readPos by 8
                }

                return value; // Return the long
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public long ReadVarLong(bool moveReadPos = true)
        {
            long value = 0;
            int position = 0;
            byte currentByte;

            while (true) {
                currentByte = ReadByte(moveReadPos);
                value |= (long) (currentByte & SEGMENT_BITS) << position;

                if ((currentByte & CONTINUE_BIT) == 0) break;

                position += 7;

                if (position >= 64) throw new InvalidDataException("VarLong is too big");
            }

            return value;
        }

        /// <summary>Reads a float from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public float ReadFloat(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes
                float value = BitConverter.ToSingle(_readableBuffer, _readPos); // Convert the bytes to a float
                if (moveReadPos)
                {
                    // If moveReadPos is true
                    _readPos += 4; // Increase readPos by 4
                }

                return value; // Return the float
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public bool ReadBool(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes
                bool value = BitConverter.ToBoolean(_readableBuffer, _readPos); // Convert the bytes to a bool
                if (moveReadPos)
                {
                    // If moveReadPos is true
                    _readPos += 1; // Increase readPos by 1
                }

                return value; // Return the bool
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        /// <summary>Reads a string from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        /// <param name="usesVariableLength">Is the string prefixed with a variable length integer</param>
        public string ReadString(bool moveReadPos = true, bool usesVariableLength = true)
        {
            int length;
            if (!usesVariableLength)
            {
                length = ReadInt();
            }
            else
            {
                length = ReadVarInt();
            }
            
            try
            {
                 // Get the length of the string
                string value =
                    Encoding.UTF8.GetString(_readableBuffer, _readPos, length); // Convert the bytes to a string
                if (moveReadPos && value.Length > 0)
                {
                    // If moveReadPos is true string is not empty
                    _readPos += length; // Increase readPos by the length of the string
                }

                return value; // Return the string
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        
        /// <summary>Reads a Guid from the packet that was written in Java-compatible byte order.</summary>
        /// <param name="moveReadPos">Whether to move the buffer's read position.</param>
        /// <param name="flipped">Read with bytes flipped</param>
        public Guid ReadGuid(bool moveReadPos = true, bool flipped = true)
        {
            const int guidLength = 16;

            if (_buffer.Count - _readPos < guidLength)
                throw new Exception("Could not read value of type 'Guid'! Not enough bytes.");

            byte[] bytes = ReadBytes(guidLength, moveReadPos);

            if (flipped)
            {
                Array.Reverse(bytes, 0, 4);
                Array.Reverse(bytes, 4, 2);
                Array.Reverse(bytes, 6, 2);
            }

            return new Guid(bytes);
        }
        
        /// <summary>Reads a double from the packet.</summary>
        /// <param name="moveReadPos">Move the buffer's read position.</param>
        public double ReadDouble(bool moveReadPos = true)
        {
            if (_buffer.Count > _readPos)
            {
                // If there are unread bytes
                double value = BitConverter.ToDouble(_readableBuffer, _readPos); // Convert the bytes to a double
                if (moveReadPos)
                {
                    // If moveReadPos is true
                    _readPos += 8; // Increase readPos by 4
                }

                return value; // Return the float
            }
            else
            {
                throw new Exception("Could not read value of type 'double'!");
            }
        }

        #endregion

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _buffer = null;
                    _readableBuffer = null;
                    _readPos = 0;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
