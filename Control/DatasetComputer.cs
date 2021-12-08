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
        public void ComputeCarAsset(string path, string resultPath, int limitPerBrand = -1)
        {
            var parentDir = new DirectoryInfo(path);

            Directory.Delete(resultPath,true);
            Directory.CreateDirectory(resultPath);
            var subdirs = parentDir.GetDirectories();
            int a = 1;
            foreach (var dir in subdirs)
            {
                string name = dir.Name;
                Console.WriteLine($"{name} : {a} / {subdirs.Length}");
                var images = RecImages(dir);
                if (images.Count < 10) { a++; continue; }

                var rnd = new Random();
                images = images.OrderBy(item => rnd.Next()).ToList();


                int max = images.Count;
                if (limitPerBrand != -1 && limitPerBrand < images.Count)
                {
                    max = limitPerBrand;
                }

                for (int i = 0; i < 0.8 * max; i++)
                {
                    string newName = $"{name}{i}.jpg";
                    File.Copy(images[i].FullName, Path.Combine(resultPath, newName));

                    string val = newName + "\t" + name + Environment.NewLine;
                    File.AppendAllText(Path.Combine(resultPath, "tags.tsv"), val);
                }

                for (int i = (int)(max * 0.8); i < max; i++)
                {
                    if (File.Exists(Path.Combine(resultPath, $"{name}{i}.jpg")))
                    {
                        continue;
                    }

                    string newName = $"{name}{i}.jpg";
                    File.Copy(images[i].FullName, Path.Combine(resultPath, newName));

                    string val = newName + "\t" + name + Environment.NewLine;
                    File.AppendAllText(Path.Combine(resultPath, "test-tags.tsv"), val);

                }

                a++;
            }
        }

        private List<FileInfo> RecImages(DirectoryInfo dir)
        {
            List<FileInfo> res = new List<FileInfo>();
            var dirs = dir.GetDirectories();

            if (dirs.Length == 0)
            {
                return dir.GetFiles().ToList();
            }

            foreach (var subDir in dirs)
            {
                res.AddRange(RecImages(subDir));
            }

            return res;
        }
    }
}
