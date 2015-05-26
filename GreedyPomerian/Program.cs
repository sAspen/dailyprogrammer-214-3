using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Trees;

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

            dogPen = new KdTree(points);

            double[] targetPoint = start.Clone() as double[];
            double distance = 0.0;

            count = numberOfTreats; 
            while (count-- > 0)
            {
                if (count == 96) {

                }
                double[] current = dogPen.GetNearestTo(targetPoint);
                distance += Math.Sqrt(KdTree.GetDistanceTo(current, targetPoint));
                targetPoint = (double[]) current.Clone();
                if (!dogPen.Remove(targetPoint)) {
                    throw new SystemException();
                }
            }

            Console.WriteLine(distance);
        }
    }
}
