using System;
using System.IO;
using ML.Control;

namespace ML
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DBG mode ? (Y/n)");
            string dbg = Console.ReadLine();

            Console.WriteLine(
                "Mode : (1 Load Model, 2 Read data, 3 Read data + save, 4: process data, 5 : process data + save");
            string res = Console.ReadLine();

            MLController controller = new MLController();
            if (res == "1")
            {
                controller.LoadModel();
            }
            else if (res == "2")
            {
                controller.InitializeAndLoadData();
            }
            else if (res == "3")
            {
                controller.InitializeAndLoadData();
                controller.SaveModel();
            }
            else if (res == "4")
            {
                Console.WriteLine("Data path :");
                string path = Console.ReadLine();
                DatasetComputer computer = new DatasetComputer();
                int val = -1;
                if (Equals(dbg, "Y") || Equals(dbg, "y")) { val = 50; }

                computer.ComputeCarAsset(path, MLController._imagesFolder, val);
                controller.InitializeAndLoadData();
            }
            else if (res == "5")
            {
                Console.WriteLine("Data path :");
                string path = Console.ReadLine();
                DatasetComputer computer = new DatasetComputer();
                int val = -1;
                if (Equals(dbg, "Y") || Equals(dbg, "y")) { val = 50; }
                computer.ComputeCarAsset(path, MLController._imagesFolder, val);
                controller.InitializeAndLoadData();
                controller.SaveModel();
            }

        }
    }
}