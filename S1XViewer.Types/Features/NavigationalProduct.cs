using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S1XViewer.Types.Features
{
    public abstract class NavigationalProduct
    {
        public int ApproximateGridResolution { get; set; }   
        public int CompilationScale { get; set; }
        public string DistributionStatus { get; set; } = string.Empty;
        public int EditionNumber { get; set; }
        public int MaximumDisplayScale { get; set; }
        public int MinimumDisplayScale { get; set; }
        public string NavigationPurpose { get; set; } = string.Empty;
        public int OptimumDisplayScale { get; set; }
        public string OriginalProductNumber { get; set; } = string.Empty;
        public string ProducerNation { get; set; } = string.Empty;
        public string ProductNumber { get; set; } = string.Empty;
        public string SpecificUsage { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }
        public int UpdateNumber { get; set; }
    }
}
