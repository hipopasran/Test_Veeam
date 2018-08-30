using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    [Serializable]
    class Chunk
    {
        int id;
        byte[] buffer;

        public int Id
        {
            get
            {
                return id;
            }
        }
        public byte[] Buffer
        {
            get
            {
                return buffer;
            }
        }

        public Chunk() : this(0, new byte[0])
        {

        }

        public Chunk(int id, byte[] buffer)
        {
            this.id = id;
            this.buffer = buffer;
        }
    }
}
