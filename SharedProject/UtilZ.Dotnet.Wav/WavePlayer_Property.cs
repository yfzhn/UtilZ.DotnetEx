using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Wav.ExBass;
using UtilZ.Dotnet.Wav.Model;

namespace UtilZ.Dotnet.Wav
{
    // WAV播放控件-属性
    public partial class WavePlayer
    {
        /// <summary>
        /// 是否对bass.dll进行版本验证
        /// </summary>
        private bool _isVersionValidate = true;

        /// <summary>
        /// 获取或设置是否对bass.dll进行版本验证[true:验证;false:不验证;默认为true]
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("是否对bass.dll进行版本验证[true:验证;false:不验证;默认为true]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsVersionValidate
        {
            get { return _isVersionValidate; }
            set { _isVersionValidate = value; }
        }

        /// <summary>
        /// 初始化采样率
        /// </summary>
        private volatile int _freq = 44100;

        /// <summary>
        /// 获取或设置初始化采样率
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("是否对bass.dll进行版本验证[true:验证;false:不验证;默认为true]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Freq
        {
            get { return _freq; }
            set { _freq = value; }
        }

        /// <summary>
        /// 获取当前播放状态
        /// </summary>
        public PlayStatus GrtPlayState()
        {
            PlayStatus status;
            int value = Bass.BASS_ChannelIsActive(this._handle);
            switch (value)
            {
                case 0:
                    status = PlayStatus.STOPPED;
                    break;
                case 1:
                    status = PlayStatus.PLAYING;
                    break;
                case 2:
                    status = PlayStatus.STALLED;
                    break;
                case 3:
                    status = PlayStatus.PAUSED;
                    break;
                default:
                    throw new ApplicationException("未知的状态" + value.ToString());
            }

            return status;
        }

        /// <summary>
        /// 获取或设置音量大小
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Volume
        {
            get
            {
                return (int)(Bass.BASS_GetVolume() * 100);
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format("音量值范围为0-100,值{0}无效", value));
                }

                Bass.BASS_SetVolume((float)value / 100);
            }
        }

        /// <summary>
        /// 获取或设置当前获取当前播放位置,单位/秒
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Position
        {
            get
            {
                if (this._handle == -1)
                {
                    return 0;
                }

                return Bass.BASS_ChannelBytes2Seconds(this._handle, Bass.BASS_ChannelGetPosition(this._handle, BASSMode.BASS_POS_BYTE));
            }
            set
            {
                if (this._handle == -1)
                {
                    return;
                }

                Bass.BASS_ChannelSetPosition(this._handle, Bass.BASS_ChannelSeconds2Bytes(this._handle, value), BASSMode.BASS_POS_BYTE);
            }
        }

        /// <summary>
        /// 获取或设置播放速度
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float PlaySpeed
        {
            get
            {
                if (this._handle == -1)
                {
                    return 0;
                }
                else
                {
                    return Bass.BASS_ChannelGetAttribute(this._handle, BASSAttribute.BASS_ATTRIB_FREQ) / this._freq;
                }
            }
            set
            {
                if (this._handle == -1)
                {
                    return;
                }

                Bass.BASS_ChannelSetAttribute(this._handle, BASSAttribute.BASS_ATTRIB_FREQ, value * this._freq);
            }
        }

        /// <summary>
        /// 播放位置指示线刷新间隔,单位毫秒[值太大或太小都可能会出现卡顿现象]
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("播放位置指示线刷新间隔,单位毫秒[值太大或太小都可能会出现卡顿现象]")]
        public int PlayLocationLineRefreshInterval
        {
            get { return _playLocationLineTimer.Interval; }
            set { _playLocationLineTimer.Interval = value; }
        }

        /// <summary>
        /// 是否自动播放
        /// </summary>
        private volatile bool _autoPlay = false;

        /// <summary>
        /// 获取或设置是否自动播放
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoPlay
        {
            get { return _autoPlay; }
            set { _autoPlay = value; }
        }

        /// <summary>
        /// 是否开启环听
        /// </summary>
        private volatile bool _isRingHear = false;

