﻿using System.Xml;

namespace S1XViewer.Types.Interfaces
{
    public interface ISeaPlaneLandingArea : ILayout
    {
        IDepthsDescription DepthsDescription { get; set; }
        string ISPSLevel { get; set; }
        string LocationByText { get; set; }
        IMarkedBy MarkedBy { get; set; }    }
}