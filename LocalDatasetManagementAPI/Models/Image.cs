using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalDatasetManagementAPI.Models
{
    public class Image
    {
        [Key]
        [Column("IMAGE_ID")]
        public string ImageID { get; set; }

        [Column("SAMPLE_ID")]
        public int SampleID { get; set; }

        [Required]
        [Column("IMAGE_NO")]
        public int ImageNO { get; set; }

        [Column("IMAGE_CODE")]
        public string ImageCode { get; set; }

        [Column("ORIGINAL_FILENAME")]
        public string OriginalFilename { get; set; }

        [Column("IMAGE_SCHEMA")]
        public string ImageScheme { get; set; }

        [ForeignKey("SampleID")]
        public Sample Sample { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
