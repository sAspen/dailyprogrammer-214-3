using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedyPomerian
{
    class KdTree
    {
        private static Random _RNG;
        public double[] NodePoint;
        KdTree Parent, LeftChild, RightChild;
        internal bool Ignore;

        static KdTree()
        {
            _RNG = new Random();
        }

        private KdTree(double[] point, KdTree leftChild, KdTree rightChild)
        {
            NodePoint = point.Clone() as double[];
            LeftChild = leftChild;
            RightChild = rightChild;
            Ignore = false;

            _ConfirmParents();
        }

        public int Dimensions
        {
            get
            {
                return NodePoint.Length;
            }
        }

        public int Axis
        {
            get
            {
                return Depth % Dimensions;
            }
        }

        public int Depth
        {
            get
            {
                return _GetDepth();
            }
        }

        private KdTree _GetSibling(KdTree child)
        {
            if (child == LeftChild)
            {
                return RightChild;
            }
            else if (child == RightChild)
            {
                return LeftChild;
            }
            else
            {
                return null;
            }
        }

        private int _GetDepth()
        {
            if (Parent != null)
            {
                return 1 + Parent._GetDepth();
            }

            return 0;
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

            KdTree tree = new KdTree(points[median].Clone() as double[], null, null);
            points.Remove(points[median]);
            while (points.Count > 0)
            {
                int i = _RNG.Next(points.Count);
                double[] point = points[i].Clone() as double[];
                points.RemoveAt(i);
                tree.Insert(point.Clone() as double[]);
            }

            return tree;

            /*List<double[]> leftChildren = points.Count > 0 ? points.GetRange(0, median).ToList() : null;
            List<double[]> rightChildren = points.Count > 0 ? points.GetRange(median + 1, points.Count - median - 1).ToList() : null;

            return new KdTree(points[median].Clone() as double[],
                KdTree._New(leftChildren, depth + 1),
                KdTree._New(rightChildren, depth + 1));*/
        }

        public static KdTree New(List<double[]> points)
        {
            return KdTree._New(points, 0);
        }

        public void Assert()
        {
            if (LeftChild != null)
            {
                if (!(LeftChild.NodePoint[Axis] < NodePoint[Axis]))
                {
                    throw new SystemException();
                }
                LeftChild.Assert();
            }
            if (RightChild != null)
            {
                if (!(RightChild.NodePoint[Axis] >= NodePoint[Axis]))
                {
                    throw new SystemException();
                }
                RightChild.Assert();
            }
        }

        private KdTree _GetParentOf(double[] point)
        {
            int axis = Axis;

            if (point[axis] < NodePoint[axis])
            {
                if (LeftChild != null)
                {
                    return LeftChild._GetParentOf(point);
                }
                else
                {
                    return this;
                }
            }
            else
            {
                if (RightChild != null)
                {
                    return RightChild._GetParentOf(point);
                }
                else
                {
                    return this;
                }
            }
        }

        private bool _Insert(double[] point)
        {
            int axis = Axis;

            if (point[axis] < NodePoint[axis])
            {
                if (LeftChild != null)
                {
                    return LeftChild._Insert(point);
                }
                else
                {
                    LeftChild = new KdTree(point.Clone() as double[], null, null);
                    _ConfirmParents();
                    return true;
                }
            }
            else
            {
                if (RightChild != null)
                {
                    return RightChild._Insert(point);
                }
                else
                {
                    RightChild = new KdTree(point.Clone() as double[], null, null);
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

            return _Insert(point.Clone() as double[]);
        }

        private void _RemoveChild(KdTree child, List<double[]> grandchildren)
        {
            if (child == LeftChild)
            {
                if (grandchildren.Count == 0)
                {
                    LeftChild = null;
                }
                else
                {
                    LeftChild = KdTree._New(grandchildren, _GetDepth() + 1);
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
                    RightChild = KdTree._New(grandchildren, _GetDepth() + 1);
                }
            }

            _ConfirmParents();
        }

        private static bool _Remove(double[] point, KdTree node)
        {
            if (node != null)
            {
                List<double[]> children = new List<double[]>();

                if (node.LeftChild != null)
                {
                    children = node.LeftChild.ToList().
                        Concat(children).ToList();
                }

                if (node.RightChild != null)
                {
                    children = children.
                        Concat(node.RightChild.ToList()).ToList();
                }

                if (node.Parent != null)
                {
                    node.Parent._RemoveChild(node, children);
                }
                else
                {
                    children = children.OrderBy(p => p[node.Axis]).ToList();
                    int median = children.Count / 2;

                    node.NodePoint = children[median].Clone() as double[];
                    node.LeftChild = node.RightChild = null;

                    children.Remove(children[median]);
                    while (children.Count > 0)
                    {
                        int i = _RNG.Next(children.Count);
                        double[] child = children[i].Clone() as double[];
                        children.RemoveAt(i);
                        node.Insert(child.Clone() as double[]);
                    }


                    
                    /*List<double[]> leftChildren = children.Count > 0 ? 
                        children.GetRange(0, median).ToList() : null;
                    List<double[]> rightChildren = children.Count > 0 ? 
                        children.GetRange(median + 1, children.Count - median - 1).ToList() : null;

                    node.Point = children[median].Clone() as double[];
                    node.LeftChild = KdTree._New(leftChildren, node._GetDepth() + 1);
                    node.RightChild = KdTree._New(rightChildren, node._GetDepth() + 1);*/

                    node._ConfirmParents();
                }

                return true;
            }

            return false;
        }

        public bool Remove(double[] point)
        {
            Find(point).Ignore = true;
            return true;
            //return _Remove(point, Find(point));
        }

        public KdTree Find(double[] point)
        {
            if (!Ignore && point.SequenceEqual(NodePoint))
            {
                return this;
            }

            int axis = Axis;

            if (point[axis] < NodePoint[axis])
            {
                if (LeftChild != null)
                {
                    return LeftChild.Find(point);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (RightChild != null)
                {
                    return RightChild.Find(point);
                }
                else
                {
                    return null;
                }
            }
        }

        public KdTree FindBrute(double[] point)
        {
            if (!Ignore && point.SequenceEqual(NodePoint))
            {
                return this;
            }

            if (LeftChild != null)
            {
                KdTree ret = LeftChild.FindBrute(point);
                if (ret != null)
                {
                    return ret;
                }
            }
            if (RightChild != null)
            {
                KdTree ret = RightChild.FindBrute(point);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }

        private bool _Contains(double[] point)
        {
            if (!Ignore && point.SequenceEqual(NodePoint))
            {
                return true;
            }

            int axis = Axis;

            if (point[axis] < NodePoint[axis])
            {
                if (LeftChild != null)
                {
                    return LeftChild._Contains(point);
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
                    return RightChild._Contains(point);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Contains(double[] point)
        {
            if (!Ignore && point.Length != Dimensions)
            {
                return false;
            }

            return _Contains(point);
        }

        public double GetDistanceTo(double[] targetPoint)
        {
            double ret = 0.0;
            for (int i = 0; i < Dimensions; i++)
            {
                ret += Math.Pow(NodePoint[i] - targetPoint[i], 2);
            }

            return ret;
        }


        private void _GetNearestTo(double[] targetPoint, ref KdTree currentBest, ref double bestDist)
        {
            if (targetPoint[Axis] < NodePoint[Axis])
            {
                if (LeftChild != null)
                {
                    LeftChild._GetNearestTo(targetPoint, ref currentBest, ref bestDist);
                }
            }
            else if (RightChild != null)
            {
                RightChild._GetNearestTo(targetPoint, ref currentBest, ref bestDist);
            }

            if (!Ignore)
            {
                double distance = GetDistanceTo(targetPoint);

                if (distance < bestDist)
                {
                    bestDist = distance;
                    currentBest = this;
                }
            }

            if (Math.Pow(targetPoint[Axis] - NodePoint[Axis], 2) < bestDist)
            {
                if (targetPoint[Axis] < NodePoint[Axis])
                {
                    if (RightChild != null)
                    {
                        RightChild._GetNearestTo(targetPoint, ref currentBest, ref bestDist);
                    }
                }
                else if (LeftChild != null)
                {
                    LeftChild._GetNearestTo(targetPoint, ref currentBest, ref bestDist);
                }
            }
        }

        public KdTree GetNearestTo(double[] targetPoint)
        {
            if (targetPoint.Length != Dimensions)
            {
                return null;
            }

            KdTree currentBest = null;
            double bestDist = Double.PositiveInfinity;

            _GetNearestTo(targetPoint, ref currentBest, ref bestDist);

            return currentBest;
        }

        public List<double[]> ToList()
        {
            List<double[]> ret = new List<double[]>();

            ret.Add(NodePoint.Clone() as double[]);
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

        public override string ToString()
        {
            string s = "{ ";

            for (int i = 0; i < NodePoint.Length; i++)
            {
                s += NodePoint[i] + ", ";
            }

            s = s.Substring(0, s.Length - 2) + " }";

            return s;
        }

        public int GetCount()
        {
            int ret = Ignore ? 0 : 1;

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
