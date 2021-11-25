using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class BaseClasses
    {
        public virtual List<string> ReadFile(string file)
        {
            List<string> result = new List<string>();
            StreamReader reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }
            reader.Close();
            return result;
        }


        public virtual void CreateDirectoriesToSave(string dirToSave)
        {
            if (!Directory.Exists(dirToSave))
                Directory.CreateDirectory(dirToSave);
        }
    }

    public abstract class Abstr
    {
        public abstract void DoSth();
    }
    public static class Test
    {
        static void DoSth()
        {
            BaseClasses c = new BaseClasses();
        }
    }
}
