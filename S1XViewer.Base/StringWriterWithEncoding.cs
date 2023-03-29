using System.Text;

namespace S1XViewer.Base
{
    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        #region Constructors 

        public StringWriterWithEncoding() { }
        public StringWriterWithEncoding(IFormatProvider formatProvider) : base(formatProvider) { }
        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider) { }
        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }
        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            _encoding = encoding;
        }
        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding) : base(sb, formatProvider)
        {
            _encoding = encoding;
        }

        #endregion

        public override Encoding Encoding
        {
            get { return (null == _encoding ? base.Encoding : _encoding); }
        }
    }
}
