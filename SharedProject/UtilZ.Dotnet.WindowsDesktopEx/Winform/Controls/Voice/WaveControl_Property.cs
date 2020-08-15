using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 属性
    /// </summary>
    public partial class WaveControl
    {
        /// <summary>
        /// 全局视图图元信息
        /// </summary>
        private GlobalViewPlotElementInfo _globalView = null;
        /// <summary>
        /// 获取或设置全局视图图元信息
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GlobalViewPlotElementInfo GlobalView
        {
            get { return this._globalView; }
            set
            {
                if (this._globalView != null)
                {
                    this._globalView.Dispose();
                }

                this.SetAreaType(value, AreaType.GlobalView);
                this._globalView = value;
                this.UpdateDrawAreaActualSize();
                this.AllDraw();
            }
        }

        /// <summary>
        /// x轴(时间)坐标图元信息
        /// </summary>
        private XAxisPlotElementInfo _xAxis = null;
        /// <summary>
        /// 获取或设置x轴(时间)坐标图元信息
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XAxisPlotElementInfo XAxis
        {
            get { return this._xAxis; }
            set
            {
                if (this._xAxis != null)
                {
                    this._xAxis.Dispose();
                }

                this.SetAreaType(value, AreaType.AxisX);
                this._xAxis = value;
                this.UpdateDrawAreaActualSize();
                this.AllDraw();
            }
        }

        /// <summary>
        /// y坐标图元信息
        /// </summary>
        private YAxisPlotElementInfo _yAxis = null;
        /// <summary>
        /// 获取或设置y坐标图元信息
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public YAxisPlotElementInfo YAxis
        {
            get { return this._yAxis; }
            set
            {
                if (value != null && value.Dock != DockStyle.Left && value.Dock != DockStyle.Right)
                {
                    throw new ArgumentException($"坐标轴停靠方向只能是{nameof(DockStyle.Left)}和{nameof(DockStyle.Right)}");
                }

                if (this._yAxis != null)
                {
                    this._yAxis.Dispose();
                }

                this.SetAreaType(value, AreaType.AxisY);
                this._yAxis = value;
                this.UpdateDrawAreaActualSize();
                this.AllDraw();
            }
        }

        /// <summary>
        /// 内容图元信息
        /// </summary>
        private ContentPlotElementInfo _content = null;
        /// <summary>
        /// 获取或设置内容图元信息
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContentPlotElementInfo Content
        {
            get { return this._content; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._content.Dispose();
                this.SetAreaType(value, AreaType.Content);
                this._content = value;
                this.AllDraw();
            }
        }

        private void SetAreaType(PlotElementInfoAbs plotElementInfo, AreaType areaType)
        {
            if (plotElementInfo != null)
            {
                plotElementInfo.AreaType = areaType;
            }
        }

        /// <summary>
        /// 波形分隔线绘制画笔
        /// </summary>
        private Pen _channelSeparatorPen = new Pen(Color.Red, PlotConstant.DEFAULLINE_WIDTH);
        /// <summary>
        /// 获取或设置分隔线绘制画笔
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Pen ChannelSeparatorPen
        {
            get { return this._channelSeparatorPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._channelSeparatorPen.Dispose();
                this._channelSeparatorPen = value;
            }
        }

        /// <summary>
        /// 选中区域背景画刷
        /// </summary>
        private Brush _seleactionAreaBrush = new SolidBrush(Color.FromArgb(184, 88, 25));
        /// <summary>
        /// 获取或设置选中区域背景画刷
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Brush SeleactionAreaBrush
        {
            get { return this._seleactionAreaBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._seleactionAreaBrush.Dispose();
                this._seleactionAreaBrush = value;
            }
        }

        /// <summary>
        /// 整体视图选中区域背景画刷
        /// </summary>
        private Brush _seleactionGlobalViewAreaBrush = new SolidBrush(Color.FromArgb(245, 245, 255));
        /// <summary>
        /// 获取或设置整体视图选中区域背景画刷
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Brush SeleactionGlobalViewAreaBrush
        {
            get { return this._seleactionGlobalViewAreaBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._seleactionGlobalViewAreaBrush.Dispose();
                this._seleactionGlobalViewAreaBrush = value;
            }
        }

        /// <summary>
        /// 获取或设置背景色
        /// </summary>
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                this.AllDraw();
            }
        }

        /// <summary>
        /// 图形绘制密度,值越大越密,同时绘制波形图也就越慢[建议值为1-5]
        /// </summary>
        private int _drawDensity = 1;
        /// <summary>
        /// 获取或设置波形图密度,值越大越密,同时绘制波形图也就越慢[建议值为1-5],默认为1
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DrawDensity
        {
            get { return this._drawDensity; }
            set
            {
                if (this._drawDensity < 1 || this._drawDensity > 5)
                {
                    throw new ArgumentException("图像质量系数值不能小于1或大于5");
                }

                this._drawDensity = value;
                this.PartDraw_DrawDensityChanged();
            }
        }

        /// <summary>
        /// 自定义时间坐标刻度文本回调
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<DateTime, double, string> CustomTimeAxisLabelFun;

        /// <summary>
        /// 是否绘制播放位置线[true:绘制;false:不绘制]
        /// </summary>
        private bool _playPositionLine = true;
        /// <summary>
        /// 是否绘制播放位置线[true:绘制;false:不绘制;默认为true]
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否绘制播放位置线[true:绘制;false:不绘制;默认为true]")]
        public bool PlayPositionLine
        {
            get { return this._playPositionLine; }
            set
            {
                this._playPositionLine = value;
            }
        }
    }
}
