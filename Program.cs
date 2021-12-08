using System;
using System.IO;
using ML.Control;

namespace ML
{
    class Program
    {
        static void Main(string[] args)
        {
            MLController controller = new MLController();
            controller.InitializeAndLoadData();
        }
    }
}
