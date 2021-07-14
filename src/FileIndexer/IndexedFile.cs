using System;
using System.ComponentModel.DataAnnotations;

namespace FileIndexer
{
    public class IndexedFile
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Path { get; set; }
    }
}
