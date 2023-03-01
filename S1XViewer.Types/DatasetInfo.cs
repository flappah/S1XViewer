using System.ComponentModel;

namespace S1XViewer.Types
{
    public class DatasetInfo
    {
        [DisplayName("Filename")]
        public string FileName { get; set; }

        [DisplayName("Start time dataset")]
        public string DateTimeStart { get; set; }

        [DisplayName("End time dataset")]
        public string DateTimeEnd { get; set; } 
    }
}
