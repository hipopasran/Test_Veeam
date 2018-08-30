using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    interface IDecompressor
    {
        void Decompress(object threadNumber);
    }
}
