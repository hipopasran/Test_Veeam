using System;
using System.Configuration;
using System.Threading;

namespace GZipTest
{
    class Gzipper : IGzipper
    {
        protected static bool cancel = false;
        Canceler canceler;

        protected int bufferSize = Convert.ToInt32(ConfigurationManager.AppSettings.Get("bufferSize"));
        protected static int queueSize = Convert.ToInt16(ConfigurationManager.AppSettings.Get("queueSize"));

        protected ChunkQueue readBuffer = new ChunkQueue(queueSize);
        protected ChunkQueue writeBuffer = new ChunkQueue(queueSize);

        protected string inputFile;
        protected string outputFile;

        protected static int compressionThreads = Environment.ProcessorCount;



        public Gzipper(string input, string output, Canceler cancr)
        {
            inputFile = input;
            outputFile = output;
            canceler = cancr;
        }

        public void Cancel()
        {
            cancel = true;
        }

        public void Compress()
        {
            ReaderForCompress Reader = new ReaderForCompress(inputFile,readBuffer,bufferSize,canceler);
            Compressor Compressor = new Compressor(readBuffer,writeBuffer, canceler);
            WriterForCompress Writer = new WriterForCompress(outputFile,writeBuffer, canceler);

            ExecuteCompress(Reader,Compressor,Writer);
        }

        public void Decompress()
        {
            ReaderForDecompress Reader = new ReaderForDecompress(inputFile, readBuffer, bufferSize, canceler);
            Decompressor Decompressor = new Decompressor(readBuffer, writeBuffer,bufferSize , canceler);
            WriterForDecompress Writer = new WriterForDecompress(outputFile,writeBuffer, canceler);

            ExecuteDecompress(Reader, Decompressor, Writer);
        }

        public void ExecuteCompress(IReader Reader, ICompressor Compressor, IWriter Writer)
        {
            Thread reader = new Thread(new ThreadStart(Reader.Read));
            reader.Start();


            Thread[] compressor = new Thread[compressionThreads];
            for (int i = 0; i < compressionThreads; i++)
            {
                compressor[i] = new Thread(new ParameterizedThreadStart(Compressor.Compress));
                compressor[i].Start(i);

            }

            Thread writer = new Thread(new ThreadStart(Writer.Write));
            writer.Start();

            reader.Join();
            for (int i = 0; i < compressionThreads; i++)
            {
                compressor[i].Join();
            }

            writeBuffer.Close();
            writer.Join();
        }

        public void ExecuteDecompress(ReaderForDecompress Reader, Decompressor Decompressor, WriterForDecompress Writer)
        {
            Thread reader = new Thread(new ThreadStart(Reader.Read));
            reader.Start();

            Thread[] decompressor = new Thread[compressionThreads];
            for (int i = 0; i < compressionThreads; i++)
            {
                decompressor[i] = new Thread(new ParameterizedThreadStart(Decompressor.Decompress));
                decompressor[i].Start(i);
            }

            Thread writer = new Thread(new ThreadStart(Writer.Write));
            writer.Start();

            reader.Join();
            for (int i = 0; i < compressionThreads; i++)
            {
                decompressor[i].Join();
            }

            writeBuffer.Close();
            writer.Join();
        }
    }
}
