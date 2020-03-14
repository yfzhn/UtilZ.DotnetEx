using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 线形图
    /// </summary>
    public class LineSeries : LineSeriesBase
    {
        private LineSeriesType _lineSeriesType = LineSeriesType.Bezier;
        /// <summary>
        /// 获取或设置线曲线类型
        /// </summary>
        public LineSeriesType LineSeriesType
        {
            get { return _lineSeriesType; }
            set
            {
                _lineSeriesType = value;
                base.OnRaisePropertyChanged(nameof(LineSeriesType));
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineSeries()
            : base()
        {

        }

        /// <summary>
        /// 创建曲线Geometry
        /// </summary>
        /// <param name="pointInfoListCollection">目标点信息集合</param>
        /// <returns>Geometry</returns>
        protected override Geometry CreatePathGeometry(List<List<PointInfo>> pointInfoListCollection)
        {
            Geometry geometry;
            switch (this._lineSeriesType)
            {
                case LineSeriesType.Bezier:
                    geometry = this.CreateBezierPathGeometry(pointInfoListCollection);
                    break;
                case LineSeriesType.PolyQuadraticBezier:
                    geometry = this.CreatePolyQuadraticBezierPathGeometry(pointInfoListCollection);
                    break;
                case LineSeriesType.QuadraticBezier:
                    geometry = this.CreateQuadraticBezierPathGeometry(pointInfoListCollection);
                    break;
                case LineSeriesType.PolyLine:
                    geometry = this.CreateLinePathGeometry(pointInfoListCollection);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return geometry;
        }


        private PathGeometry CreatePolyQuadraticBezierPathGeometry(List<List<PointInfo>> pointInfoListCollection)
        {
            PathFigureCollection pfc = new PathFigureCollection();
            foreach (var pointInfoList in pointInfoListCollection)
            {
                Point[] pointArr = pointInfoList.Select(t => { return t.Point; }).ToArray();
                var polyQuadraticBezierSegment = new PolyQuadraticBezierSegment();
                polyQuadraticBezierSegment.Points = new PointCollection(pointArr);

                var pathFigure = new PathFigure();
                pathFigure.StartPoint = pointInfoList[0].Point;
                pathFigure.Segments.Add(polyQuadraticBezierSegment);

                pfc.Add(pathFigure);
            }

            return new PathGeometry(pfc);
        }


        private PathGeometry CreateQuadraticBezierPathGeometry(List<List<PointInfo>> pointInfoListCollection)
        {
            PathFigureCollection pfc = new PathFigureCollection();
            foreach (var pointInfoList in pointInfoListCollection)
            {
                var pathFigure = new PathFigure();
                pathFigure.StartPoint = pointInfoList[0].Point;

                for (var i = 1; i < pointInfoList.Count; i++)
                {
                    var quadraticBezierSegment = new QuadraticBezierSegment(pointInfoList[i - 1].Point, pointInfoList[i].Point, true);
                    pathFigure.Segments.Add(quadraticBezierSegment);
                }

                pfc.Add(pathFigure);
            }

            return new PathGeometry(pfc);
        }



        #region CreateBezierPathGeometry
        private PathGeometry CreateBezierPathGeometry(List<List<PointInfo>> pointInfoListCollection)
        {
            PathFigureCollection pfc = new PathFigureCollection();
            foreach (var pointInfoList in pointInfoListCollection)
            {
                PathFigure pathFigure = new PathFigure();
                pathFigure.StartPoint = pointInfoList[0].Point;

                List<Point> controls = new List<Point>();
                for (int i = 0; i < pointInfoList.Count; i++)
                {
                    controls.AddRange(this.CalBezierControlPoint(pointInfoList, i));
                }

                int controlPointIndex;
                for (int i = 1; i < pointInfoList.Count; i++)
                {
                    controlPointIndex = i * 2;
                    BezierSegment bs = new BezierSegment(controls[controlPointIndex - 1], controls[controlPointIndex], pointInfoList[i].Point, true);
                    bs.IsSmoothJoin = true;

                    pathFigure.Segments.Add(bs);
                }

                pfc.Add(pathFigure);
            }

            return new PathGeometry(pfc);
        }

        private List<Point> CalBezierControlPoint(List<PointInfo> list, int index)
        {
            List<Point> point = new List<Point>();
            point.Add(new Point());
            point.Add(new Point());
            if (index == 0)
            {
                point[0] = list.First().Point;
            }
            else
            {
                point[0] = this.Average(list[index - 1].Point, list[index].Point);
            }

            if (index == list.Count - 1)
            {
                point[1] = list.Last().Point;
            }
            else
            {
                point[1] = this.Average(list[index].Point, list[index + 1].Point);
            }

            Point ave = this.Average(point[0], point[1]);
            Point sh = this.Sub(list[index].Point, ave);
            const double mul = 0.6d;
            point[0] = this.Mul(Add(point[0], sh), list[index].Point, mul);
            point[1] = this.Mul(Add(point[1], sh), list[index].Point, mul);
            return point;
        }

        private Point Average(Point x, Point y)
        {
            return new Point((x.X + y.X) / 2, (x.Y + y.Y) / 2);
        }

        private Point Add(Point x, Point y)
        {
            return new Point(x.X + y.X, x.Y + y.Y);
        }

        private Point Sub(Point x, Point y)
        {
            return new Point(x.X - y.X, x.Y - y.Y);
        }

        private Point Mul(Point x, Point y, double mul = 0.6d)
        {
            Point subPoint = this.Sub(x, y);
            //绘制图表时,Y轴值不发生变化,以后如果重构,则根据 图是水平还是垂直做判断,水平方向Y轴为0,垂直方向X轴为0
            var temp = new Point(subPoint.X * mul, subPoint.Y * mul);
            //var temp = new Point(subPoint.X * mul, 0);
            return this.Add(y, temp);
        }
        #endregion


        private PathGeometry CreateLinePathGeometry(List<List<PointInfo>> pointInfoListCollection)
        {
            PathFigureCollection pfc = new PathFigureCollection();
            foreach (var pointInfoList in pointInfoListCollection)
            {
                Point[] pointArr = pointInfoList.Select(t => { return t.Point; }).ToArray();
                var polyLineSegment = new PolyLineSegment();
                polyLineSegment.Points = new PointCollection(pointArr);

                var pathFigure = new PathFigure();
                pathFigure.StartPoint = pointInfoList[0].Point;
                pathFigure.Segments.Add(polyLineSegment);
                pfc.Add(pathFigure);
            }

            return new PathGeometry(pfc);
        }
    }

    /// <summary>
    /// 线类型枚举
    /// </summary>
    public enum LineSeriesType
    {
        /// <summary>
        /// 特点:比较硬 
        /// </summary>
        PolyLine,

        /// <summary>
        /// 特点:追加点时最后一节线的趋势走向与追加之前会发生变化
        /// </summary>
        Bezier,

        /// <summary>
        /// 特点:线与点会发生偏移
        /// </summary>
        PolyQuadraticBezier,

        /// <summary>
        /// 特点:转角很尖锐
        /// </summary>
        QuadraticBezier
    }
}
