using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpyServer
{
    class RequestMessage : Message
    {

        public override object GetSpecyficObject()
        {
            if (Content[0] == Byte.Parse("1"))
            {
                return "zwracam żądanie";
            }
            else
            {
                if (Next != null)
                    return Next.GetSpecyficObject();

                return null;
            }
        }

        public override void SetSpecyficObject(object obj)
        {
            throw new NotImplementedException();
        }
    }
}