using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalDatasetManagementAPI.Models
{
    public class Sample
    {
        [Key]
        [Column("SAMPLE_ID")]
        public int SampleID { get; set; }
        [Column("DATASET_ID")]
        public string DatasetID { get; set; }
        [Column("SAMPLE_TYPE")]
        public int SampleType { get; set; }
        [Column("METADATA")]
        public string Metadata { get; set; }
        [Column("IMAGE_COUNT")]
        public int ImageCount { get; set; }

        public ICollection<Image> Images { get; set; } = new List<Image>();

        public bool ShouldSerializeMetadata()
        {
            return !String.IsNullOrWhiteSpace(this.Metadata);
        }
    }
}
