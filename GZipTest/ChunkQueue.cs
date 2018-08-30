using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    class ChunkQueue : IChunkQueue
    {
        public bool closed = false;

        private int idCounter = 0;
        private int queueCounter = 0;
        private Queue<Chunk> queue = new Queue<Chunk>();
        private int maxSize;


        public ChunkQueue(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void Enqueue(Chunk chunk)
        {
            int id = chunk.Id;
            lock (queue)
            {
                while (queueCounter >= maxSize || id != idCounter)
                {
                    if(closed)
                    {
                        chunk = new Chunk();

                        return;
                    }
                    Monitor.Wait(queue);
                }
                queue.Enqueue(chunk);
                idCounter++;
                queueCounter++;
                Monitor.PulseAll(queue);
            }
        }

        public void EnqueueBytes(byte[] buffer)
        {
            lock (queue)
            {
                while (queueCounter >= maxSize)
                {
                    Monitor.Wait(queue);
                }
                Chunk chunk = new Chunk(idCounter, buffer);
                queue.Enqueue(chunk);
                idCounter++;
                queueCounter++;
                Monitor.PulseAll(queue);
            }
        }

        public bool TryDequeue(out Chunk chunk)
        {

            lock (queue)
            {
                while (queueCounter == 0)
                {
                    if (closed)
                    {
                        chunk = new Chunk();
                    
                        return false;
                    }
                    Monitor.Wait(queue);
                }
                    chunk = queue.Dequeue();
                    queueCounter--;
                    Monitor.PulseAll(queue);

                    return true;
            }
        }

        public void Close()
        {
            lock (queue)
            {
                closed = true;
                Monitor.PulseAll(queue);
            }
        }

        public void Clear()
        {
            lock (queue)
            {
                closed = true;
                queueCounter = 0;
                Monitor.PulseAll(queue);
                
            }
        }
    }
}
