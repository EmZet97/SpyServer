using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpyServer
{
    class TextMessage : Message
    {
        
        public override object GetSpecyficObject()
        {
            if(Content[0] == Byte.Parse("2"))
            {
                return "zwracam tekst";
            }
            else
            {
                if (Next != null)
                {
                    Next.Content = Content;
                    return Next.GetSpecyficObject();
                }
                    //return Next.GetSpecyficObject();

                return null;
            }
        }

        public override void SetSpecyficObject(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
