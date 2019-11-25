using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpyServer
{
    class ImageMessage : Message
    {

        public override object GetSpecyficObject()
        {
            if (Content[0] == Byte.Parse("3"))
            {
                byte[] arr = Content.Skip(1).ToArray();
                using (var ms = new MemoryStream(arr))
                {
                    return Image.FromStream(ms);
                }
                //return "sss";// BitmapSourceToByte(CopyScreen());
            }
            else
            {
                if (Next != null)
                {
                    Next.Content = Content;
                    return Next.GetSpecyficObject();

                }

                return null;
            }
        }

        public override void SetSpecyficObject(object obj)
        {
            throw new NotImplementedException();
        }
        

    }
}