using S1XViewer.Types.Interfaces;
using System.Xml;

namespace S1XViewer.Types.ComplexTypes
{
    public class ProducingAgency : ComplexTypeBase, IProducingAgency
    {
        public string IndividualName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public IContactAddress ContactAddress { get; set; } = new ContactAddress();
        public IOnlineResource OnlineResource { get; set; } = new OnlineResource();
        public ITelecommunications Telecommunications { get; set; } = new Telecommunications();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IComplexType DeepClone()
        {
            return new ProducingAgency
            {
                IndividualName = IndividualName,
                OrganizationName = OrganizationName,
                PositionName = PositionName,
                ContactAddress = ContactAddress == null
                    ? new ContactAddress()
                    : ContactAddress.DeepClone() as IContactAddress,
                OnlineResource = OnlineResource == null
                    ? new OnlineResource()
                    : OnlineResource.DeepClone() as IOnlineResource,
                Telecommunications = Telecommunications == null
                    ? new Telecommunications()
                    : Telecommunications.DeepClone() as ITelecommunications
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public override IComplexType FromXml(XmlNode node, XmlNamespaceManager mgr, string nameSpacePrefix = "")
        {
            var individualNameNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}individualName", mgr);
            if (individualNameNode != null && individualNameNode.HasChildNodes)
            {
                IndividualName = individualNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            var organizationNameNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}organizationName", mgr);
            if (organizationNameNode != null && organizationNameNode.HasChildNodes)
            {
                OrganizationName = organizationNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            var positionNameNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}positionName", mgr);
            if (positionNameNode != null && positionNameNode.HasChildNodes)
            {
                PositionName = positionNameNode.FirstChild?.InnerText ?? string.Empty;
            }

            var contactAddressNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}contactAddress", mgr);
            if (contactAddressNode != null && contactAddressNode.HasChildNodes)
            {
                ContactAddress = new ContactAddress();
                ContactAddress.FromXml(contactAddressNode, mgr, nameSpacePrefix);
            }

            var onlineResourceNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}onlineResource", mgr);
            if (onlineResourceNode != null && onlineResourceNode.HasChildNodes)
            {
                OnlineResource = new OnlineResource();
                OnlineResource.FromXml(onlineResourceNode, mgr, nameSpacePrefix);
            }

            var telecommunicationsNode = node.SelectSingleNode($"{(String.IsNullOrEmpty(nameSpacePrefix) ? "" : $"{nameSpacePrefix}:")}telecommunications", mgr);
            if (telecommunicationsNode != null && telecommunicationsNode.HasChildNodes)
            {
                Telecommunications = new Telecommunications();
                Telecommunications.FromXml(telecommunicationsNode, mgr, nameSpacePrefix);
            }

            return this;
        }
    }
}
