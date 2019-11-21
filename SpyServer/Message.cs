using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpyServer
{
    abstract class Message
    {
        private int requestCode;
        private byte[] content;
        private Message next;

        public byte[] Content { get; set; }
        //public Object GetObject() { }

        public abstract Object GetSpecyficObject();
    }
}
