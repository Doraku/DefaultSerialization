using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace DefaultSerialization.Internal.TextSerializer
{
    internal sealed class InvariantCultureStreamWriter : StreamWriter
    {
        public InvariantCultureStreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
            : base(stream, encoding, bufferSize, leaveOpen)
        { }

        public override IFormatProvider FormatProvider => CultureInfo.InvariantCulture;
    }
}
