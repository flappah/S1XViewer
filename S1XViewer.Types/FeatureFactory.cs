﻿using S1XViewer.Base;
using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types
{
    public class FeatureFactory : IFeatureFactory
    {        /// <summary>
             ///     Uses Autofac to insert all existing IFeature's in this property. 
             /// </summary>
        public S1XViewer.Types.Interfaces.IFeature[] Features { get; set; } = new IFeature[0];

        /// <summary>
        ///     No dependencies
        /// </summary>
        public FeatureFactory() { }

        /// <summary>
        ///     Uses specified XMLNode to determine type of feature to create.
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="mgr">XmlNamespaceManager</param>
        /// <returns>IFeature</returns>
        public S1XViewer.Types.Interfaces.IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            var featureTypeString = "";
            if (node != null)
            {
                // determine the typestring of the feature we're looking for
                featureTypeString = (node.HasChildNodes ? node.ChildNodes[0].Name : "").LastPart(char.Parse(":"));

                // look for the feature in the collection of features Autofac initialized and inserted in the Features property
                var locatedFeature =
                    Features.ToList().Find(tp => tp.GetType().Name.Equals(featureTypeString));

                // if there's a feature, start XML parsing it and return the feature
                if (locatedFeature != null)
                {
                    // just to make sure to have a copy of the autofac feature
                    if (locatedFeature.DeepClone() is S1XViewer.Types.Interfaces.IFeature clonedFeature)
                    {
                        // clear the feature of the original content
                        clonedFeature.Clear();
                        // and parse xml content into it
                        clonedFeature.FromXml(node, mgr);
                        return clonedFeature;
                    }
                }
            }

            throw new Exception($"Featuretype '{featureTypeString}' not found!");
        }
    }
}
