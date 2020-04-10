using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// UCZoomTranslateContainerControl.xaml 的交互逻辑
    /// </summary>
    public partial class ZoomTranslateContainer : UserControl
    {
        private const double _PRE = 0.0001d;

        #region 依赖属性
        /// <summary>
        /// 烟雾值依赖属性
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register(nameof(Child), typeof(FrameworkElement), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 缩放最小值依赖属性
        /// </summary>
        public static readonly DependencyProperty ZoomMinScaleProperty =
            DependencyProperty.Register(nameof(ZoomMinScale), typeof(double), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(0.1d, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 缩放最大值依赖属性
        /// </summary>
        public static readonly DependencyProperty ZoomMaxScaleProperty =
            DependencyProperty.Register(nameof(ZoomMaxScale), typeof(double), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(10.0d, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 缩放值依赖属性
        /// </summary>
        public static readonly DependencyProperty ZoomScaleProperty =
            DependencyProperty.Register(nameof(ZoomScale), typeof(double), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(1.0d, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 缩放至完整显示依赖属性
        /// </summary>
        public static readonly DependencyProperty ZoomToFullProperty =
            DependencyProperty.Register(nameof(ZoomToFull), typeof(bool), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 缩放至完整显示模式依赖属性
        /// </summary>
        public static readonly DependencyProperty ZoomToFullPolicyProperty =
            DependencyProperty.Register(nameof(ZoomToFullPolicy), typeof(ZoomToFullPolicy), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(ZoomToFullPolicy.Both, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 缩放启用依赖属性
        /// </summary>
        public static readonly DependencyProperty ZoomEnableProperty =
            DependencyProperty.Register(nameof(ZoomEnable), typeof(bool), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 平移启用依赖属性
        /// </summary>
        public static readonly DependencyProperty TranslateEnableProperty =
            DependencyProperty.Register(nameof(TranslateEnable), typeof(bool), typeof(ZoomTranslateContainer),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChangedCallback)));


        /// <summary>
        /// 获取或设置进行缩放平移的控件
        /// </summary>
        public FrameworkElement Child
        {
            get
            {
                return (FrameworkElement)base.GetValue(ChildProperty);
            }
            set
            {
                base.SetValue(ChildProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置缩放最小值
        /// </summary>
        public double ZoomMinScale
        {
            get
            {
                return (double)base.GetValue(ZoomMinScaleProperty);
            }
            set
            {
                if (value - ZoomMaxScale > _PRE)
                {
                    throw new ArgumentException($"最小值{value}不能大于最大值");
                }

                base.SetValue(ZoomMinScaleProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置缩放最大值
        /// </summary>
        public double ZoomMaxScale
        {
            get
            {
                return (double)base.GetValue(ZoomMaxScaleProperty);
            }
            set
            {
                if (value - ZoomMinScale < _PRE)
                {
                    throw new ArgumentException($"最大值{value}不能小于最小值");
                }

                base.SetValue(ZoomMaxScaleProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置缩放值
        /// </summary>
        public double ZoomScale
        {
            get
            {
                return (double)base.GetValue(ZoomScaleProperty);
            }
            set
            {
                if (value - ZoomMinScale < _PRE)
                {
                    throw new ArgumentException($"缩放比例值{value}不能小于最小值");
                }

                if (value - ZoomMaxScale > _PRE)
                {
                    throw new ArgumentException($"缩放比例值{value}不能大于最小值");
                }

                base.SetValue(ZoomScaleProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置是否自动缩放至完整显示
        /// </summary>
        public bool ZoomToFull
        {
            get
            {
                return (bool)base.GetValue(ZoomToFullProperty);
            }
            set
            {
                base.SetValue(ZoomToFullProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置缩放至完整显示模式
        /// </summary>
        public ZoomToFullPolicy ZoomToFullPolicy
        {
            get
            {
                return (ZoomToFullPolicy)base.GetValue(ZoomToFullPolicyProperty);
            }
            set
            {
                base.SetValue(ZoomToFullPolicyProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置缩放启用
        /// </summary>
        public bool ZoomEnable
        {
            get
            {
                return (bool)base.GetValue(ZoomEnableProperty);
            }
            set
            {
                base.SetValue(ZoomEnableProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置平移启用
        /// </summary>
        public bool TranslateEnable
        {
            get
            {
                return (bool)base.GetValue(TranslateEnableProperty);
            }
            set
            {
                base.SetValue(TranslateEnableProperty, value);
            }
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (ZoomTranslateContainer)d;
            if (e.Property == ChildProperty)
            {
                selfControl.UpdateChild((FrameworkElement)e.NewValue, (FrameworkElement)e.OldValue);
            }
            else if (e.Property == ZoomMinScaleProperty)
            {
                selfControl.UpdateZoomMinScale((double)e.NewValue);
            }
            else if (e.Property == ZoomMaxScaleProperty)
            {
                selfControl.UpdateZoomMaxScale((double)e.NewValue);
            }
            else if (e.Property == ZoomScaleProperty)
            {
                selfControl.UpdateScale((double)e.NewValue);
            }
            else if (e.Property == ZoomToFullProperty || e.Property == ZoomToFullPolicyProperty)
            {
                selfControl.AutoZoomToFull();
            }
        }
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public ZoomTranslateContainer()
        {
            InitializeComponent();
        }



        private void UpdateChild(FrameworkElement newElement, FrameworkElement oldElement)
        {
            if (oldElement != null)
            {
                oldElement.MouseWheel -= Child_MouseWheel;
                oldElement.MouseLeftButtonDown -= Child_MouseLeftButtonDown;
                oldElement.MouseMove -= Child_MouseMove;
                oldElement.SizeChanged -= Child_SizeChanged;
                canvasRoot.Children.Clear();
            }

            if (newElement == null)
            {
                return;
            }

            newElement.Focusable = true;
            newElement.MouseWheel += Child_MouseWheel;
            newElement.MouseLeftButtonDown += Child_MouseLeftButtonDown;
            newElement.MouseMove += Child_MouseMove;
            newElement.SizeChanged += Child_SizeChanged;

            var transformGroup = new TransformGroup();
            this._scaleTransform = new ScaleTransform();
            transformGroup.Children.Add(this._scaleTransform);
            //this._translateTransform = new TranslateTransform();
            //transformGroup.Children.Add(this._translateTransform);
            newElement.RenderTransform = transformGroup;

            canvasRoot.Children.Add(newElement);
            Canvas.SetLeft(newElement, 0);
            Canvas.SetTop(newElement, 0);
            this._child = newElement;

            this.UpdateScale(this.ZoomScale);
        }

        #region 平移
        private FrameworkElement _child;
        private ScaleTransform _scaleTransform = null;
        //private TranslateTransform _translateTransform = null;

        private void Child_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateCanvasRoot(this.ZoomScale);

            this.AutoZoomToFull();
        }

        //平移
        private Point _beginPoint;
        private double _beginLeft;
        private double _beginTop;
        private void Child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.TranslateEnable)
            {
                return;
            }

            this._beginPoint = Mouse.GetPosition(canvasRoot);
            this._beginLeft = Canvas.GetLeft(_child);
            this._beginTop = Canvas.GetTop(_child);
        }

        private void Child_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.TranslateEnable || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            var endPoint = Mouse.GetPosition(canvasRoot);
            double left = this._beginLeft + (endPoint.X - this._beginPoint.X);
            double top = this._beginTop + (endPoint.Y - this._beginPoint.Y);


            const double OFFSET = 100d;
            double width = canvasRoot.ActualWidth;
            if (left + width < OFFSET)
            {
                left = OFFSET - width;
            }
            else if (left > width - OFFSET)
            {
                left = width - OFFSET;
            }

            double height = canvasRoot.ActualHeight;
            if (top + height < OFFSET)
            {
                top = OFFSET - height;
            }
            else if (top > height - OFFSET)
            {
                top = height - OFFSET;
            }


            Canvas.SetLeft(this._child, left);
            Canvas.SetTop(this._child, top);

            this._beginPoint = endPoint;
            this._beginLeft = left;
            this._beginTop = top;
        }

        //private void TranslateTranslateTransform()
        //{
        //    TranslateTransform translateTransform = this._translateTransform;
        //    var endPoint = Mouse.GetPosition(this);

        //    double x = translateTransform.X + (endPoint.X - this._beginPoint.X);
        //    double y = translateTransform.Y + (endPoint.Y - this._beginPoint.Y);

        //    const double PRE = 0.001d;
        //    const double OFFSET = 100d;

        //    double width = canvas.ActualWidth;// * this._scale;
        //    //double width = this.ActualWidth;


        //    if (x + width - OFFSET < PRE)
        //    {
        //        x = OFFSET - width;
        //    }
        //    else if (x - width + OFFSET > PRE)
        //    {
        //        x = width - OFFSET;
        //    }

        //    double height = canvas.ActualHeight;// * this._scale;
        //    //double height = this.ActualHeight;
        //    if (y + height - OFFSET < PRE)
        //    {
        //        y = OFFSET - height;
        //    }
        //    else if (y - height + OFFSET > PRE)
        //    {
        //        y = height - OFFSET;
        //    }

        //    translateTransform.X = x;
        //    translateTransform.Y = y;
        //    this._beginPoint = endPoint;
        //}
        #endregion





        #region 缩放
        private void Child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.ZoomEnable)
            {
                return;
            }

            var value = (double)e.Delta;
            double offset = value / 1000;
            double scale = this.ZoomScale + offset;
            if (scale - this.ZoomMinScale < _PRE ||
                scale - this.ZoomMaxScale > _PRE)
            {
                return;
            }

            this.ZoomScale = scale;
        }

        private void UpdateScale(double scale)
        {
            var scaleTransform = this._scaleTransform;
            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
            this.UpdateCanvasRoot(scale);

        }
        #endregion




        private void UpdateZoomMaxScale(double zoomMaxScale)
        {
            if (this.ZoomScale - zoomMaxScale > _PRE)
            {
                this.ZoomScale = zoomMaxScale;
            }
        }

        private void UpdateZoomMinScale(double zoomMinScale)
        {
            if (this.ZoomScale - zoomMinScale < _PRE)
            {
                this.ZoomScale = zoomMinScale;
            }
        }


        private void UpdateCanvasRoot(double scale)
        {
            canvasRoot.Width = this._child.ActualWidth * scale;
            canvasRoot.Height = this._child.ActualHeight * scale;
        }







        #region 自适应缩放
        /// <summary>
        /// 缩放至完成展示
        /// </summary>
        private void AutoZoomToFull(double step = 0.1d)
        {
            if (!this.ZoomToFull)
            {
                return;
            }

            if (step < _PRE)
            {
                throw new ArgumentOutOfRangeException($"步进值{step}不能小于{_PRE}");
            }

            if (this.HasZoomout())
            {
                //缩小
                this.AdjustToZoomout(step);
            }
            else
            {
                //放大
                this.AdjustToZoomin(step);
            }
        }

        private bool HasZoomout()
        {
            bool hasZoomout = false;
            switch (this.ZoomToFullPolicy)
            {
                case ZoomToFullPolicy.Both:
                    if (canvasRoot.Height - scrollViewer.RenderSize.Height > _PRE ||
                        canvasRoot.Width - scrollViewer.RenderSize.Width > _PRE)
                    {
                        hasZoomout = true;
                    }
                    break;
                case ZoomToFullPolicy.Width:
                    if (canvasRoot.Width - scrollViewer.RenderSize.Width > _PRE)
                    {
                        hasZoomout = true;
                    }
                    break;
                case ZoomToFullPolicy.Height:
                    if (canvasRoot.Height - scrollViewer.RenderSize.Height > _PRE)
                    {
                        hasZoomout = true;
                    }
                    break;
            }

            return hasZoomout;
        }

        /// <summary>
        /// 缩小
        /// </summary>
        private void AdjustToZoomout(double step)
        {
            double zoomScale;

            while (this.HasZoomout())
            {
                zoomScale = this.ZoomScale - step;
                if (zoomScale - this.ZoomMinScale < _PRE)
                {
                    break;
                }

                this.ZoomScale = zoomScale;
            }
        }

        /// <summary>
        /// 放大
        /// </summary>
        private void AdjustToZoomin(double step)
        {
            double zoomScale1 = this.ZoomScale;
            double zoomScale2 = zoomScale1;

            while (!this.HasZoomout())
            {
                zoomScale2 = this.ZoomScale + -step;
                if (zoomScale2 - this.ZoomMinScale > _PRE)
                {
                    break;
                }

                this.ZoomScale = zoomScale2;
            }

            if (zoomScale2 - zoomScale1 > _PRE)
            {
                this.ZoomScale = zoomScale1;
            }
        }
        #endregion
    }

    /// <summary>
    /// 缩放至全屏策略
    /// </summary>
    public enum ZoomToFullPolicy
    {
        /// <summary>
        /// 以宽度为依据,当宽度达到超出可视范围则停止
        /// </summary>
        Width,

        /// <summary>
        /// 以高度为依据,当高度达到超出可视范围则停止
        /// </summary>
        Height,

        /// <summary>
        /// 同时以宽度和高度为依据,当宽度或高度达到超出可视范围则停止
        /// </summary>
        Both
    }
}
