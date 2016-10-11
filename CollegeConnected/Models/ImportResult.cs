namespace CollegeConnected.Models
{
    using System;
    using System.Collections.Generic;

    public partial class ImportResult
    {
        public short Id { get; set; }
        public string Type { get; set; }
        public byte[] ImportFile { get; set; }
        public byte[] RejectFile { get; set; }
        public short ImportCount { get; set; }
        public short ConvertCount { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string Errors { get; set; }
    }
}