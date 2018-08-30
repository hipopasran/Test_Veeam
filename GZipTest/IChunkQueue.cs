using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    interface IChunkQueue
    {
        void Enqueue(Chunk chunk);
        void EnqueueBytes(byte[] buffer);
        bool TryDequeue(out Chunk chunk);
        void Close();
        void Clear();
    }
}
