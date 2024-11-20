using S1XViewer.Types.ComplexTypes;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace S1XViewer.Types.Features
{
    public class CatalogueSectionHeader : InformationFeatureBase, IS128Feature, ICatalogueSectionHeader
    {
        public int CatalogueSectionNumber { get; set; }
        public string CatalogueSectionTitle { get; set; } = string.Empty;
        public IInformation Information { get; set; } = new Information();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IFeature DeepClone()
        {
            return new CatalogueSectionHeader
            {
                CatalogueSectionNumber = CatalogueSectionNumber,
                CatalogueSectionTitle = CatalogueSectionTitle,
                Information = Information == null
                    ? new Information()
                    : Information.DeepClone() as IInformation
            };
        }

        public override IFeature FromXml(XmlNode node, XmlNamespaceManager mgr)
        {
            throw new NotImplementedException();
        }
    }
}
