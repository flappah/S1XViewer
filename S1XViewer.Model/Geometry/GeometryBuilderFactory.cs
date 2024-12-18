﻿using S1XViewer.Base;
using S1XViewer.Model.Interfaces;
using System.Xml;

namespace S1XViewer.Model.Geometry
{
    public class GeometryBuilderFactory : IGeometryBuilderFactory
    {
        public IGeometryBuilder[] Builders { get; set; }
        public bool InvertLonLat { get; set; } = false;
        public string DefaultCRS { get; set; } = string.Empty;
       
        /// <summary>
        ///     Creates an ESRI geometry based on the provided XML
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public Esri.ArcGISRuntime.Geometry.Geometry? Create(XmlNode? node, XmlNamespaceManager? mgr)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (mgr == null)
            {
                throw new ArgumentNullException(nameof(mgr));
            }

            string geometryTypeString;

            if (node.Name.Contains(true, "EnvelopeProperty", "CurveProperty", "PointProperty", "PolygonProperty", "SurfaceProperty"))
            {
                geometryTypeString = (node.HasChildNodes ? node.ChildNodes[0].Name : "").LastPart(char.Parse(":"));
            }
            else 
            {
                geometryTypeString = node.Name.LastPart(char.Parse(":"));
            }

            IGeometryBuilder? locatedBuilder =
                Builders.ToList().Find(tp => tp.GetType().ToString().Contains($"{geometryTypeString}Builder"));

            if (locatedBuilder != null)
            {
                locatedBuilder.InvertLonLat = InvertLonLat;
                locatedBuilder.DefaultCRS = DefaultCRS;
                Esri.ArcGISRuntime.Geometry.Geometry? result = locatedBuilder.FromXml(node, mgr);

                InvertLonLat = locatedBuilder.InvertLonLat;
                return result;
            }

            return null;
        }

        /// <summary> 
        ///     Creates an ESRI geometry based on the provided positions
        /// </summary>
        /// <param name="geometryTypeString"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="srs">horizontal crs</param>
        /// <returns></returns>
        public Esri.ArcGISRuntime.Geometry.Geometry? Create(string geometryTypeString, double[] x, double[] y, int srs = -1)
        {
            var locatedBuilder =
                Builders.ToList().Find(tp => tp.GetType().ToString().Contains($"{geometryTypeString}Builder"));

            if (locatedBuilder != null)
            {
                locatedBuilder.InvertLonLat = InvertLonLat;
                locatedBuilder.DefaultCRS = DefaultCRS;
                return locatedBuilder.FromPositions(x, y, srs);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryTypeString"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="srs"></param>
        /// <returns></returns>
        public Esri.ArcGISRuntime.Geometry.Geometry? Create(string geometryTypeString, double[] x, double[] y, double z, int srs = -1)
        {
            var locatedBuilder =
                 Builders.ToList().Find(tp => tp.GetType().ToString().Contains($"{geometryTypeString}Builder"));

            if (locatedBuilder != null)
            {
                locatedBuilder.InvertLonLat = InvertLonLat;
                locatedBuilder.DefaultCRS = DefaultCRS;
                return locatedBuilder.FromPositions(x, y, z, srs);
            }

            return null;
        }
    }
}
