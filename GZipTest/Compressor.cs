using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    class Compressor : ICompressor
    {
        private IChunkQueue readBuffer;
        private IChunkQueue writeBuffer;
        private Canceler Canceler;

        public Compressor(IChunkQueue readBuffer,IChunkQueue writeBuffer, Canceler canceler)
        {
            this.readBuffer = readBuffer;
            this.writeBuffer = writeBuffer;
            this.Canceler = canceler;
        }

        public void Compress(object threadNumber)
        {
            try
            {
                Chunk inputChunk;

                while (readBuffer.TryDequeue(out inputChunk) && !Canceler.IsCancel)
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress))
                        using (BinaryWriter bwriter = new BinaryWriter(gzip))
                        {
                            bwriter.Write(inputChunk.Buffer, 0, inputChunk.Buffer.Length);
                        }

                        byte[] outBuffer = memory.ToArray();
                        Chunk outputChunk = new Chunk(inputChunk.Id, outBuffer);

                        writeBuffer.Enqueue(outputChunk);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Canceler.Cancel();
                readBuffer.Clear();
                writeBuffer.Clear();
            }
        }
    }
}
