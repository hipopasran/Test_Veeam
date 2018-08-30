using System;
using System.Diagnostics;
using System.IO;

namespace GZipTest
{
    class Program
    {
        static Gzipper gzipper;
        static Canceler canceler;

        static void Main(string[] args)
        {
            var time = Stopwatch.StartNew();
            canceler = new Canceler();

            Console.CancelKeyPress += new ConsoleCancelEventHandler(cancelHandler);

            try
            {
                ValidateCommandLineInput(args);
                var mode = args[0];
                var inputFile = args[1];
                var outputFile = args[2];
                gzipper = new Gzipper(inputFile, outputFile, canceler);
                if (mode == "compress")
                {
                    gzipper.Compress();
                }
                else
                {
                    gzipper.Decompress();
                }


                Console.WriteLine($"time Elapsed - {time.Elapsed.TotalSeconds}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                canceler.Cancel();
            }
        }

        static void ValidateCommandLineInput(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("Command-line must contain 3 arguments", "args");
            }

            if (args[0].ToLower() != "compress" && args[0].ToLower() != "decompress")
            {
                throw new ArgumentException("First argument must be 'compress' or 'decompress'", "args[0]");
            }

            if (!File.Exists(args[1]))
            {
                throw new ArgumentException("File [" + args[1] + "] doesn't exist", "args[1]");
            }

            if (args[1] == args[2])
            {
                throw new ArgumentException("Input and output files have same names");
            }

            FileInfo fileIn = new FileInfo(args[1]);
            if (fileIn.Length == 0)
            {
                throw new ArgumentException("File [" + args[1] + "] has 0 bytes size", "args[1]");
            }
            if (fileIn.Extension == ".gz" && args[0] == "compress")
            {
                throw new ArgumentException("File [" + args[1] + "] already compressed", "args[1]");
            }

            if (fileIn.Length < 11 && args[0] == "decompress")
            {
                throw new Exception("Minimal file size to decompress = 11 bytes (10 for header)");
            }
        }

        static void cancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Progrem canceled by user");
            args.Cancel = true;
            canceler.Cancel();

        }
    }
}
