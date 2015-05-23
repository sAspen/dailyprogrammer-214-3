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
            int numberOfTreats = 0, count;

            try
            {
                using (StreamReader sr = new StreamReader(args[0]))
                {
                    string line;

                    if ((line = sr.ReadLine()) != null)
                    {
                        numberOfTreats = Int32.Parse(line);

                        points = new List<double[]>(numberOfTreats + 1);
                        points.Add(start);

                        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
                        count = numberOfTreats;
                        while (count-- > 0 && (line = sr.ReadLine()) != null)
                        {
                            string[] tokens = line.Split(' ');
                            double[] point = { Double.Parse(tokens[0], culture), 
                                Double.Parse(tokens[1], culture) };
                            points.Add(point.Clone() as double[]);
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

            double[] targetPoint = start.Clone() as double[];
            double distance = 0.0;

            count = numberOfTreats; 
            while (count-- > 0)
            {
                KdTree cur = dogPen.GetNearestTo(targetPoint);
                distance += Math.Sqrt(cur.GetDistanceTo(targetPoint));
                targetPoint = cur.NodePoint.Clone() as double[];
                /*if (count == 24962)
                {
                    Console.WriteLine("Hello, world");
                }*/
                dogPen.Remove(targetPoint);
                /*if (count == 24962)
                {
                    dogPen.Assert();
                }*/
                /*if (dogPen.FindBrute(targetPoint) != null)
                {
                    throw new SystemException();
                }
                dogPen.Assert();*/
            }
            //distance += Math.Sqrt(dogPen.GetDistanceTo(targetPoint));

            Console.WriteLine(distance);
        }
    }
}
