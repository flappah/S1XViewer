using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace S1XViewer.Types
{
    public class ColorSchemeRangeItem : IXmlSerializable
    {
        public int Id { get; set; }
        public System.Windows.Media.Color Color { get; set; } 
        public double? Min { get; set; }
        public bool MinInclusive { get; set; }
        public double? Max { get; set; }
        public bool MaxInclusive { get; set; }

        /// <summary>
        ///     Evaluates if given value is between Max and Min and takes inclusiveness under consideration
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Between (double value)
        {
            lock (this)
            {
                var minResult = Min == null;
                if (Min != null)
                {
                    if (MinInclusive && value >= Min)
                    {
                        minResult = true;
                    }
                    else if (MinInclusive == false && value > Min)
                    {
                        minResult = true;
                    }
                }

                var maxResult = Max == null;
                if (Max != null)
                {
                    if (MaxInclusive && value <= Max)
                    {
                        maxResult = true;
                    }
                    else if (MaxInclusive == false && value < Max)
                    {
                        maxResult = true;
                    }
                }

                return minResult && maxResult;
            }
        }

        public XmlSchema? GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Parses the XML data
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ColorSchemeRangeItem Parse(XmlNode? node)
        {
            if (node != null)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;

                var minNode = node.SelectSingleNode("Min");
                Min = null;
                if (minNode != null)
                {
                    if (double.TryParse(minNode.InnerText.Replace(minNode.InnerText.Contains('.') ? "." : ",", currentCulture.NumberFormat.NumberDecimalSeparator), out double minValue))
                    {
                        Min = minValue;
                    }

                    if (minNode.Attributes["inclusive"] != null)
                    {
                        if (bool.TryParse(minNode.Attributes["inclusive"].InnerText, out bool minInclusive))
                        {
                            MinInclusive = minInclusive;
                        }
                    }
                }

                var maxNode = node.SelectSingleNode("Max");
                Max = null;
                if (maxNode != null)
                {
                    if (double.TryParse(maxNode.InnerText.Replace(maxNode.InnerText.Contains('.') ? "." : ",", currentCulture.NumberFormat.NumberDecimalSeparator), out double maxValue))
                    {
                        Max = maxValue;
                    }

                    if (maxNode.Attributes["inclusive"] != null)
                    {
                        if (bool.TryParse(maxNode.Attributes["inclusive"].InnerText, out bool maxInclusive))
                        {
                            MaxInclusive = maxInclusive;
                        }
                    }
                }

                var colorNode = node.SelectSingleNode("Color"); 
                if (colorNode != null)
                {
                    int a = -1;
                    if (colorNode.Attributes["a"] != null)
                    {
                        if (int.TryParse(colorNode.Attributes["a"].InnerText, out int aValue))
                        {
                            a = aValue;
                        }
                    }

                    int r = -1;
                    if (colorNode.Attributes["r"] != null)
                    {
                        if (int.TryParse(colorNode.Attributes["r"].InnerText, out int rValue))
                        {
                            r = rValue;
                        }
                    }

                    int g = -1;
                    if (colorNode.Attributes["g"] != null)
                    {
                        if (int.TryParse(colorNode.Attributes["g"].InnerText, out int gValue))
                        {
                            g = gValue;
                        }
                    }
                    int b = -1;
                    if (colorNode.Attributes["b"] != null)
                    {
                        if (int.TryParse(colorNode.Attributes["b"].InnerText, out int bValue))
                        {
                            b = bValue;
                        }
                    }

                    if (r != -1 && g != -1 && b != -1)
                    {
                        Color = System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b);
                    }

                    if (a != -1)
                    {
                        Color = System.Windows.Media.Color.FromArgb((byte)a, (byte)r, (byte)g, (byte) b);
                    }
                }
            }

            return this;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Writes Xml
        /// </summary>
        /// <param name="writer"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Range");

            if (Min != null)
            {
                writer.WriteStartElement("Min");
                writer.WriteAttributeString("inclusive", MinInclusive ? "true" : "false");

                writer.WriteString(Min.ToString().Replace(",", "."));
                writer.WriteEndElement();
            }

            if (Max != null)
            {
                writer.WriteStartElement("Max");
                writer.WriteAttributeString("inclusive", MaxInclusive ? "true" : "false");  
              
                writer.WriteString(Max.ToString().Replace(",", "."));
                writer.WriteEndElement();
            }

            writer.WriteStartElement("Color");
            writer.WriteAttributeString("a", Color.A.ToString() ?? "");
            writer.WriteAttributeString("r", Color.R.ToString());
            writer.WriteAttributeString("g", Color.G.ToString());
            writer.WriteAttributeString("b", Color.B.ToString());

            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
