//#define DEBUGON 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithms
{
    #region Structs
    public struct PathFinderNode
    {
        #region Variables Declaration
        public int     F;
        public int     G;
        public int     H;  // f = gone + heuristic
        public int     X;
        public int     Y;
        public int     PX; // Parent
        public int     PY;
        #endregion
    }
    #endregion

    #region Enum
	/*
    public enum PathFinderNodeType : byte
    {
        Start   = 1,
        End     = 2,
        Open    = 4,
        Close   = 8,
        Current = 16,
        Path    = 32
    }
*/
    public enum HeuristicFormula : byte
    {
        Manhattan           = 1,
        MaxDXDY             = 2,
        DiagonalShortCut    = 3,
        Euclidean           = 4,
        EuclideanNoSQR      = 5,
        Custom1             = 6
    }
    #endregion

	[System.Serializable]
	public struct Point
	{
		public int X;
		public int Y;

		/// <summary>
		/// Создание точки
		/// </summary>
		public Point(int pX, int pY)
		{
			X = pX;
			Y = pY;          
		}

		/// <summary>
		/// Для загрузки из сейва
		/// </summary>
		public Point(string pPoint)
		{
			if (pPoint != null && pPoint != string.Empty)
			{
				string[] d = pPoint.Split(',');
				if (d.Length == 2)
				{
					X = int.Parse(d[0]);
					Y = int.Parse(d[1]);
				}
				else
				{
					X = 0;
					Y = 0;
				}
			}
			else
			{
				X = 0;
				Y = 0;
			}
		}

		/// <summary>
		/// Дистанция (для оптимизации поиска пути)
		/// </summary>
		public Point(int pX1, int pX2, int pY1, int pY2)
		{
			X = pX1 - pX2;
			if (X<0)
				X = -X;
			Y = pY1-pY2;
			if (Y<0)
				Y = -Y;
		}

		public static Point Zero = new Point(0,0); 
		public static Point One  = new Point(1,1);

		public bool Empty
		{
			get
			{
				return (X==0 && Y==0);
			}
		}

		public static Point ClipNegativeValues(Point p)
		{
			return new Point(p.X < 0 ? 0 : p.X, p.Y < 0 ? 0 : p.Y); 
		}

		public override int GetHashCode ()
		{
			return this.X.GetHashCode () ^ this.Y.GetHashCode () << 2;
		}

		public override string ToString()
		{
			return X.ToString()+","+Y.ToString();
		}

		public static bool operator == (Point lhs, Point rhs)
		{
			return lhs.X == rhs.X && lhs.Y==rhs.Y;
		}

		public static bool operator != (Point lhs, Point rhs)
		{
			return lhs.X != rhs.X || lhs.Y!=rhs.Y;
		}

		public static int SqrMagnitude(Point p1, Point p2)
		{
			return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
		}

		public static float Distance(Point p1, Point p2) {
			return Mathf.Sqrt(SqrMagnitude(p1, p2));
		}

		public override bool Equals (object other)
		{
			if (!(other is Point))
			{
				return false;
			}
			Point point = (Point)other;
			return this.X.Equals (point.X) && this.Y.Equals (point.Y);
		}

		public int this [int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.X;
				case 1:
					return this.Y;
				default:
					throw new IndexOutOfRangeException ("Invalid Vector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.X = value;
					break;
				case 1:
					this.Y = value;
					break;
				default:
					throw new IndexOutOfRangeException ("Invalid Vector3 index!");
				}
			}
		}

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        } 
	}

    public class PathFinder
    {
        #region Variables Declaration
        private byte[,]                         mGrid                   = null;
        #endregion

        #region Constructors
        public PathFinder(byte[,] grid)
        {
            if (grid == null)
                throw new Exception("Grid cannot be null");

            mGrid = grid;
        }
        #endregion

		/// <summary>
		/// Задаёт новую сетку
		/// </summary>
		public void SetGrid(byte[,] grid)
		{
			if (grid == null)
				throw new Exception("Grid cannot be null");
			
			mGrid = grid;
		}

		/// <summary>
		/// Возвращает модуль максимальной величины
		/// </summary>
		public static int aMax(int a, int b)
		{
			if (a<0)
				a = -a;
			if (b<0)
				b = -b;
			return a<b?b:a;
		}

		/// <summary>
		/// Возвращает модуль минимальной величины
		/// </summary>
		public static int aMin(int a, int b)
		{
			if (a<0)
				a = -a;
			if (b<0)
				b = -b;
			return a<b?a:b;
		}

		/// <summary>
		/// Модуль. Для Оптимизации
		/// </summary>
		public static int Abs(int pValue)
		{
			return pValue<0?0-pValue:pValue;
		}

        #region Methods
        public List<PathFinderNode> FindPath(Point start, Point end)
        {
            PathFinderNode 		parentNode;
            bool 				found					= false;
            int  				gridX					= mGrid.GetUpperBound(0);
            int  				gridY					= mGrid.GetUpperBound(1);
			int                 mHoriz                  = 0;
			HeuristicFormula    mFormula                = HeuristicFormula.DiagonalShortCut;
			bool                mDiagonals              = true;
			int                 mHEstimate              = 4;
			bool                mPunishChangeDirection  = false;
			bool                mTieBreaker             = true;
			bool                mHeavyDiagonals         = true;
			int                 mSearchLimit            = 450;

			PriorityQueueB<PathFinderNode> mOpen 		= new PriorityQueueB<PathFinderNode>(new ComparePFNode());
			List<PathFinderNode> mClose		= new List<PathFinderNode>();

            sbyte[,] direction;
            if (mDiagonals)
                direction = new sbyte[8,2]{ {0,-1} , {1,0}, {0,1}, {-1,0}, {1,-1}, {1,1}, {-1,1}, {-1,-1}};
            else
                direction = new sbyte[4,2]{ {0,-1} , {1,0}, {0,1}, {-1,0}};

            parentNode.G        = 0;
            parentNode.H        = mHEstimate;
            parentNode.F        = parentNode.G + parentNode.H;
            parentNode.X        = start.X;
            parentNode.Y        = start.Y;
            parentNode.PX       = parentNode.X;
            parentNode.PY       = parentNode.Y;
            mOpen.Push(parentNode);
            while(mOpen.Count > 0)
            {
                parentNode = mOpen.Pop();
                if (parentNode.X == end.X && parentNode.Y == end.Y)
                {
                    mClose.Add(parentNode);
					mOpen.Clear();
                    found = true;
                    break;
                }

                if (mClose.Count > mSearchLimit)
				{
					return null;
				}
                    

                if (mPunishChangeDirection)
                    mHoriz = (parentNode.X - parentNode.PX); 

                //Lets calculate each successors
                for (int i=0; i<(mDiagonals ? 8 : 4); i++)
                {
                    PathFinderNode newNode;
                    newNode.X = parentNode.X + direction[i,0];
                    newNode.Y = parentNode.Y + direction[i,1];

                    if (newNode.X < 0 || newNode.Y < 0 || newNode.X >= gridX || newNode.Y >= gridY)
                        continue;

                    int newG;
                    if (mHeavyDiagonals && i>3)
                        newG = parentNode.G + (int) (mGrid[newNode.X, newNode.Y] * 2.41);
                    else
                        newG = parentNode.G + mGrid[newNode.X, newNode.Y];


                    if (newG == parentNode.G)
                        continue;

                    if (mPunishChangeDirection)
                    {
                        if ((newNode.X - parentNode.X) != 0)
                        {
                            if (mHoriz == 0)
                                newG += 20;
                        }
                        if ((newNode.Y - parentNode.Y) != 0)
                        {
                            if (mHoriz != 0)
                                newG += 20;

                        }
                    }

                    int     foundInOpenIndex = -1;
                    for(int j=0; j<mOpen.Count; j++)
                    {
                        if (mOpen[j].X == newNode.X && mOpen[j].Y == newNode.Y)
                        {
                            foundInOpenIndex = j;
                            break;
                        }
                    }
                    if (foundInOpenIndex != -1 && mOpen[foundInOpenIndex].G <= newG)
                        continue;

                    int     foundInCloseIndex = -1;
                    for(int j=0; j<mClose.Count; j++)
                    {
                        if (mClose[j].X == newNode.X && mClose[j].Y == newNode.Y)
                        {
                            foundInCloseIndex = j;
                            break;
                        }
                    }
                    if (foundInCloseIndex != -1 && mClose[foundInCloseIndex].G <= newG)
                        continue;

                    newNode.PX      = parentNode.X;
                    newNode.PY      = parentNode.Y;
                    newNode.G       = newG;

                    switch(mFormula)
                    {
                        default:
                        case HeuristicFormula.Manhattan:
                            newNode.H       = mHEstimate * (Abs(newNode.X - end.X) + Abs(newNode.Y - end.Y));
                            break;
                        case HeuristicFormula.MaxDXDY:
                            newNode.H       = mHEstimate * (aMax(newNode.X - end.X, newNode.Y - end.Y));
                            break;
                        case HeuristicFormula.DiagonalShortCut:
                            int h_diagonal  = aMin(newNode.X - end.X, newNode.Y - end.Y);
                            int h_straight  = (Abs(newNode.X - end.X) + Abs(newNode.Y - end.Y));
                            newNode.H       = (mHEstimate * 2) * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                            break;
                        case HeuristicFormula.Euclidean:
                            newNode.H       = (int) (mHEstimate * Math.Sqrt(Math.Pow((newNode.X - end.X) , 2) + Math.Pow((newNode.Y - end.Y), 2)));
                            break;
                        case HeuristicFormula.EuclideanNoSQR:
                            newNode.H       = (int) (mHEstimate * (Math.Pow((newNode.X - end.X) , 2) + Math.Pow((newNode.Y - end.Y), 2)));
                            break;
                        case HeuristicFormula.Custom1:
                            Point dxy       = new Point(end.X, newNode.X, end.Y, newNode.Y);
                            int Orthogonal  = Abs(dxy.X - dxy.Y);
                            int Diagonal    = Abs(((dxy.X + dxy.Y) - Orthogonal) / 2);
                            newNode.H       = mHEstimate * (Diagonal + Orthogonal + dxy.X + dxy.Y);
                            break;
                    }
                    if (mTieBreaker)
                    {
                        int dx1 = parentNode.X - end.X;
                        int dy1 = parentNode.Y - end.Y;
                        int dx2 = start.X - end.X;
                        int dy2 = start.Y - end.Y;
                        int cross = Abs(dx1 * dy2 - dx2 * dy1);
                        newNode.H = (int) (newNode.H + cross * 0.001);
                    }
                    newNode.F       = newNode.G + newNode.H;
                    mOpen.Push(newNode);
                }

                mClose.Add(parentNode);
            }

            if (found)
            {
                PathFinderNode fNode = mClose[mClose.Count - 1];
                for(int i=mClose.Count - 1; i>=0; i--)
                {
                    if (fNode.PX == mClose[i].X && fNode.PY == mClose[i].Y || i == mClose.Count - 1)
                        fNode = mClose[i];
                    else
                        mClose.RemoveAt(i);
                }
                return mClose;
            }
			Debug.Log("Null 2");
            return null;
        }
        #endregion

        #region Inner Classes
        internal class ComparePFNode : IComparer<PathFinderNode>
        {
            #region IComparer Members
            public int Compare(PathFinderNode x, PathFinderNode y)
            {
                if (x.F > y.F)
                    return 1;
                else if (x.F < y.F)
                    return -1;
                return 0;
            }
            #endregion
        }
        #endregion
    }
}
