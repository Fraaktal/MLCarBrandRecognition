using System;
using System.IO;
using ML.Control;

namespace ML
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Mode : (1 Load Model, 2 Read data, "+
                              "3 Read data + save, 4: process data, "+
                              "5 : process data + save");

            string res = Console.ReadLine();

            Console.WriteLine("Result path :");
            string rpath = Console.ReadLine();

            if (res == "1")
            {
                MLController.GetInstance(rpath).LoadModel();
            }
            else if (res == "2")
            {
                MLController.GetInstance(rpath).InitializeAndLoadData();
            }
            else if (res == "3")
            {
                MLController.GetInstance(rpath).InitializeAndLoadData();
                MLController.GetInstance(rpath).SaveModel();
            }
            else if (res == "4")
            {
                Console.WriteLine("Data path :");
                string path = Console.ReadLine();
                DatasetComputer computer = new DatasetComputer();
                int val = -1;

                Console.WriteLine("TestMode ? (Y/n)");
                string dbg = Console.ReadLine();

                if (Equals(dbg, "Y") || Equals(dbg, "y")) { val = 50; }

                computer.ComputeCarAsset(path, Path.Combine(rpath, "cars_asset"), val);
                MLController.GetInstance(rpath).InitializeAndLoadData();
            }
            else if (res == "5")
            {
                Console.WriteLine("Data path :");
                string path = Console.ReadLine();
                DatasetComputer computer = new DatasetComputer();
                int val = -1;

                Console.WriteLine("TestMode ? (Y/n)");
                string dbg = Console.ReadLine();

                if (Equals(dbg, "Y") || Equals(dbg, "y")) { val = 50; }
                computer.ComputeCarAsset(path, Path.Combine(rpath, "cars_asset"), val);
                MLController.GetInstance(rpath).InitializeAndLoadData();
                MLController.GetInstance(rpath).SaveModel();
            }
        }      
    }
}