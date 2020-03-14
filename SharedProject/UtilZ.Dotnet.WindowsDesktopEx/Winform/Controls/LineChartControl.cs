using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 使用率控件,类似任务管理器
    /// </summary>
    public class LineChartControl : PaintBaseControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LineChartControl()
            : base()
        {
            this.CalMaxPointCount();
        }

        /// <summary>
        /// OnSizeChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.CalMaxPointCount();
            base.OnSizeChanged(e);
        }

        #region 外部属性
        /// <summary>
        /// 表格线条绘制画笔
        /// </summary>
        private Pen _gridLinePen = new Pen(Color.SeaGreen, 1.0f);

        /// <summary>
        /// 表格线条宽度
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("表格线条宽度")]
        public float GridLineWidth
        {
            get { return this._gridLinePen.Width; }
            set
            {
                this.CheckInvokeRequired();

                if (value < 0.0001)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var oldPen = this._gridLinePen;
                this._gridLinePen = new Pen(oldPen.Color, value);
                oldPen.Dispose();
                this.CustomerPaint();
            }
        }

        /// <summary>
        /// 表格线条颜色
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("表格线条颜色")]
        public Color GridLineColor
        {
            get { return this._gridLinePen.Color; }
            set
            {
                this.CheckInvokeRequired();
                var oldPen = this._gridLinePen;
                this._gridLinePen = new Pen(value, oldPen.Width);
                oldPen.Dispose();
                this.CustomerPaint();
            }
        }

        private bool _isMoveGrid = true;
        /// <summary>
        /// 是否移动背景表格
        /// </summary>
        [SettingsBindable(true)]
        [Description("true:移动表格;false:不移动表格")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsMoveGrid
        {
            get { return _isMoveGrid; }
            set
            {
                this.CheckInvokeRequired();
                if (_isMoveGrid == value)
                {
                    return;
                }

                _isMoveGrid = value;
                if (!_isMoveGrid)
                {
                    this._gridOffset = 0;
                }
            }
        }

        /// <summary>
        /// 绘制方向[true:自左向右;false:自右向左]
        /// </summary>
        private bool _drawDirection = false;
        /// <summary>
        /// 绘制方向[true:自左向右;false:自右向左]
        /// </summary>
        [SettingsBindable(true)]
        [Description("true:自左向右;false:自右向左")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool DrawDirection
        {
            get { return _drawDirection; }
            set
            {
                this.CheckInvokeRequired();
                if (_drawDirection == value)
                {
                    return;
                }

                _drawDirection = value;
                this.CustomerPaint();
            }
        }

        /// <summary>
        /// 是否显示背景表格[true:绘制;false:不绘制]
        /// </summary>
        private bool _showGrid = true;
        /// <summary>
        /// 是否绘制背景表格[true:绘制;false:不绘制]
        /// </summary>
        [SettingsBindable(true)]
        [Description("true:绘制;false:不绘制")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowGrid
        {
            get { return _showGrid; }
            set
            {
                this.CheckInvokeRequired();
                if (_showGrid == value)
                {
                    return;
                }

                _showGrid = value;
                this.CustomerPaint();
            }
        }

        /// <summary>
        /// 是否显示标题
        /// </summary>
        private bool _showTitle = true;
        /// <summary>
        /// 是否显示标题[true:显示;false:不显示]
        /// </summary>
        [SettingsBindable(true)]
        [Description("是否显示标题[true:显示;false:不显示]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get { return _showTitle; }
            set
            {
                this.CheckInvokeRequired();
                if (_showTitle == value)
                {
                    return;
                }

                _showTitle = value;
                this.CustomerPaint();
            }
        }
        #endregion

        private const int _POINT_DISTANCE = 3;
        private const float _PRE = 0.0000001f;//浮点数精度
        private const int _GRID_SIZE = 13;
        private const int _MINUS_GRID_SIZE = 0 - _GRID_SIZE;
        private int _maxCount = 100;
        private float _gridOffset = 0;
        private readonly Font _font = new Font("宋体", 9);

        private void CalMaxPointCount()
        {
            this._maxCount = this.Width / _POINT_DISTANCE;
        }

        /// <summary>
        /// 要绘制的线Hashtable[key:线标识;value:UsageChannel]
        /// </summary>
        private readonly Hashtable _htLine = new Hashtable();

        #region 线管理
        /// <summary>
        /// 获取要绘制的线数量
        /// </summary>
        public int LineCount
        {
            get
            {
                this.CheckInvokeRequired();
                return this._htLine.Count;
            }
        }

        /// <summary>
        /// 添加要绘制的线
        /// </summary>
        /// <param name="lines">要绘制的线集合</param>
        public void AddLine(IEnumerable<CharLine> lines)
        {
            this.CheckInvokeRequired();
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            if (lines.Count() == 0)
            {
                throw new ArgumentException("样式不能为空", nameof(lines));
            }

            foreach (var line in lines)
            {
                if (line == null)
                {
                    throw new ArgumentNullException("样式数组包含为null的项");
                }

                if (this._htLine.ContainsKey(line.Id))
                {
                    throw new ArgumentException($"已存在标识为[{line.Id}]的线");
                }

                this._htLine.Add(line.Id, line);
            }

            this.CustomerPaint();
        }

        /// <summary>
        /// 添加要绘制的线
        /// </summary>
        /// <param name="line">要绘制的线</param>
        public void AddLine(CharLine line)
        {
            this.CheckInvokeRequired();
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            if (this._htLine.ContainsKey(line.Id))
            {
                throw new ArgumentException($"已存在标识为[{line.Id}]的线");
            }

            this._htLine.Add(line.Id, line);
            this.CustomerPaint();
        }

        /// <summary>
        /// 是否存在指定标识的线[存在返回true;不存在返回false]
        /// </summary>
        /// <param name="lineId">线标识</param>
        /// <returns>存在返回true;不存在返回false</returns>
        public bool ExistLine(object lineId)
        {
            this.CheckInvokeRequired();
            return this._htLine.ContainsKey(lineId);
        }

        /// <summary>
        /// 移除指定绘制的线
        /// </summary>
        /// <param name="line">线</param>
        public void RemoveLine(CharLine line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            this.RemoveLineById(line.Id);
        }

        /// <summary>
        /// 移除指定绘制的线
        /// </summary>
        /// <param name="lineId">线标识</param>
        public void RemoveLineById(object lineId)
        {
            this.CheckInvokeRequired();

            if (this._htLine.ContainsKey(lineId))
            {
                ((CharLine)this._htLine[lineId]).Dispose();
                this._htLine.Remove(lineId);
                this.CustomerPaint();
            }
        }

        /// <summary>
        /// 根据线标识获取指定绘制的线
        /// </summary>
        /// <returns>绘制的线</returns>
        public CharLine GetLineById(object lineId)
        {
            this.CheckInvokeRequired();
            if (this._htLine.ContainsKey(lineId))
            {
                return (CharLine)this._htLine[lineId];
            }
            else
            {
                throw new ArgumentException($"不存在标识为[{lineId}]的线");
            }
        }

        /// <summary>
        /// 获取所有绘制的线
        /// </summary>
        /// <returns>绘制的线数组</returns>
        public CharLine[] GetAllLine()
        {
            this.CheckInvokeRequired();
            CharLine[] lines = new CharLine[this._htLine.Count];
            this._htLine.Values.CopyTo(lines, 0);
            return lines;
        }

        /// <summary>
        /// 清空所有的线
        /// </summary>
        public void ClearLine()
        {
            this.CheckInvokeRequired();
            this.DisposeChannel();
            this.CustomerPaint();
        }

        /// <summary>
        /// 清空所有数据并刷新UI
        /// </summary>
        public void ClearData()
        {
            this.CheckInvokeRequired();
            if (this.PrimitiveClearData())
            {
                this.CustomerPaint();
            }
        }

        /// <summary>
        /// 清空所有数据,但不刷新UI
        /// </summary>
        public void ClearDataNoRefresh()
        {
            this.CheckInvokeRequired();
            this.PrimitiveClearData();
        }

        /// <summary>
        /// 有数据清空返回true;没有清空项返回false
        /// </summary>
        /// <returns></returns>
        private bool PrimitiveClearData()
        {
            if (this._htLine.Count > 0)
            {
                bool result = false;
                foreach (CharLine channel in this._htLine.Values)
                {
                    if (channel.ValueList.Count > 0)
                    {
                        channel.ValueList.Clear();
                        result = true;
                    }
                }

                return result;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 添加值
        private bool _isBeingAdd = false;
        /// <summary>
        /// 开始添加值,谨慎使用
        /// </summary>
        /// <param name="lineId">线标识</param>
        /// <param name="value">值</param>
        public void AddValueBegin(object lineId, float value)
        {
            this.CheckInvokeRequired();
            if (this._htLine.ContainsKey(lineId))
            {
                if (!this._isBeingAdd)
                {
                    this._isBeingAdd = true;
                }

                ((CharLine)this._htLine[lineId]).ValueList.Add(value);
            }
            else
            {
                throw new ArgumentException($"不存在标识为[{lineId}]的线");
            }
        }

        /// <summary>
        /// 开始添加值
        /// </summary>
        /// <param name="lineId">线标识</param>
        /// <param name="values">值集合</param>
        public void AddValueBegin(object lineId, IEnumerable<float> values)
        {
            this.CheckInvokeRequired();
            if (this._htLine.ContainsKey(lineId))
            {
                if (!this._isBeingAdd)
                {
                    this._isBeingAdd = true;
                }

                ((CharLine)this._htLine[lineId]).ValueList.AddRange(values);
            }
            else
            {
                throw new ArgumentException($"不存在标识为[{lineId}]的线");
            }
        }

        /// <summary>
        /// 添加值完成,谨慎使用
        /// </summary>
        /// <param name="addCount">添加数项</param>
        public void AddValueEnd(int addCount)
        {
            this.CheckInvokeRequired();
            if (!this._isBeingAdd)
            {
                return;
            }

            this._isBeingAdd = false;

            var maxCount = this._maxCount;
            List<float> valueList;
            foreach (CharLine line in this._htLine.Values)
            {
                valueList = line.ValueList;
                while (valueList.Count > maxCount)
                {
                    valueList.RemoveAt(0);
                }
            }

            this._gridOffset += _POINT_DISTANCE * addCount;
            while (this._gridOffset >= _GRID_SIZE)
            {
                this._gridOffset -= _GRID_SIZE;
            }

            this.CustomerPaint();
        }

        private void CheckBeginAdd()
        {
            if (this._isBeingAdd)
            {
                throw new InvalidOperationException($"调用了[AddValueBegin]方法,未调用[AddValueEnd]方法");
            }
        }

        /// <summary>
        /// 添加值
        /// </summary>
        /// <param name="values">值集合</param>
        public void AddValue(IEnumerable<KeyValuePair<object, float>> values)
        {
            if (values == null || values.Count() == 0)
            {
                return;
            }

            var valueCount = values.Count();
            if (valueCount == 0)
            {
                return;
            }

            this.CheckInvokeRequired();
            this.CheckBeginAdd();

            if (valueCount != this._htLine.Count)
            {
                throw new InvalidOperationException($"线条件数[{this._htLine.Count}]与值数组长度[{valueCount}]不匹配");
            }

            foreach (var kv in values)
            {
                if (this._htLine.ContainsKey(kv.Key))
                {
                    ((CharLine)this._htLine[kv.Key]).ValueList.Add(kv.Value);
                }
                else
                {
                    throw new ArgumentException($"不存在标识为[{kv.Key}]的线");
                }
            }

            this.CustomerPaint();
        }

        /// <summary>
        /// 添加值
        /// </summary>
        /// <param name="values">值集合</param>
        public void AddValue(IEnumerable<KeyValuePair<object, float[]>> values)
        {
            if (values == null)
            {
                return;
            }

            var valueCount = values.Count();
            if (valueCount == 0)
            {
                return;
            }

            var valueLen = values.ElementAt(0).Value.Length;
            if (values.Where(t => { return t.Value.Length != valueLen; }).Count() > 0)
            {
                throw new ArgumentException("数组长度不全相同");
            }

            if (valueLen == 0)
            {
                return;
            }

            this.CheckInvokeRequired();

            if (valueCount != this._htLine.Count)
            {
                throw new InvalidOperationException($"线条数[{this._htLine.Count}]与值数组长度[{valueCount}]不匹配");
            }

            this.CheckBeginAdd();

            foreach (var kv in values)
            {
                if (this._htLine.ContainsKey(kv.Key))
                {
                    ((CharLine)this._htLine[kv.Key]).ValueList.AddRange(kv.Value);
                }
                else
                {
                    throw new ArgumentException($"不存在标识为[{kv.Key}]的线");
                }
            }

            this.CustomerPaint();
        }

        /// <summary>
        /// 添加值,值数组中的项与添加线时的顺序必须保持一致,谨慎使用
        /// </summary>
        /// <param name="values">值集合</param>
        public void AddValue(float[] values)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            if (values.Length != this._htLine.Count)
            {
                throw new InvalidOperationException($"线条数[{this._htLine.Count}]与值数组长度[{values.Length}]不匹配");
            }

            this.CheckInvokeRequired();
            this.CheckBeginAdd();

            var maxCount = this._maxCount;
            List<float> valueList;
            int index = 0;

            foreach (CharLine channel in this._htLine.Values)
            {
                valueList = channel.ValueList;
                valueList.Add(values[index++]);
                while (valueList.Count > maxCount)
                {
                    valueList.RemoveAt(0);
                }
            }

            this._gridOffset += _POINT_DISTANCE;
            while (this._gridOffset >= _GRID_SIZE)
            {
                this._gridOffset -= _GRID_SIZE;
            }

            this.CustomerPaint();
        }

        /// <summary>
        /// 添加值数组,值中一维数组中的项与添加线时的顺序必须保持一致,谨慎使用
        /// </summary>
        /// <param name="values">值数组</param>
        public void AddValue(float[][] values)
        {
            if (values == null)
            {
                return;
            }

            var len = values.GetLength(0);
            if (len == 0 || values[0].Length == 0)
            {
                return;
            }

            if (len != this._htLine.Count)
            {
                throw new InvalidOperationException($"线条数[{this._htLine.Count}]与值数组长度[{len}]不匹配");
            }

            this.CheckInvokeRequired();
            this.CheckBeginAdd();

            var maxCount = this._maxCount;
            List<float> valueList;
            int index = 0;

            foreach (CharLine channel in this._htLine.Values)
            {
                valueList = channel.ValueList;
                valueList.AddRange(values[index++]);
                while (valueList.Count > maxCount)
                {
                    valueList.RemoveAt(0);
                }
            }

            this._gridOffset += _POINT_DISTANCE * values[0].Length;
            while (this._gridOffset >= _GRID_SIZE)
            {
                this._gridOffset -= _GRID_SIZE;
            }

            this.CustomerPaint();
        }
        #endregion

        #region 绘制图
        /// <summary>
        /// 自定义绘制
        /// </summary>
        protected override void CustomerPaint()
        {
            try
            {
                if (this._grafx == null)
                {
                    return;
                }

                Graphics graphics = this._grafx.Graphics;
                //重置平移
                graphics.ResetTransform();

                //重置平移
                graphics.ResetTransform();

                //清空所有已绘制的图形
                graphics.Clear(this.BackColor);

                float width = this.Width, height = this.Height;
                if (width < _PRE || height < _PRE)
                {
                    return;
                }

                var linePen = this._gridLinePen;
                if (linePen.Width - _GRID_SIZE > _PRE)
                {
                    return;
                }

                ICollection lineCollection = this._htLine.Values;
                float lineAreaWidth;

                if (this._showTitle)
                {
                    const int TITLE_WIDTH = 100;
                    lineAreaWidth = width - TITLE_WIDTH;
                    this.DrawTitle(graphics, lineAreaWidth, width, height, lineCollection);
                }
                else
                {
                    lineAreaWidth = width;
                }

                //float minValue, maxValue;
                //this.GetBestValue(out minValue, out maxValue);

                //绘制表格
                if (this._drawDirection)
                {
                    this.LeftToRightDraw(graphics, lineAreaWidth, height, linePen, lineCollection);
                }
                else
                {
                    this.RightToLeftDraw(graphics, lineAreaWidth, height, linePen, lineCollection);
                }

                this.Refresh();
                this.Update();
            }
            catch (Exception ex)
            {
                Loger.Error(ex, "绘图异常");
            }
        }

        private void DrawTitle(Graphics graphics, float minX, float maxX, float height, ICollection lineCollection)
        {
            const int BOUNDARY_INTERVAL = 2;
            const int TITLE_LINE_LEN = 20;

            float lineX1 = minX + BOUNDARY_INTERVAL;
            float lineX2 = lineX1 + TITLE_LINE_LEN;
            float textX = lineX2 + BOUNDARY_INTERVAL;


            const int TITLE_LINE_Y_OFFSET = 15;
            const int TITLE_TEXT_LINE_Y_OFFSET = 5;
            const int TITLE_LINE_HEIGHT = 20;
            const int TILE_NAME_CHAR_LEN = 4;
            const int TILE_NAME_TEXT_LEN = 53;

            float lineY = height - TITLE_LINE_Y_OFFSET;
            float textY;

            string str, tmpStr;
            foreach (CharLine line in lineCollection)
            {
                //线
                graphics.DrawLine(line.LinePen, lineX1, lineY, lineX2, lineY);

                //名称
                str = line.Name;
                if (!string.IsNullOrWhiteSpace(str))
                {
                    if (str.Length > TILE_NAME_CHAR_LEN)
                    {
                        int len = TILE_NAME_CHAR_LEN;
                        int lenTmp = TILE_NAME_CHAR_LEN;

                        while (lenTmp < str.Length)
                        {
                            tmpStr = str.Substring(0, lenTmp);
                            if (graphics.MeasureString(tmpStr, this._font).Width > TILE_NAME_TEXT_LEN)
                            {
                                break;
                            }

                            len = lenTmp;
                            lenTmp += 1;
                        }

                        str = str.Substring(0, len) + "....";
                    }

                    textY = lineY - TITLE_TEXT_LINE_Y_OFFSET;
                    var brush = new SolidBrush(line.LinePen.Color);
                    graphics.DrawString(str, this._font, brush, new PointF(textX, textY));
                    brush.Dispose();
                }

                lineY = lineY - TITLE_LINE_HEIGHT;
            }
        }

        private void RightToLeftDraw(Graphics graphics, float width, float height, Pen linePen, ICollection lineCollection)
        {
            if (this._showGrid)
            {
                //绘制横线
                float x1 = 0, x2 = width, y = height - linePen.Width;
                while (y > _PRE)
                {
                    graphics.DrawLine(linePen, x1, y, x2, y);
                    y -= _GRID_SIZE;
                }

                //绘制竖线
                float y1 = 0, y2 = height, x = width;
                if (this._isMoveGrid)
                {
                    //移动表格
                    x -= this._gridOffset;
                }

                while (x > _PRE)
                {
                    graphics.DrawLine(linePen, x, y1, x, y2);
                    x = x - _GRID_SIZE;
                }
            }

            //绘制使用率线
            float pointDistance = width / this._maxCount;
            foreach (CharLine channel in lineCollection)
            {
                this.RightToLeftDrawLine(graphics, channel, pointDistance, width, height);
            }
        }

        private void RightToLeftDrawLine(Graphics graphics, CharLine channel, float pointDistance, float width, float height)
        {
            //绘制使用率线
            var values = channel.ValueList;
            if (values.Count > 1)
            {
                var usageLinePen = channel.LinePen;
                var points = new PointF[values.Count];
                float vx = width - usageLinePen.Width, vy;
                for (int i = values.Count - 1; i >= 0; i--)
                {
                    vy = height - values[i] * height / 100;
                    points[i] = new PointF(vx, vy);
                    vx = vx - pointDistance;
                    if (vx < _PRE)
                    {
                        break;
                    }
                }

                graphics.DrawLines(usageLinePen, points);
            }
        }

        private void LeftToRightDraw(Graphics graphics, float width, float height, Pen linePen, ICollection lineCollection)
        {
            if (this._showGrid)
            {
                //绘制横线
                float x1 = 0, x2 = width, y = height - linePen.Width;
                while (y > _PRE)
                {
                    graphics.DrawLine(linePen, x1, y, x2, y);
                    y -= _GRID_SIZE;
                }

                //绘制竖线
                float y1 = 0, y2 = height, x = 0;
                if (this._isMoveGrid)
                {
                    //移动表格
                    x += this._gridOffset;
                }

                while (width - x > _PRE)
                {
                    graphics.DrawLine(linePen, x, y1, x, y2);
                    x = x + _GRID_SIZE;
                }
            }

            //绘制使用率线
            float pointDistance = width / this._maxCount;
            foreach (CharLine channel in lineCollection)
            {
                this.LeftToRightDrawLine(graphics, channel, pointDistance, width, height);
            }
        }

        private void LeftToRightDrawLine(Graphics graphics, CharLine channel, float pointDistance, float width, float height)
        {
            //绘制使用率线
            var values = channel.ValueList;
            if (values.Count > 1)
            {
                var usageLinePen = channel.LinePen;
                var points = new PointF[values.Count];
                float vx = 0, vy;
                for (int i = values.Count - 1; i >= 0; i--)
                {
                    vy = height - values[i] * height / 100;
                    points[i] = new PointF(vx, vy);
                    vx = vx + pointDistance;
                    if (vx - width > _PRE)
                    {
                        break;
                    }
                }

                graphics.DrawLines(usageLinePen, points);
            }
        }

        private void GetBestValue(out float minValue, out float maxValue)
        {
            minValue = 0;
            maxValue = 0;
            float tmp;
            foreach (CharLine channel in this._htLine.Values)
            {
                tmp = channel.ValueList.Min();
                if (minValue - tmp > _PRE)
                {
                    minValue = tmp;
                }

                tmp = channel.ValueList.Max();
                if (tmp - maxValue > _PRE)
                {
                    maxValue = tmp;
                }
            }
        }
        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.DisposeChannel();
            this._font.Dispose();
        }

        private void DisposeChannel()
        {
            foreach (CharLine channel in this._htLine.Values)
            {
                channel.Dispose();
            }

            this._htLine.Clear();
        }
    }

    /// <summary>
    /// 要绘制线
    /// </summary>
    public class CharLine : IDisposable
    {
        /// <summary>
        /// 线标识
        /// </summary>
        public object Id { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 线绘制画笔
        /// </summary>
        private Pen _linePen;

        internal Pen LinePen
        {
            get { return _linePen; }
        }

        ///// <summary>
        ///// 线宽度
        ///// </summary>
        //public float LineWidth
        //{
        //    get { return this._linePen.Width; }
        //    set
        //    {
        //        if (value < 0.0001)
        //        {
        //            throw new ArgumentOutOfRangeException();
        //        }

        //        var oldPen = this._linePen;
        //        this._linePen = new Pen(oldPen.Color, value);
        //        oldPen.Dispose();
        //    }
        //}

        ///// <summary>
        ///// 线颜色
        ///// </summary>
        //public Color LineColor
        //{
        //    get { return this._linePen.Color; }
        //    set
        //    {
        //        var oldPen = this._linePen;
        //        this._linePen = new Pen(value, oldPen.Width);
        //        oldPen.Dispose();
        //    }
        //}

        private readonly List<float> _valueList = new List<float>();
        /// <summary>
        /// 值列表
        /// </summary>
        internal List<float> ValueList
        {
            get { return _valueList; }
        }

        /// <summary>
        /// 构造函数,使用默认参数
        /// </summary>
        /// <param name="Id">线标识,使用hash值</param>
        /// <param name="name">名称</param>
        public CharLine(object Id, string name)
            : this(Id, name, Color.Lime, 1.0f)
        {

        }

        /// <summary>
        /// 构造函数,使用指定参数
        /// </summary>
        /// <param name="Id">线标识,使用hash值</param>
        /// <param name="name">名称</param>
        /// <param name="lineColor">线颜色</param>
        /// <param name="lineWidth">线宽度</param>
        public CharLine(object Id, string name, Color lineColor, float lineWidth)
        {
            this.Id = Id;
            this.Name = name;
            this._linePen = new Pen(lineColor, lineWidth);
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            try
            {
                this._linePen.Dispose();
            }
            catch
            { }
        }
    }
}
