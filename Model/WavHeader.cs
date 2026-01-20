using System.ComponentModel.DataAnnotations;

namespace TuneSpot.Model
{
    public class WavHeader
    {
        [Required]
        public string RiffId { get; set; } = "RIFF";  // Always "RIFF(Resource interchange file format)"

        public int Channels { get; set; } = 1;  // 1 or 2 (either ono or stereo)

        public int SampleRate { get; set; } = 44100;  // SampleRate is how many snapshots of sound per second (in Hertz, or Hz).

        /*
         * BitsPerSample Summary (WAV Header Field):
         * - Location: Bytes 34-35 in header (read as short with reader.ReadInt16()).
         * - What it is: Number of bits per audio sample (e.g., 16 = standard for CD quality).
         * - Why it matters: Tells how big each volume value is (16 bits = 2 bytes per sample, range -32768 to 32767 for quiet to loud).
         * - Example: If 16 bits, each number in left/right arrays (like 123) is stored as 2 bytes in the file.
         * - Use in code: Check if 16 (common); use to unpack raw bytes to shorts (e.g., BitConverter.ToInt16 for 2 bytes).
         * - Edge: 8 bits = lower quality (0-255 range, tinny sound); 24 bits = high-end (bigger files).
         * - Source: RIFF WAV spec—fixed offset, no calc needed.
         */
        public short BitsPerSample { get; set; } = 16;  // Bits per sample

        /*
         * DataSize Calculation Example (For 1-Second Clip at 44,100 Hz):
         * 1. Total samples = SampleRate (44,100) × Channels (2 for stereo) = 88,200 samples.
         * 2. Total bytes = 88,200 samples × (BitsPerSample / 8 = 16/8 = 2 bytes per sample) = 176,400 bytes.
         * 
         * For mono (Channels = 1): 44,100 × 1 × 2 = 88,200 bytes.
         * Use this to read exactly DataSize bytes from position 44 in the file.
         */
        public int DataSize { get; set; } = 0;  // Audio data length in bytes - the total bytes of the audio samplings
    }
}
