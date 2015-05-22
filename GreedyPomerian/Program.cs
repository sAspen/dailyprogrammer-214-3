using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedyPomerian
{
    class Program
    {
        static void Main(string[] args)
        {
            KdTree dogPen;
            List<double[]> points = null;
            double[] start = { 0.5, 0.5 };

            try
            {
                using (StreamReader sr = new StreamReader(args[0]))
                {
                    string line;

                    if ((line = sr.ReadLine()) != null)
                    {
                        int numberOfTreats = Int32.Parse(line);

                        points = new List<double[]>(numberOfTreats + 1);
                        points.Add(start);

                        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
                        while (numberOfTreats-- > 0 && (line = sr.ReadLine()) != null)
                        {
                            string[] tokens = line.Split(' ');
                            double[] point = { Double.Parse(tokens[0], culture), 
                                Double.Parse(tokens[1], culture) };
                            points.Add(point);
                            //Console.WriteLine("Added " + line);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);

                throw;
            }

            dogPen = KdTree.New(points);

            dogPen.Remove(start);
            double[] curPoint = start;
            double distance = 0.0;

            int count; 
            while ((count = dogPen.GetCount()) > 1)
            {
                double ret;
                KdTree cur = dogPen.GetNearestTo(curPoint, out ret);
                curPoint = cur.Point;
                dogPen.Remove(cur.Point);
                distance += ret;
            }
            distance += dogPen.GetDistanceTo(curPoint);

            Console.WriteLine(distance);
        }
    }
}
