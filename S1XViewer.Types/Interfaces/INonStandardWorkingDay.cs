using System;
using S1XViewer.Types.Interfaces;

namespace S1XViewer.Types.Interfaces
{
    public interface INonStandardWorkingDay : IInformationFeature
    {
        string[] DateFixed { get; set; }
        string[] DateVariable { get; set; }
        IInformation[] Information { get; set; }
    }
}