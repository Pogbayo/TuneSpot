using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TuneSpot.Model
{
    public class Song
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Artist { get; set; } = string.Empty;

        public double Duration { get; set; }  // In seconds

        // Link to fingerprints (one song has many fingerprints)
        public virtual List<Fingerprint> Fingerprints { get; set; } = new List<Fingerprint>();
    }
}
