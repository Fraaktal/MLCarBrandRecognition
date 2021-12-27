using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Control
{
    public static class ImageSaver
    {
        public static string SaveImage(byte[] img)
        {
            MemoryStream ms = new MemoryStream(img);
            Image ret = Image.FromStream(ms);
            string path = Path.Combine(MLController.GetInstance()._assetsPath, "imgTest.jpg");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            ret.Save(path);
            return path;
        }
    }
}
