using System;
using System.IO;

namespace GZipTest
{
    class ReaderForCompress : IReader
    {

        private string inputFile;
        private int bufferSize;
        private IChunkQueue readBuffer;
        private Canceler Canceler;

        public ReaderForCompress(string inputFile, IChunkQueue readBuffer, int bufferSize, Canceler canceler)
        {
            this.inputFile = inputFile;
            this.readBuffer = readBuffer;
            this.bufferSize = bufferSize;
            this.Canceler = canceler;
        }

        public void Read()
        {
            try
            {
                int countRead;
                byte[] buffer = new byte[bufferSize];

                using (FileStream input = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    while ((countRead = input.Read(buffer, 0, bufferSize)) > 0 && !Canceler.IsCancel)
                    {
                        byte[] lastBuffer = new byte[countRead];
                        Buffer.BlockCopy(buffer, 0, lastBuffer, 0, countRead);

                        readBuffer.EnqueueBytes(lastBuffer);
                    }
                    readBuffer.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Canceler.Cancel();
                readBuffer.Close();
            }
        }
    }
}
