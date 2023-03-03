using Esri.ArcGISRuntime.Geometry;
using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using S1XViewer.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class PolygonBuilder : GeometryBuilderBase, IPolygonBuilder
    {
        /// <summary>
        ///     For injection purposes
        /// </summary>
        /// <param name="optionsStorage"></param>
        public PolygonBuilder(IOptionsStorage optionsStorage)
        {
            _optionsStorage = optionsStorage;
        }

        /// <summary>
        ///     Retrieves the geometry from the specified Xml Node
        /// </summary>
        /// <param name="node">node containing a basic geometry</param>
        /// <param name="mgr">namespace manager</param>
        /// <returns>ESRI Arc GIS geometry</returns>
        public override Esri.ArcGISRuntime.Geometry.Geometry FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            if (node != null && node.HasChildNodes)
            {
                XmlNode srsNode = null;
                if (node.Attributes.Count > 0 && node.Attributes[0].Name == "srsName")
                {
                    srsNode = node;
                }
                else if (node.FirstChild.Attributes.Count > 0 && node.FirstChild.Attributes[0].Name == "srsName")
                {
                    srsNode = node.FirstChild;
                }

                if (srsNode != null)
                {
                    if (!int.TryParse(srsNode.Attributes[0].Value.ToString().LastPart(char.Parse(":")), out int refSystem))
                    {
                        refSystem = 0;
                    }
                    _spatialReferenceSystem = refSystem;
                }

                if (_spatialReferenceSystem == 0)
                {
                    string defaultCRS = _optionsStorage.Retrieve("comboBoxCRS");
                    if (int.TryParse(defaultCRS, out int defaultCRSValue))
                    {
                        _spatialReferenceSystem = defaultCRSValue; // if no srsNode is found assume default reference systema
                    }
                    else
                    {
                        _spatialReferenceSystem = 4326; // since most S1xx standards assume WGS84 is default, use this is the uber default CRS
                    }
                }

                string invertLatLonString = _optionsStorage.Retrieve("checkBoxInvertLatLon");
                if (!bool.TryParse(invertLatLonString, out bool invertLatLon))
                {
                    invertLatLon = false;
                }

                // parse the exterior linearring
                var spatialReferenceSystem = SpatialReference.Create(_spatialReferenceSystem);
                var currentCulture = Thread.CurrentThread.CurrentCulture;

                var segments = new List<List<MapPoint>>();
                var exteriorNode = node["gml:exterior"];
                if (exteriorNode != null && exteriorNode.HasChildNodes)
                {
                    var exteriorMapPoints = new List<MapPoint>();
                    var linearRingNodes = exteriorNode.ChildNodes;
                    foreach (XmlNode linearRingNode in linearRingNodes)
                    {
                        if (linearRingNode.HasChildNodes &&
                            linearRingNode.ChildNodes[0].Name.ToUpper().Contains("POSLIST"))
                        {
                            string[] splittedPositionArray =
                                linearRingNode.ChildNodes[0].InnerText
                                    .Replace("\t", " ")
                                    .Replace("\n", " ")
                                    .Replace("\r", " ")
                                    .Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                            if (((double)splittedPositionArray.Length / 2.0) == Math.Abs(splittedPositionArray.Length / 2.0))
                            {
                                var latitudes = new double[splittedPositionArray.Length];
                                var longitudes = new double[splittedPositionArray.Length];
                                
                                Parallel.For(0, splittedPositionArray.Length, index =>
                                {
                                    // try to avoid this overload since this one is quite a bit slower than the simple TryParse! 
                                    //if (double.TryParse(splittedPositionArray[index], NumberStyles.Float, new CultureInfo("en-US"), out double positionValue) == true)
                                    if (double.TryParse(
                                        splittedPositionArray[index].Replace(splittedPositionArray[index].Contains('.') ? "." : ",", currentCulture.NumberFormat.NumberDecimalSeparator), 
                                        out double positionValue) == true)
                                    {
                                        if (((BigInteger)index).IsEven)
                                        {
                                            latitudes[index] = positionValue;
                                        }
                                        else
                                        {
                                            longitudes[index] = positionValue;
                                        }
                                    }
                                });

                                for (int i = 0; i < latitudes.Length; i += 2)
                                {
                                    if (invertLatLon)
                                    {
                                        exteriorMapPoints.Add(new MapPoint(longitudes[i + 1], latitudes[i], spatialReferenceSystem));
                                    }
                                    else
                                    {
                                        exteriorMapPoints.Add(new MapPoint(latitudes[i], longitudes[i + 1], spatialReferenceSystem));
                                    }
                                }

                                longitudes = null;
                                latitudes = null;
                            }
                        }

                        segments.Add(exteriorMapPoints);
                    }
                }

                var interiorNode = node["gml:interior"];
                if (interiorNode != null && interiorNode.HasChildNodes)
                {
                    var interiorMapPoints = new List<MapPoint>();
                    var linearRingNodes = interiorNode.ChildNodes;
                    foreach (XmlNode linearRingNode in linearRingNodes)
                    {
                        if (linearRingNode.HasChildNodes &&
                            linearRingNode.ChildNodes[0].Name.ToUpper().Contains("POSLIST"))
                        {
                            string[] splittedPositionArray =
                                linearRingNode.ChildNodes[0].InnerText
                                    .Replace("\t", " ")
                                    .Replace("\n", " ")
                                    .Replace("\r", " ")
                                    .Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

                            if ((splittedPositionArray.Length / 2.0) == Math.Abs(splittedPositionArray.Length / 2.0))
                            {
                                var latitudes = new double[splittedPositionArray.Length];
                                var longitudes = new double[splittedPositionArray.Length];

                                Parallel.For(0, splittedPositionArray.Length, index =>
                                {
                                    // try to avoid this overload since this one is quite a bit slower than the simple TryParse!
                                    //if (double.TryParse(splittedPositionArray[index], NumberStyles.Float, new CultureInfo("en-US"), out double positionValue) == true)
                                    if (double.TryParse(
                                        splittedPositionArray[index].Replace(splittedPositionArray[index].Contains('.') ? "." : ",", currentCulture.NumberFormat.NumberDecimalSeparator),
                                        out double positionValue) == true)
                                    {
                                        if (((BigInteger)index).IsEven)
                                        {
                                            latitudes[index] = positionValue;
                                        }
                                        else
                                        {
                                            longitudes[index] = positionValue;
                                        }
                                    }
                                });

                                for (int i = 0; i < latitudes.Length; i += 2)
                                {
                                    if (invertLatLon)
                                    {
                                        interiorMapPoints.Add(new MapPoint(longitudes[i + 1], latitudes[i], spatialReferenceSystem));
                                    }
                                    else
                                    {
                                        interiorMapPoints.Add(new MapPoint(latitudes[i], longitudes[i + 1], spatialReferenceSystem));
                                    }
                                }

                                longitudes = null;
                                latitudes = null;
                            }
                        }

                        segments.Add(interiorMapPoints);
                    }
                }

                if (segments.Count > 0)
                {
                    var polygon = new Polygon(segments, spatialReferenceSystem);
                    return polygon;
                }
            }

            return null;
        }
    }
}