using TuneSpot.Model;

namespace TuneSpot.Services.DecodeWavFolder
{ 
    // Decoding the wav file and standardizing the channels into mono 
    public class DecodeWavDoc
    {
        public (int sampleRate , int channels, short[] left , short[] right) Decode(byte[] wavBytes)
        {
            //iles come as raw binary data, not a ready file object. So, we grab it as byte[] (an array of numbers representing the file's bits). It's like breaking the file into tiny pieces for the server to read fast.
            //frotnend send the bytes as base64(an encoded form of the bytes) and the controller decodes into bytes[] wavBytes which is passed to the decodeWav method

            //Turns the byte[] wavBytes (the file data) into a streamlike a flowing river of bytes I can read from. MemoryStream keeps it in RAM 
            //using var auto closes the stream after use to keep memory free of leakages coz leakages could crash the server
            using var stream = new MemoryStream(wavBytes);

            //This is the part where the reader maps out the important numbers and I use them to set the headers of the audio file
            using var reader = new BinaryReader(stream);

            //instantiating a WavHeader class
            var header = new WavHeader();

            //Jump the reader's pointer to byte 0 (start of file), from the beginning
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            //Reads exaclty the first 4 bytes from there (0,1,2,3) in order then GetString turns the bytes to string (R I F F).
            //Encoding.ASCII is an inbuilt C# tool used to convert bytes into readble text
            header.RiffId = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4));

            //ReadInt is ised when we want to convert the byte to a number directly and this is RaeadInt16 because it expects a small value(refer to WavHeader class to check the channels property for bettter understanding)
            reader.BaseStream.Seek(22, SeekOrigin.Begin);
            header.Channels = reader.ReadInt16();  // Bytes 22-23

            reader.BaseStream.Seek(24, SeekOrigin.Begin);
            //RaeadInt32 here because it expects a big value(refer to WavHeader class to check the SampleRAte property for better understanding)
            header.SampleRate = reader.ReadInt32();  // Bytes 24-27

            reader.BaseStream.Seek(34, SeekOrigin.Begin);
            //RaeadInt16 here because it expects a small value(refer to WavHeader class to check the BitPerSample property for better understanding)
            header.BitsPerSample = reader.ReadInt16();  // Bytes 34-35

            reader.BaseStream.Seek(40, SeekOrigin.Begin);
            //RaeadInt32 here because it expects a big value(refer to WavHeader class to check the SamplDataSize property for better understanding)
            header.DataSize = reader.ReadInt32();  // Bytes 40-43

            // Check valid
            if (header.RiffId != "RIFF" || header.Channels > 2) throw new Exception("Invalid WAV");

            //now we move the stream marker to byte point 44 to read the sample audios
            reader.BaseStream.Seek(44, SeekOrigin.Begin);
            // here we calculate the total number of sample audios in the channels(depending on mono or stereo)
            // We get the total number of audio bytes and and divide by each record which could come as mono or stereo(mono is one channel while stereo is 2 channels)
            // we alculate the total number of audio samples in the file
            // The dataSize gives total audio bytes. Each sample frame contains one value per channel (1 for mono, 2 for stereo)
            // BitsPerSample tellsme how many bits each sample uses, so I convert it to bytes (BitsPerSample / 8)
            // Dividing total bytes by bytes per sample frame gives the total number of samples
            var numSamples = header.DataSize / (header.Channels * (header.BitsPerSample / 8));

            // redas all audio bytes from te file and convert them into a numeric array
            // DataSize tells how many bytes of audio data there are
            // Each byte is converted to a short (simplified here; for real 16-bit audio, two bytes = one sample)
            // The result is an interleaved array containing all audio data in order

            //short[] interleaved = reader.ReadBytes(header.DataSize).Select(b => (short)b).ToArray();

            // moved the marker to the start of audio data immediately after the header
            reader.BaseStream.Seek(44, SeekOrigin.Begin);
            short[] interleaved = new short[numSamples];

            //looping through the bytes in the audio data mark , picking two bytes to be turned into a short and finally adding it to the array of shorts
            for (int i = 0; i < numSamples; i ++)
            {
                interleaved[i] = reader.ReadInt16();
            }
            //after reading the butes into a unified array, it is time to separate the left and right channels
            //I then initialized two short arrays(left and right) with both having the limit of half the total number of bytes gotten from the wav file
            short[] left = new short[numSamples / 2];
            short[] right = new short[numSamples / 2];

            //if the header.Channels is not equals to 2, we skip this block and jump to the else block
            if (header.Channels == 2)
            {
                //we create a loop to separate the sheep from the wolves
                //I initialized two pointers (i and j), and the i pointer increments by 2 while the j pointer increments by 1 after every iteration
                for (int i = 0, j = 0;  i < numSamples; i += 2, j++)
                {
                    //interleaved = [ L0, R0, L1, R1, L2, R2, L3, R3 ]  ---- example to work with

                    //to properlly get the separation, when the iteration starts, I use the j pointer to punch in the exact value when it gets to the exact element
                    //now when the first iteration starts, j is initialized by default to 0, both left and right array have their element placed on the same value as J which is 0
                    //as seen here j = the nth element of the interleaved array(this depending on the iterartion number - take 0 as an example)..  
                    //As seen in the above array example named interleaved, the first element = l0 which is assigned to left[j=0] by interleaved[i(which is also 0 in the first iteration]
                    left[j] = interleaved[i];
                    //after carefully assigning the first element of the interleaved array to left[j=o(first position as well)]],
                    //we take one step forward and assign the element there to the right[j=0]

                    //                    i+1     
                   // interleaved = [L0,  R0,  L1, R1, L2, R2, L3, R3]
                    right[j] = interleaved[i + 1];
                    //after this iteration, the i pointer increments by 2, j increments by 1 and repeats the same process until the end
                }
            }
            else
            {
                //if the channel is not 2, it just defaults to this automatically
                left = interleaved;
            }

            return (header.SampleRate, header.Channels, left, right); 
        }
          
        public float[] Standardize(short[] left, short[] right, int targetRate = 44100)
        {
            /// Standardizes audio by converting to mono, resampling to a target sample rate,
            // and normalizing 16-bit samples into float values between -1 and 1.


            return new float[0];  
        }
    }
}
