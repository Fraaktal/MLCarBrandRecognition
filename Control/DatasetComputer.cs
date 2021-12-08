using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Control
{
    public class DatasetComputer
    {
        //tags.tsv
        //test-tags.tsv
        public void ComputeCarAsset(string path, string resultPath, int limitPerBrand = -1)
        {
            var parentDir = new DirectoryInfo(path);

            foreach (var dir in parentDir.GetDirectories())
            {
                string name = dir.Name;
                var images = RecImages(dir);

                var rnd = new Random();
                images = images.OrderBy(item => rnd.Next()).ToList();

                int max = limitPerBrand != -1 ? limitPerBrand : images.Count;

            }
        }

        private List<string> RecImages(DirectoryInfo dir)
        {
            List<string> res = new List<string>();
            var dirs = dir.GetDirectories();

            if (dirs.Length == 0)
            {
                return dir.GetFiles().Select(f => f.FullName).ToList();
            }

            foreach (var subDir in dirs)
            {
                res.AddRange(RecImages(subDir));
            }

            return res;
        }

        //shuffle test et tags



       
        

           
        
    }
}
