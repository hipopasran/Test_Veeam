using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    class Decompressor
    {

        private int bufferSize;
        private IChunkQueue readBuffer;
        private IChunkQueue writeBuffer;
        private Canceler Canceler;
        
        public Decompressor(IChunkQueue readBuffer, IChunkQueue writeBuffer, int bufferSize, Canceler canceler)
        {
            this.readBuffer = readBuffer;
            this.writeBuffer = writeBuffer;
            this.bufferSize = bufferSize;
            this.Canceler = canceler;
        }

        public void Decompress(object threadNumber)
        {
            try
            {
                Chunk inputChunk;

                while (readBuffer.TryDequeue(out inputChunk) && !Canceler.IsCancel)
                {
                    using (MemoryStream memory = new MemoryStream(inputChunk.Buffer))
                    {
                        using (GZipStream gzip = new GZipStream(memory, CompressionMode.Decompress))
                        {
                            int countRead;
                            byte[] buffer = new byte[bufferSize];

                            countRead = gzip.Read(buffer, 0, buffer.Length);

                            byte[] lastBuffer = new byte[countRead];
                            Buffer.BlockCopy(buffer, 0, lastBuffer, 0, countRead);
                            Chunk outputChunk = new Chunk(inputChunk.Id, lastBuffer);

                            writeBuffer.Enqueue(outputChunk);
                        }
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
