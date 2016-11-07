using System;

namespace CollegeConnected.Models
{
    public class ImportResult
    {
        public short Id { get; set; }
        public string Type { get; set; }
        public byte[] ImportFile { get; set; }
        public byte[] RejectFile { get; set; }
        public short ImportCount { get; set; }
        public short ConvertCount { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Errors { get; set; }
    }
}