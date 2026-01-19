using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuneSpot.Model;

public class Fingerprint
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Song")]
    public Guid SongId { get; set; }

    public uint Hash { get; set; }  // 32-bit fingerprint code

    public int TimeOffset { get; set; }  // Position in song (e.g., frmae or seconds)

    // Link back to song
    public virtual Song Song { get; set; } = null!;
}