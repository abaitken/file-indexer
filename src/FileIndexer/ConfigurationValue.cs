using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileIndexer
{
    public class ConfigurationValue
    {
        [Key]
        [Required]
        public string Name { get; set; }

        public string StringValue { get; set; }
        public double? FloatingValue { get; set; }
        public int? IntegerValue { get; set; }
        public int? BooleanValue { get; set; }

        [NotMapped]
        public bool? BooleanValueBool
        {
            get => BooleanValue.HasValue && BooleanValue.Value == 1;
            set => BooleanValue = value.HasValue && value.Value ? 1 : 0;
        }
    }
}
