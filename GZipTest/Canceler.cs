using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    class Canceler
    {
        private bool cancel;

        public bool IsCancel
        {
            get
            {
                return cancel;
            }
        }

        public void Cancel()
        {
            cancel = true;
        }

    }
}
