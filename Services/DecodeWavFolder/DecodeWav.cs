using TuneSpot.Model;

namespace TuneSpot.Services.DecodeWavFolder
{
    public class DecodeWav
    {
        public (int sampleRate, int channels, short[] left, short[] right) Decode(byte[] wavBytes)
        {
            
            using var stream = new MemoryStream(wavBytes);
            using var reader = new BinaryReader(stream);

            var header = new WavHeader();

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            header.RiffId = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4));

            reader.BaseStream.Seek(22, SeekOrigin.Begin);
            header.Channels = reader.ReadInt16();  

            reader.BaseStream.Seek(24, SeekOrigin.Begin);
            header.SampleRate = reader.ReadInt32();  

            reader.BaseStream.Seek(34, SeekOrigin.Begin);
            header.BitsPerSample = reader.ReadInt16();  

            reader.BaseStream.Seek(40, SeekOrigin.Begin);
            header.DataSize = reader.ReadInt32();  

            if (header.RiffId != "RIFF" || header.Channels > 2) throw new Exception("Invalid WAV");

            reader.BaseStream.Seek(44, SeekOrigin.Begin);
           
            var numSamples = header.DataSize / (header.Channels * (header.BitsPerSample / 8));
            reader.BaseStream.Seek(44, SeekOrigin.Begin);
            short[] interleaved = new short[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                interleaved[i] = reader.ReadInt16();
            }
            
            short[] left = new short[numSamples / 2];
            short[] right = new short[numSamples / 2];

            if (header.Channels == 2)
            {
                
                for (int i = 0, j = 0; i < numSamples; i += 2, j++)
                {
                   
                    left[j] = interleaved[i];
                   
                    right[j] = interleaved[i + 1];
                }
            }
            else
            {
                left = interleaved;
            }

            return (header.SampleRate, header.Channels, left, right);
        }

        public float[] Standardize(short[] left, short[] right, int targetRate = 44100)
        {
            return new float[0];
        }
    }
}