        /// <summary>
        /// 获取或设置是否是否开启环听[即当播放到选中区域时,则循环播放该区域]
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("是否是否开启环听[即当播放到选中区域时,则循环播放该区域]")]
        public bool IsRingHear
        {
            get { return _isRingHear; }
            set { _isRingHear = value; }
        }

        /// <summary>
        /// 图像质量系数,值越大质量越高,同时绘制波形图也就越慢[建议值为1-5]
        /// </summary>
        private int _qualityCoefficient = 1;

        /// <summary>
        /// 获取或设置图像质量系数,值越大质量越高,同时绘制波形图也就越慢[建议值为1-3],默认为1
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("图像质量系数,值越大质量越高,同时绘制波形图也就越慢[建议值为1-3]")]
        public int QualityCoefficient
        {
            get { return _qualityCoefficient; }
            set
            {
                if (_qualityCoefficient < 1 || _qualityCoefficient > 5)
                {
                    throw new ArgumentException("图像质量系数值不能小于1或大于5");
                }

                _qualityCoefficient = value;

                //添加缩略波形图区域和主波形图区域为需要更新的区域
                this.AddPartRefreshArea(this._zoomArea);
                this.AddPartRefreshArea(this._wavArea);

                //当前控件大小改变后,刷新波形图
                this.RefreshWave(true, false, false, false, true, true);
            }
        }

        /// <summary>
        /// 获取或设置绘制波形图时间间隔,值越小绘制的越频繁,越耗CPU[默认为100毫秒]
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("获取或设置绘制波形图时间间隔,值越小绘制的越频繁,越耗CPU[默认为100毫秒]")]
        public int DrawInterval
        {
            get { return this._playLocationLineTimer.Interval; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("绘制波形图时间间隔不能为负数");
                }

                if (this._playLocationLineTimer.Interval == value)
                {
                    return;
                }

                this._playLocationLineTimer.Interval = value;
            }
        }

        /// <summary>
        /// 文件持续播放时间
        /// </summary>
        private double _durationTime = 0d;

        /// <summary>
        /// 获取文件持续播放时间
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double DurationTime
        {
            get { return this._durationTime; }
        }

        /// <summary>
        /// 当前文件是否是立体声,即双声道[true:双声道;false:单声道]
        /// </summary>
        private bool _isStereo = false;

        /// <summary>
        /// 获取当前文件是否是立体声,即双声道[true:双声道;false:单声道]
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsStereo
        {
            get { return _isStereo; }
        }

        /// <summary>
        /// 播放的音频文件
        /// </summary>
        private string _fileName = null;

        /// <summary>
        /// 获取或设置播放的音频文件
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "文件路径不能为空或null");
                }

                if (!File.Exists(value))
                {
                    throw new FileNotFoundException("音频文件不存在", value);
                }

                //加载播放文件
                this.LoadFile(value);
                this._fileName = value;
            }
        }

        /// <summary>
        /// 是否合并左右声道波形图[true:合并;false:分离]
        /// </summary>
        private bool _isMergeChanel = false;

        /// <summary>
        /// 获取或设置是否合并左右声道波形图[true:合并;false:分离]
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否合并左右声道波形图[true:合并;false:分离]")]
        public bool IsMergeChanel
        {
            get { return _isMergeChanel; }
            set
            {
                if (_isMergeChanel == value)
                {
                    return;
                }

                _isMergeChanel = value;

                //添加缩略波形图区域和主波形图区域为需要更新的区域
                this.AddPartRefreshArea(this._zoomArea);
                this.AddPartRefreshArea(this._wavArea);
                this.AddPartRefreshArea(this._dbArea);
                //当是否合并左右声道波形图改变后,刷新波形图
                this.RefreshWave(true, false, true, false, false, false);
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backgroudColor = Color.Black;

        /// <summary>
        /// 获取或设置背景色
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("波形背景色")]
        public Color BackgroudColor
        {
            get { return _backgroudColor; }
            set { _backgroudColor = value; }
        }

        /// <summary>
        /// 主波形区域背景画刷
        /// </summary>
        private Brush _wavAreaBackgroundBrush = new SolidBrush(Color.FromArgb(38, 32, 57));

        /// <summary>
        /// 获取或设置主波形区域背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("主波形区域背景画刷")]
        public Brush WavAreaBackgroundBrush
        {
            get { return _wavAreaBackgroundBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _wavAreaBackgroundBrush.Dispose();
                _wavAreaBackgroundBrush = value;
            }
        }

        /// <summary>
        /// 是否启用主波形图区域背景
        /// </summary>
        private volatile bool _enableWavAreaBackground = false;

        /// <summary>
        /// 获取或设置是否启用主波形图区域背景
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否启用主波形图区域背景")]
        public bool EnableWavAreaBackground
        {
            get { return _enableWavAreaBackground; }
            set { _enableWavAreaBackground = value; }
        }

        /// <summary>
        /// 主波形选中区域背景画刷
        /// </summary>
        private Brush _seleactionAreaBrush = new SolidBrush(Color.FromArgb(184, 88, 25));

        /// <summary>
        /// 获取或设置主波形选中区域背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("主波形选中区域背景画刷")]
        public Brush SeleactionAreaBrush
        {
            get { return _seleactionAreaBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _seleactionAreaBrush.Dispose();
                _seleactionAreaBrush = value;
            }
        }

        /// <summary>
        /// 左声道波形绘制画笔
        /// </summary>
        private Pen _leftChannelPen = new Pen(Color.LimeGreen, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置左声道波形绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("左声道波形绘制画笔")]
        public Pen LeftChannelPen
        {
            get { return _leftChannelPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _leftChannelPen.Dispose();
                _leftChannelPen = value;
            }
        }

        /// <summary>
        /// 右声道波形绘制画笔
        /// </summary>
        private Pen _rightChannelPen = new Pen(Color.DarkSeaGreen, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置右声道波形绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("右声道波形绘制画笔")]
        public Pen RightChannelPen
        {
            get { return _rightChannelPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _rightChannelPen.Dispose();
                _rightChannelPen = value;
            }
        }

        /// <summary>
        /// 波形中线绘制画笔
        /// </summary>
        private Pen _wavMidLinePen = new Pen(Color.Red, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置波形中线绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("波形中线绘制画笔")]
        public Pen WavMidLinePen
        {
            get { return _wavMidLinePen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _wavMidLinePen.Dispose();
                _wavMidLinePen = value;
            }
        }

        /// <summary>
        /// 声道分隔线绘制画笔
        /// </summary>
        private Pen _channelDivideLinePen = new Pen(Color.Gray, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置声道分隔线绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("声道分隔线绘制画笔")]
        public Pen ChannelDivideLinePen
        {
            get { return _channelDivideLinePen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _channelDivideLinePen.Dispose();
                _channelDivideLinePen = value;
            }
        }

        /// <summary>
        /// 是否绘制声道分隔线[仅声道波形分离绘制时有效]
        /// </summary>
        private bool _isDrawChannelDivideLine = true;

        /// <summary>
        /// 获取或设置是否绘制声道分隔线[仅声道波形分离绘制时有效]
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否绘制声道分隔线[仅声道波形分离绘制时有效]")]
        public bool IsDrawChannelDivideLine
        {
            get { return _isDrawChannelDivideLine; }
            set { _isDrawChannelDivideLine = value; }
        }

        /// <summary>
        /// 是否绘制波形中线
        /// </summary>
        private bool _isDrawWavMidLine = true;

        /// <summary>
        /// 获取或设置是否绘制波形中线[true:绘制;false:不绘制]
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否绘制波形中线[true:绘制;false:不绘制]")]
        public bool IsDrawWavMidLine
        {
            get { return _isDrawWavMidLine; }
            set { _isDrawWavMidLine = value; }
        }

        /// <summary>
        /// 播放位置线绘制画笔
        /// </summary>
        private Pen _playLineChannelPen = new Pen(Color.Yellow, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置播放位置线绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("播放位置线绘制画笔")]
        public Pen PlayLineChannelPen
        {
            get { return _playLineChannelPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _playLineChannelPen.Dispose();
                _playLineChannelPen = value;
            }
        }

        /// <summary>
        /// 是否绘制播放位置线
        /// </summary>
        private bool _isDrawPlayLine = true;

        /// <summary>
        /// 获取或设置是否绘制播放位置线[true:绘制;false:不绘制]
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否绘制播放位置线[true:绘制;false:不绘制]")]
        public bool IsDrawPlayLine
        {
            get { return _isDrawPlayLine; }
            set { _isDrawPlayLine = value; }
        }

        /// <summary>
        /// 时间绘制画笔
        /// </summary>
        private Pen _timePen = new Pen(Color.WhiteSmoke, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置时间绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("时间绘制画笔")]
        public Pen TimePen
        {
            get { return _timePen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _timePen.Dispose();
                _timePen = value;
            }
        }

        /// <summary>
        /// 幅度绘制画笔
        /// </summary>
        private Pen _dbPen = new Pen(Color.WhiteSmoke, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置幅度绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("幅度绘制画笔")]
        public Pen DbPen
        {
            get { return _dbPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _dbPen.Dispose();
                _dbPen = value;
            }
        }

        /// <summary>
        /// 缩略波形图背景画刷
        /// </summary>
        private Brush _zoomBackgroundBrush = new SolidBrush(Color.FromArgb(162, 94, 73));

        /// <summary>
        /// 获取或设置缩略波形图背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("缩略波形图背景画刷")]
        public Brush ZoomBackgroundBrush
        {
            get { return _zoomBackgroundBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _zoomBackgroundBrush.Dispose();
                _zoomBackgroundBrush = value;
            }
        }

        /// <summary>
        /// 是否启用缩略波形背景
        /// </summary>
        private volatile bool _enableZoomWavBackground = false;

        /// <summary>
        /// 获取或设置是否启用缩略波形背景
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否启用缩略波形背景")]
        public bool EnableZoomWavBackground
        {
            get { return _enableZoomWavBackground; }
            set { _enableZoomWavBackground = value; }
        }

        /// <summary>
        /// 缩略波形图选中区域背景画刷
        /// </summary>
        private Brush _zoomSelectedBackgroundBrush = new SolidBrush(Color.FromArgb(197, 124, 240));

        /// <summary>
        /// 获取或设置缩略波形图选中区域背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("缩略波形图选中区域背景画刷")]
        public Brush ZoomSelectedBackgroundBrush
        {
            get { return _zoomSelectedBackgroundBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _zoomSelectedBackgroundBrush.Dispose();
                _zoomSelectedBackgroundBrush = value;
            }
        }

        /// <summary>
        /// 显示波形区域在缩略波形中的对应的区域背景画刷
        /// </summary>
        private Brush _zoomDisplayAreaBrush = new SolidBrush(Color.FromArgb(131, 32, 115));

        /// <summary>
        /// 获取或设置显示波形区域在缩略波形中的对应的区域背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("显示波形区域在缩略波形中的对应的区域背景画刷")]
        public Brush ZoomDisplayAreaBrush
        {
            get { return _zoomDisplayAreaBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _zoomDisplayAreaBrush.Dispose();
                _zoomDisplayAreaBrush = value;
            }
        }

        /// <summary>
        /// 显示波形区域在缩略波形中与选中的区域重叠背景画刷
        /// </summary>
        private Brush _zoomDisplaySelectedOverlapAreaBrush = new SolidBrush(Color.FromArgb(106, 134, 239));

        /// <summary>
        /// 获取或设置显示波形区域在缩略波形中与选中的区域重叠背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("显示波形区域在缩略波形中与选中的区域重叠背景画刷")]
        public Brush ZoomDisplaySelectedOverlapAreaBrush
        {
            get { return _zoomDisplaySelectedOverlapAreaBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _zoomDisplaySelectedOverlapAreaBrush.Dispose();
                _zoomDisplaySelectedOverlapAreaBrush = value;
            }
        }

        /// <summary>
        /// 显示波形区域在缩略波形中的对应的区域画笔
        /// </summary>
        private Pen _zoomDisplayAreaPen = new Pen(Color.Azure);

        /// <summary>
        /// 获取或设置显示波形区域在缩略波形中的对应的区域画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("显示波形区域在缩略波形中的对应的区域画笔")]
        public Pen ZoomDisplayAreaPen
        {
            get { return _zoomDisplayAreaPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _zoomDisplayAreaPen.Dispose();
                _zoomDisplayAreaPen = value;
            }
        }

        /// <summary>
        /// 时间背景画刷
        /// </summary>
        private Brush _timeBackgroundBrush = new SolidBrush(Color.FromArgb(25, 46, 218));

        /// <summary>
        /// 获取或设置时间背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("时间背景画刷")]
        public Brush TimeBackgroundBrush
        {
            get { return _timeBackgroundBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _timeBackgroundBrush.Dispose();
                _timeBackgroundBrush = value;
            }
        }

        /// <summary>
        /// 是否启用时间背景
        /// </summary>
        private volatile bool _enableTimeBackground = true;

        /// <summary>
        /// 获取或设置是否启用时间背景
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否启用时间背景")]
        public bool EnableTimeBackground
        {
            get { return _enableTimeBackground; }
            set { _enableTimeBackground = value; }
        }

        /// <summary>
        ///幅度背景画刷
        /// </summary>
        private Brush _dbBackgroundBrush = new SolidBrush(Color.Gray);

        /// <summary>
        /// 获取或设置幅度背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("幅度背景画刷")]
        public Brush DbBackgroundBrush
        {
            get { return _dbBackgroundBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _dbBackgroundBrush.Dispose();
                _dbBackgroundBrush = value;
            }
        }

        /// <summary>
        /// 是否启用幅度区域背景
        /// </summary>
        private volatile bool _enableDbAreaBackground = true;

        /// <summary>
        /// 获取或设置是否启用幅度区域背景
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否启用幅度区域背景")]
        public bool EnableDbAreaBackground
        {
            get { return _enableDbAreaBackground; }
            set { _enableDbAreaBackground = value; }
        }

        /// <summary>
        /// 缩略左声道波形绘制画笔
        /// </summary>
        private Pen _zoomLeftChannelPen = new Pen(Color.LimeGreen, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置缩略左声道波形绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("缩略左声道波形绘制画笔")]
        public Pen ZoomLeftChannelPen
        {
            get { return _zoomLeftChannelPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _zoomLeftChannelPen.Dispose();
                _zoomLeftChannelPen = value;
            }
        }

        /// <summary>
        /// 缩略右声道波形绘制画笔
        /// </summary>
        private Pen _zoomRightChannelPen = new Pen(Color.DarkSeaGreen, WavePlayer.DEFAULLINEWIDTH);

        /// <summary>
        /// 获取或设置缩略右声道波形绘制画笔
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("缩略右声道波形绘制画笔")]
        public Pen ZoomRightChannelPen
        {
            get { return _zoomRightChannelPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _zoomRightChannelPen.Dispose();
                _zoomRightChannelPen = value;
            }
        }

        /// <summary>
        /// 缩略波形高度
        /// </summary>
        private int _zoomHeight = 60;

        /// <summary>
        /// 获取或设置缩略波形高度
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("缩略波形高度")]
        public int ZoomHeight
        {
            get { return _zoomHeight; }
            set
            {
                _zoomHeight = value;

                //更新UI布局区域
                this.UpdateUIArea();

                //刷新波形图
                this.RefreshWave(true, true, true, true, false, false);
            }
        }

        /// <summary>
        /// 时间区域高度
        /// </summary>
        private int _timeHeight = 25;

        ///// <summary>
        ///// 获取或设置时间区域高度
        ///// </summary>
        //[Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //[Description("时间区域高度")]
        //public int TimeHeight
        //{
        //    get { return _timeHeight; }
        //    set
        //    {
        //        if (value < 10)
        //        {
        //            throw new ArgumentException("时间区域高度不能小于10", "value");
        //        }

        //        _timeHeight = value;

        //        //更新UI布局区域
        //        this.UpdateUIArea();

        //        //添加缩略波形图区域和主波形图区域为需要更新的区域
        //        this.AddPartRefreshArea(this._timeArea);
        //        this.AddPartRefreshArea(this._wavArea);

        //        //刷新波形图
        //        this.RefreshWave(false, true, true,true, false, false);
        //    }
        //}

        /// <summary>
        /// 幅度宽度
        /// </summary>
        private int _dbWidth = 50;

        /// <summary>
        /// 获取或设置幅度宽度
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("幅度宽度")]
        public int DbWidth
        {
            get { return _dbWidth; }
            set
            {
                if (value < 10)
                {
                    throw new ArgumentException("幅度区域宽度不能小于10", "value");
                }

                _dbWidth = value;

                //更新UI布局区域
                this.UpdateUIArea();

                //刷新波形图
                this.RefreshWave(true, true, true, true, true, true);
            }
        }

        /// <summary>
        ///Logo背景画刷
        /// </summary>
        private Brush _logoBackgroundBrush = new SolidBrush(Color.FromArgb(14, 13, 226));

        /// <summary>
        /// 获取或设置Logo背景画刷
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Logo背景画刷")]
        public Brush LogoBackgroundBrush
        {
            get { return _logoBackgroundBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _logoBackgroundBrush.Dispose();
                _logoBackgroundBrush = value;
            }
        }

        /// <summary>
        /// 是否启用Logo区域背景
        /// </summary>
        private volatile bool _enableLogoAreaBackground = false;

        /// <summary>
        /// 获取或设置是否启用Logo区域背景
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("是否启用Logo区域背景")]
        public bool EnableLogoAreaBackground
        {
            get { return _enableLogoAreaBackground; }
            set { _enableLogoAreaBackground = value; }
        }

        /// <summary>
        /// 缩放倍数
        /// </summary>
        private int _zoomMuilt = 2;

        /// <summary>
        /// 获取或设置缩放倍数,该值越大,则缩放越明显
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("缩放倍数,该值越大,则缩放越明显")]
        public int ZoomMuilt
        {
            get { return _zoomMuilt; }
            set { _zoomMuilt = value; }
        }

        /// <summary>
        /// 绘制刻度文本的Brush
        /// </summary>
        private Brush _fontBrush = new SolidBrush(Color.White);

        /// <summary>
        /// 获取或设置绘制刻度文本的Brush
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("绘制刻度文本的Brush")]
        public Brush FontBrush
        {
            get { return _fontBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _fontBrush.Dispose();
                _fontBrush = value;
            }
        }

        /// <summary>
        /// 用于绘制刻度值的字体
        /// </summary>
        private Font _stringFont = new Font("MS UI Gothic", 8);

        ///// <summary>
        ///// 获取或设置用于绘制刻度值的字体
        ///// </summary>
        //[Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //[Description("用于绘制刻度值的字体")]
        //public Font StringFont
        //{
        //    get { return _stringFont; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            throw new ArgumentNullException("value");
        //        }

        //        _stringFont.Dispose();
        //        _stringFont = value;
        //    }
        //}

        /// <summary>
        /// 缩略波形显示区域移动鼠标样式
        /// </summary>
        private Cursor _zoomWavDisplayAreaMmoveMouseStyle = Cursors.SizeAll;

        /// <summary>
        /// 获取或设置缩略波形显示区域移动鼠标样式
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("缩略波形显示区域移动鼠标样式")]
        public Cursor ZoomWavDisplayAreaMmoveMouseStyle
        {
            get { return _zoomWavDisplayAreaMmoveMouseStyle; }
            set { _zoomWavDisplayAreaMmoveMouseStyle = value; }
        }

        /// <summary>
        /// 波形可选中时鼠标样式
        /// </summary>
        private Cursor _wavSelecteMouseStyle = Cursors.IBeam;

        /// <summary>
        /// 获取或设置波形可选中时鼠标样式
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("波形可选中时鼠标样式")]
        public Cursor WavSelecteMouseStyle
        {
            get { return _wavSelecteMouseStyle; }
            set { _wavSelecteMouseStyle = value; }
        }
    }
}
