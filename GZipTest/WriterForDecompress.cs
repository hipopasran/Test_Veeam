using System;
using System.IO;

namespace GZipTest
{
    class WriterForDecompress : IWriter
    {
        private string outputFile;
        private IChunkQueue writeBuffer;
        private Canceler Canceler;

        public WriterForDecompress(string outputFile, IChunkQueue writeBuffer, Canceler canceler)
        {
            this.outputFile = outputFile;
            this.writeBuffer = writeBuffer;
            this.Canceler = canceler;
        }

        public void Write()
        {
            try
            {
                using (FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    Chunk chunk;
                    while (writeBuffer.TryDequeue(out chunk) && !Canceler.IsCancel)
                    {
                        byte[] buffer = chunk.Buffer;

                        output.Write(buffer, 0, buffer.Length);
                    }
                }
                if (Canceler.IsCancel)
                {
                    writeBuffer.Close();
                    File.Delete(outputFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Canceler.Cancel();
                writeBuffer.Clear();
                File.Delete(outputFile);
            }
        }
    }
}
