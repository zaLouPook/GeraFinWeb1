using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.Importer
{
    public partial class UpdateFilesUploaded
    {
        [Key]
        public int FileUploadId { get; set; }
        public string FileName { get; set; }
        public DateTime FileUploadDate { get; set; }
        public int FileRecordCount { get; set; }
        public int FileRecordsProcessed { get; set; }
        public bool IsApproved { get; set; }
        public string UploadedBy { get; set; }
        public string ApprovedBy { get; set; }
    }
}
