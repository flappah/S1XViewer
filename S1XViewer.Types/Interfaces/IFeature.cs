using System.Data;
using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface IFeature
    {
        #region Public Properties

        IFeatureObjectIdentifier FeatureObjectIdentifier { get; set; }
        string Id { get; set; }

        #endregion

        #region For Feature Linking 

        ILink[] Links { get; set; }

        #endregion

        #region For Programmatic Support

        bool FeatureToolWindow { get; set; }
        string FeatureToolWindowTemplate { get; set; }

        #endregion

        #region Methods

        void Clear();
        IFeature DeepClone();
        DataTable GetData();
        IFeature FromXml(XmlNode node, XmlNamespaceManager mgr);

        #endregion
    }
}
