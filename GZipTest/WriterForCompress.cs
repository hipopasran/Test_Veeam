using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    class WriterForCompress : IWriter
    {
        private string outputFile;
        private IChunkQueue writeBuffer;
        private Canceler Canceler;

        public WriterForCompress(string outputFile, IChunkQueue writeBuffer, Canceler canceler)
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
                    BinaryFormatter formatter = new BinaryFormatter();
                    Chunk chunk;

                    while (writeBuffer.TryDequeue(out chunk) && !Canceler.IsCancel)
                    {
                        formatter.Serialize(output, chunk);
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
