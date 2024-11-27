using S1XViewer.Base;
using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using S1XViewer.Types.Links;
using System.Data;
using System.Reflection;
using System.Xml;

namespace S1XViewer.Types
{
    public abstract class FeatureBase : IFeature
    {
        /* Viewer specific attributes */
        public bool FeatureToolWindow { get; set; } = false;
        public string FeatureToolWindowTemplate { get; set; } = string.Empty;

        /* S100 specific */
        public IFeatureObjectIdentifier FeatureObjectIdentifier { get; set; } = new FeatureObjectIdentifier();
        public string Id { get; set; } = string.Empty;

        /* for linking */
        public ILink[] Links { get; set; } = Array.Empty<Link>();

        public abstract IFeature DeepClone();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public virtual IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node == null)
                return this;

            if (mgr == null)
                return this;

            if (node.HasChildNodes)
            {
                if (node.Attributes?.Count > 0 && node.Attributes.Contains("gml:id") == true)
                {
                    Id = node.Attributes["gml:id"]?.InnerText ?? string.Empty;
                }
            }

            var featureObjectIdentifierNode = node.SelectSingleNode("S100:featureObjectIdentifier", mgr);
            if (featureObjectIdentifierNode != null && featureObjectIdentifierNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(featureObjectIdentifierNode, mgr);
            }

            var foidNode = node.SelectSingleNode("S100:featureObjectIdentifier", mgr);
            if (foidNode != null && foidNode.HasChildNodes)
            {
                FeatureObjectIdentifier = new FeatureObjectIdentifier();
                FeatureObjectIdentifier.FromXml(foidNode, mgr);
            }

