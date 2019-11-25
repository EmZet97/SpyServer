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
        public Message Next { get; set; }

        public byte[] Content { get; set; }
        public Object MessageObject{ get; set; }

        public abstract Object GetSpecyficObject();
        public abstract void SetSpecyficObject(Object obj);
    }
}
