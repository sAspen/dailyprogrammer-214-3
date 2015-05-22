using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedyPomerian
{
    class KdTree
    {
        public double[] Point;
        KdTree Parent, LeftChild, RightChild;

        private KdTree(double[] point, KdTree leftChild, KdTree rightChild)
        {
            Point = point;
            LeftChild = leftChild;
            RightChild = rightChild;

            _ConfirmParents();
        }

        public int Dimensions
        {
            get
            {
                return Point.Length;
            }
        }

        private void _ConfirmParents()
        {
            if (LeftChild != null)
            {
                LeftChild.Parent = this;
                LeftChild._ConfirmParents();
            }
            if (RightChild != null)
            {
                RightChild.Parent = this;
                RightChild._ConfirmParents();
            }
        }

        private static KdTree _New(List<double[]> points, int depth)
        {
            if (points == null || points.Count == 0)
            {
                return null;
            }

            int axis = depth % points[0].Length;
            points = points.OrderBy(p => p[axis]).ToList();
            int median = points.Count / 2;

            List<double[]> leftChildren = points.GetRange(0, median).ToList();
            List<double[]> rightChildren = points.GetRange(median + 1, points.Count - median - 1).ToList();

            return new KdTree(points[median],
                KdTree._New(leftChildren, depth + 1),
                KdTree._New(rightChildren, depth + 1));
        }

        public static KdTree New(List<double[]> points)
        {
            return KdTree._New(points, 0);
        }

        private bool _Insert(double[] point, int depth)
        {
            int axis = depth % Dimensions;

            if (point[axis] <= Point[axis])
            {
                if (LeftChild != null)
                {
                    return LeftChild._Insert(point, depth + 1);
                }
                else
                {
                    LeftChild = new KdTree(point, null, null);
                    _ConfirmParents();
                    return true;
                }
            }
            else
            {
                if (RightChild != null)
                {
                    return RightChild._Insert(point, depth + 1);
                }
                else
                {
                    RightChild = new KdTree(point, null, null);
                    _ConfirmParents();
                    return true;
                }
            }
        }

        public bool Insert(double[] point)
        {
            if (point.Length != Dimensions)
            {
                return false;
            }

            return _Insert(point, 0);
        }

        private void _RemoveChild(KdTree child, List<double[]> grandchildren, int depth)
        {
            if (child == LeftChild)
            {
                if (grandchildren.Count == 0)
                {
                    LeftChild = null;
                }
                else
                {
                    LeftChild = KdTree._New(grandchildren, depth);
                }
            }
            else
            {
                if (grandchildren.Count == 0)
                {
                    RightChild = null;
                }
                else
                {
                    RightChild = KdTree._New(grandchildren, depth);
                }
            }

            _ConfirmParents();
        }

        private bool _Remove(double[] point, int depth)
        {
            int axis = depth % Dimensions;

            if (point.SequenceEqual(Point))
            {
                List<double[]> children = new List<double[]>();

                if (LeftChild != null)
                {
                    children = LeftChild.ToList().
                        Concat(children).ToList();
                }

                if (RightChild != null)
                {
                    children = children.
                        Concat(RightChild.ToList()).ToList();
                }

                if (Parent != null)
                {
                    Parent._RemoveChild(this, children, depth);
                } 
                else
                {
                    children = children.OrderBy(p => p[axis]).ToList();
                    int median = children.Count / 2;

                    List<double[]> leftChildren = children.GetRange(0, median).ToList();
                    List<double[]> rightChildren = children.GetRange(median + 1, children.Count - median - 1).ToList();

                    Point = children[median];
                    LeftChild = KdTree._New(leftChildren, depth + 1);
                    RightChild = KdTree._New(rightChildren, depth + 1);
                }

                _ConfirmParents();

                return true;
            }

            if (point[axis] <= Point[axis])
            {
                if (LeftChild != null)
                {
                    return LeftChild._Remove(point, depth + 1);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (RightChild != null)
                {
                    return RightChild._Remove(point, depth + 1);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Remove(double[] point)
        {
            if (point.Length != Dimensions)
            {
                return false;
            }

            return _Remove(point, 0);
        }

        private bool _Contains(double[] point, int depth)
        {
            if (point.SequenceEqual(Point))
            {
                return true;
            }

            int axis = depth % Dimensions;

            if (point[axis] <= Point[axis])
            {
                if (LeftChild != null)
                {
                    return LeftChild._Contains(point, depth + 1);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (RightChild != null)
                {
                    return RightChild._Contains(point, depth + 1);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Contains(double[] point)
        {
            if (point.Length != Dimensions)
            {
                return false;
            }

            return _Contains(point, 0);
        }

        public double GetDistanceTo(double[] targetPoint)
        {
            double ret = 0.0;
            for (int i = 0; i < Dimensions; i++)
            {
                ret += Math.Pow(Point[i] - targetPoint[i], 2);
            }

            return Math.Sqrt(ret);
        }

        private void _GetNearestTo(double[] targetPoint, ref KdTree currentBest, ref double currentBestDistance)
        {
            double childDistance;
            if (LeftChild != null)
            {
                childDistance = LeftChild.GetDistanceTo(targetPoint);
                if (currentBestDistance > childDistance)
                {
                    currentBestDistance = childDistance;
                    currentBest = LeftChild;
                    LeftChild._GetNearestTo(targetPoint, ref currentBest, ref currentBestDistance);
                }
            }
            if (RightChild != null)
            {
                childDistance = RightChild.GetDistanceTo(targetPoint);
                if (currentBestDistance > childDistance)
                {
                    currentBestDistance = childDistance;
                    currentBest = RightChild;
                    RightChild._GetNearestTo(targetPoint, ref currentBest, ref currentBestDistance);
                }
            }
        }

        public KdTree GetNearestTo(double[] targetPoint, out double distance)
        {
            distance = double.PositiveInfinity;
            if (targetPoint.Length != Dimensions)
            {
                return null;
            }

            KdTree currentBest = this;
            distance = GetDistanceTo(targetPoint);
            _GetNearestTo(targetPoint, ref currentBest, ref distance);

            return currentBest;
        }

        public List<double[]> ToList()
        {
            List<double[]> ret = new List<double[]>();

            ret.Add(Point);
            if (LeftChild != null)
            {
                ret = LeftChild.ToList().Concat(ret).ToList();
            }

            if (RightChild != null)
            {
                ret = ret.Concat(RightChild.ToList()).ToList();
            }

            return ret;
        }

        public int GetCount()
        {
            int ret = 1;

            if (LeftChild != null)
            {
                ret += LeftChild.GetCount();
            }
            if (RightChild != null)
            {
                ret += RightChild.GetCount();
            }

            return ret;
        }
    }
}
