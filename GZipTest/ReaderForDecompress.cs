using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GZipTest
{
    class ReaderForDecompress : IReader
    {
        private string inputFile;
        private int bufferSize;
        private IChunkQueue readBuffer;
        private Canceler Canceler;

        public ReaderForDecompress(string inputFile, IChunkQueue readBuffer, int bufferSize, Canceler canceler)
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
                BinaryFormatter formatter = new BinaryFormatter();
                Chunk chunk;

                using (FileStream input = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    while (input.Position < input.Length && !Canceler.IsCancel)
                    {
                        chunk = (Chunk)formatter.Deserialize(input);
                        readBuffer.Enqueue(chunk);
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