            var linkNodes = node.SelectNodes("*[boolean(@xlink:href)]", mgr);
            if (linkNodes != null && linkNodes.Count > 0)
            {
                var links = new List<Link>();
                foreach (XmlNode linkNode in linkNodes)
                {
                    var newLink = new Link();
                    newLink.FromXml(linkNode, mgr);
                    links.Add(newLink);
                }
                Links = links.ToArray();
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                propertyInfo.SetValue(this, null);
            }
        }

        /// <summary>
        ///     Returns the properties of the current object in a DataTable
        /// </summary>
        /// <returns>Dictionary<string, string></returns>
        public DataTable GetData()
        {
            var results = new DataTable($"Results_{this.GetHashCode()}");
            results.Columns.AddRange(new DataColumn[]
            {
                new DataColumn
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = "Name"
                },
                 new DataColumn
                {
                    DataType = System.Type.GetType("System.String"),
                    ColumnName = "Value"
                }
            });

            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.FullName.Contains(GetType().FullName.Substring(0, GetType().FullName.IndexOf("."))))
                {
                    if (propertyInfo.PropertyType.IsArray)
                    {
                        var objs = GetPropertyValue(propertyInfo, propertyInfo.Name);
                        if (objs is IComplexType[])
                        {
                            foreach (IComplexType obj in (IComplexType[])objs)
                            {
                                DataTable childResults = obj.GetData();
                                foreach (DataRow childRow in childResults.Rows)
                                {
                                    var row = results.NewRow();
                                    row["Name"] = $"{propertyInfo.Name}.{childRow["Name"]}";
                                    row["Value"] = childRow["Value"];
                                    results.Rows.Add(row);
                                }
                            }
                        }
                        else if (objs is ILink[])
                        {
                            foreach (ILink obj in (ILink[])objs)
                            {
                                if (obj.LinkedFeature != null)
                                {
                                    // links are only evaluated to ONE nested level deep! 
                                    // TODO: make it recursive so multiple levels can be used. Consider circular
                                    //       relations (if any exist).
                                    var linkedFeatureData = obj.LinkedFeature.GetData();
                                    foreach (DataRow linkedRow in linkedFeatureData.Rows)
                                    {
                                        var row = results.NewRow();
                                        row["Name"] = $"{obj.LinkedFeature.GetType().ToString().LastPart(".")}.{linkedRow["Name"]}";
                                        row["Value"] = linkedRow["Value"];
                                        results.Rows.Add(row);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var obj = GetPropertyValue(propertyInfo, propertyInfo.Name) as IComplexType;
                        if (obj != null)
                        {
                            DataTable childResults = obj.GetData();
                            foreach (DataRow childRow in childResults.Rows)
                            {
                                var row = results.NewRow();
                                row["Name"] = $"{propertyInfo.Name}.{childRow["Name"]}";
                                row["Value"] = childRow["Value"];
                                results.Rows.Add(row);
                            }
                        }
                    }
                }
                else if (propertyInfo.PropertyType.IsDictionary())
                {
                    var values = GetPropertyValue(propertyInfo, propertyInfo.Name) as Dictionary<string, string>;
                    if (values != null && values.Count > 0)
                    {
                        var listOfStrings = new List<string>();
                        foreach (var value in values)
                        {
                            listOfStrings.Add($"{value.Key}={value.Value}");
                        }

                        var row = results.NewRow();
                        row["Name"] = $"{propertyInfo.Name}";
                        row["Value"] = listOfStrings.Join(",");
                        results.Rows.Add(row);
                    }
                }
                else
                {
                    var value = GetPropertyValue(propertyInfo, propertyInfo.Name);
                    if (value != null)
                    {
                        DataRow row = null;
                        if (propertyInfo.PropertyType.IsArray)
                        {
                            string[] arrayAsStrings;
                            if (value is double[])
                            {
                                arrayAsStrings = Array.ConvertAll((double[])value, v => v.ToString() ?? "");
                            }
                            else if (value is int[])
                            {
                                arrayAsStrings = Array.ConvertAll((int[])value, v => v.ToString() ?? "");
                            }
                            else if (value is float[])
                            {
                                arrayAsStrings = Array.ConvertAll((float[])value, v => v.ToString() ?? "");
                            }
                            else if (value is DateTime[])
                            {
                                arrayAsStrings = Array.ConvertAll((DateTime[])value, v => v.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                            }
                            else
                            {
                                arrayAsStrings = Array.ConvertAll((object[])value, v => v.ToString() ?? "");
                            }

                            if (arrayAsStrings.Length > 0)
                            {
                                row = results.NewRow();
                                row["Name"] = propertyInfo.Name;
                                row["Value"] = String.Join(",", arrayAsStrings);
                            }
                        }
                        else
                        {
                            bool rowEmpty = false;
                            if (value is int)
                            {
                                rowEmpty = ((int)value) == int.MinValue;
                            }
                            else if (value is double)
                            {
                                rowEmpty = ((double)value) == double.NaN;
                            }
                            else
                            {
                                rowEmpty = String.IsNullOrEmpty(value.ToString());
                            }

                            if (!rowEmpty)
                            {
                                row = results.NewRow();
                                row["Name"] = propertyInfo.Name;
                                row["Value"] = value.ToString();
                            }
                        }

                        if (row != null)
                        {
                            results.Rows.Add(row);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcobj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object GetPropertyValue(PropertyInfo pi, string propertyName)
        {
            if (pi == null)
                return null;

            // Split property name to parts (propertyName could be hierarchical, like obj.subobj.subobj.property
            string[] propertyNameParts = propertyName.Split('.');

            foreach (string propertyNamePart in propertyNameParts)
            {                
                // propertyNamePart could contain reference to specific 
                // element (by index) inside a collection
                if (!propertyNamePart.Contains("["))
                {                    
                    if (pi == null) return null;
                    return pi.GetValue(this, null);
                }
                else
                {   // propertyNamePart is areference to specific element 
                    // (by index) inside a collection
                    // like AggregatedCollection[123]
                    //   get collection name and element index
                    int indexStart = propertyNamePart.IndexOf("[") + 1;
                    string collectionPropertyName = propertyNamePart.Substring(0, indexStart - 1);
                    int collectionElementIndex = Int32.Parse(propertyNamePart.Substring(indexStart, propertyNamePart.Length - indexStart - 1));
                    //   get collection object
                    
                    if (pi == null) return null;
                    object unknownCollection = pi.GetValue(this, null);
                    //   try to process the collection as array
                    if (unknownCollection.GetType().IsArray)
                    {
                        object[] collectionAsArray = unknownCollection as Array[];
                        return collectionAsArray[collectionElementIndex];
                    }
                    else
                    {
                        //   try to process the collection as IList
                        System.Collections.IList collectionAsList = unknownCollection as System.Collections.IList;
                        if (collectionAsList != null)
                        {
                            return collectionAsList[collectionElementIndex];
                        }
                        else
                        {
                            // ??? Unsupported collection type
                        }
                    }
                }
            }

            return "";
        }
    }
}
