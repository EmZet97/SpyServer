using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpyServer.Messages
{
    class MessageReaderBuilder
    {
        private Message root;
        private Message last;

        public MessageReaderBuilder(Message firstElement)
        {
            root = firstElement;
            last = root;
        }

        public void AddNext(Message next)
        {
            last.Next = next;
            last = next;
        }

        public Message GetChain()
        {
            return root;
        }
    }
}
