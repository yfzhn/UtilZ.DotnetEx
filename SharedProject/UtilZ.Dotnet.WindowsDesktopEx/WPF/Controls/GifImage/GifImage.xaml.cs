using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// GifImage.xaml 的交互逻辑
    /// </summary>
    public partial class GifImage : UserControl
    {
        /// <summary>
        /// gif图片动画
        /// </summary>
        private GifAnimation _gifAnimation = null;

        /// <summary>
        /// 当前显示图片的控件
        /// </summary>
        private Image _image = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GifImage()
        {
            //InitializeComponent();
        }

        #region 依赖属性
        /// <summary>
        /// 是否强制显示依赖属性
        /// </summary>
        public static readonly DependencyProperty ForceGifAnimProperty = DependencyProperty.Register("ForceGifAnim", typeof(bool), typeof(GifImage), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// 获取或设置是否强制显示
        /// </summary>
        public bool ForceGifAnim
        {
            get
            {
                return (bool)this.GetValue(ForceGifAnimProperty);
            }
            set
            {
                this.SetValue(ForceGifAnimProperty, value);
            }
        }

        /// <summary>
        /// gif源依赖属性
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(GifImage), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// gif源依赖属性改变事件
        /// </summary>
        /// <param name="d">依赖属性对象</param>
        /// <param name="e">参数</param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            string gifPath = (string)e.NewValue;
            obj.CreateFromSourceString(gifPath);
        }

        /// <summary>
        /// 获取或设置gif源
        /// </summary>
        public string Source
        {
            get
            {
                return (string)this.GetValue(SourceProperty);
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// 描述的Stretch值GifImage如何加载目标矩形依赖属性
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(GifImage), new FrameworkPropertyMetadata(Stretch.Fill, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchChanged)));

        /// <summary>
        /// 描述的Stretch值GifImage如何加载目标矩形依赖属性改变事件
        /// </summary>
        /// <param name="d">依赖属性对象</param>
        /// <param name="e">参数</param>
        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            Stretch s = (Stretch)e.NewValue;
            if (obj._gifAnimation != null)
            {
                obj._gifAnimation.Stretch = s;
            }
            else if (obj._image != null)
            {
                obj._image.Stretch = s;
            }
        }

        /// <summary>
        /// 获取或设置描述的Stretch值GifImage如何加载目标矩形
        /// </summary>
        public Stretch Stretch
        {
            get
            {
                return (Stretch)this.GetValue(StretchProperty);
            }
            set
            {
                this.SetValue(StretchProperty, value);
            }
        }

        /// <summary>
        /// 确定对缩放的限制也适用于图像的值依赖属性
        /// </summary>
        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(GifImage), new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchDirectionChanged)));

        /// <summary>
        /// 确定对缩放的限制也适用于图像的值依赖属性改变事件
        /// </summary>
        /// <param name="d">依赖属性对象</param>
        /// <param name="e">参数</param>
        private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            StretchDirection stretchDirection = (StretchDirection)e.NewValue;
            if (obj._gifAnimation != null)
            {
                obj._gifAnimation.StretchDirection = stretchDirection;
            }
            else if (obj._image != null)
            {
                obj._image.StretchDirection = stretchDirection;
            }
        }

        /// <summary>
        /// 获取或设置确定对缩放的限制也适用于图像的值
        /// </summary>
        public StretchDirection StretchDirection
        {
            get
            {
                return (StretchDirection)this.GetValue(StretchDirectionProperty);
            }
            set
            {
                this.SetValue(StretchDirectionProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// 图片加载失败路由依赖事件
        /// </summary>
        public static readonly RoutedEvent ImageFailedEvent = EventManager.RegisterRoutedEvent("ImageFailed", RoutingStrategy.Bubble, typeof(EventHandler<GifImageExceptionRoutedEventArgs>), typeof(GifImage));

        /// <summary>
        /// 图片加载失败路由依赖事件属性
        /// </summary>
        public event EventHandler<GifImageExceptionRoutedEventArgs> ImageFailed
        {
            add
            {
                AddHandler(ImageFailedEvent, value);
            }
            remove
            {
                RemoveHandler(ImageFailedEvent, value);
            }
        }

        /// <summary>
        /// 图片加载失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.RaiseImageFailedEvent(e.ErrorException);
        }

        /// <summary>
        /// 图片加载失败路由依赖事件方法
        /// </summary>
        /// <param name="exp">异常信息</param>
        private void RaiseImageFailedEvent(Exception exp)
        {
            GifImageExceptionRoutedEventArgs newArgs = new GifImageExceptionRoutedEventArgs(ImageFailedEvent, this, exp);
            this.RaiseEvent(newArgs);
        }

        /// <summary>
        /// 删除之前的图片
        /// </summary>
        private void DeletePreviousImage()
        {
            if (this._image != null)
            {
                this.RemoveLogicalChild(this._image);
                this._image = null;
            }
            if (this._gifAnimation != null)
            {
                this.RemoveLogicalChild(this._gifAnimation);
                this._gifAnimation = null;
            }
        }

        /// <summary>
        /// 创建非gif的动画图片
        /// </summary>
        private void CreateNonGifAnimationImage()
        {
            this._image = new Image();
            this._image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(this.image_ImageFailed);
            ImageSource src = (ImageSource)(new ImageSourceConverter().ConvertFromString(this.Source));
            this._image.Source = src;
            this._image.Stretch = Stretch;
            this._image.StretchDirection = StretchDirection;
            this.AddChild(this._image);
        }

        /// <summary>
        /// 创建gif动画
        /// </summary>
        /// <param name="memoryStream">内存数据流</param>
        private void CreateGifAnimation(MemoryStream memoryStream)
        {
            this._gifAnimation = new GifAnimation();
            this._gifAnimation.CreateGifAnimation(memoryStream);
            this._gifAnimation.Stretch = this.Stretch;
            this._gifAnimation.StretchDirection = this.StretchDirection;
            this.AddChild(this._gifAnimation);
        }

        /// <summary>
        /// 根据源创建获取gif图片的地址
        /// </summary>
        /// <param name="source">源</param>
        private void CreateFromSourceString(string source)
        {
            this.DeletePreviousImage();
            Uri uri = null;

            try
            {
                uri = new Uri(source, UriKind.RelativeOrAbsolute);
            }
            catch (Exception exp)
            {
                this.RaiseImageFailedEvent(exp);
                return;
            }

            if (source.Trim().ToUpper().EndsWith(".GIF") || this.ForceGifAnim)
            {
                if (uri.IsAbsoluteUri)
                {

                    string leftPart = uri.GetLeftPart(UriPartial.Scheme);

                    if (leftPart == "http://" || leftPart == "ftp://" || leftPart == "file://")
                    {
                        this.GetGifStreamFromHttp(uri);
                    }
                    else if (leftPart == "pack://")
                    {
                        this.GetGifStreamFromPack(uri);
                    }
                    else
                    {
                        this.CreateNonGifAnimationImage();
                    }
                }
                else
                {
                    this.GetGifStreamFromPack(uri);
                }
            }
            else
            {
                this.CreateNonGifAnimationImage();
            }
        }

        /// <summary>
        /// web请求完成委托
        /// </summary>
        /// <param name="memoryStream"></param>
        private delegate void WebRequestFinishedDelegate(MemoryStream memoryStream);

        /// <summary>
        /// wbe请求完成
        /// </summary>
        /// <param name="memoryStream">请求到的内存流</param>
        private void WebRequestFinished(MemoryStream memoryStream)
        {
            this.CreateGifAnimation(memoryStream);
        }

        /// <summary>
        /// web请求错误委托
        /// </summary>
        /// <param name="exp"></param>
        private delegate void WebRequestErrorDelegate(Exception exp);

        /// <summary>
        /// web请求错误
        /// </summary>
        /// <param name="exp">错误异常</param>
        private void WebRequestError(Exception exp)
        {
            this.RaiseImageFailedEvent(exp);
        }

        /// <summary>
        /// web数据响应回调
        /// </summary>
        /// <param name="asyncResult">异步回调结果</param>
        private void WebResponseCallback(IAsyncResult asyncResult)
        {
            WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
            WebResponse webResponse;
            try
            {
                webResponse = webReadState.WebRequest.EndGetResponse(asyncResult);
                webReadState.ReadStream = webResponse.GetResponseStream();
                webReadState.Buffer = new byte[100000];
                webReadState.ReadStream.BeginRead(webReadState.Buffer, 0, webReadState.Buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
            }
            catch (WebException exp)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
            }
        }

        /// <summary>
        /// web数据读取
        /// </summary>
        /// <param name="asyncResult">异步数据结构</param>
        private void WebReadCallback(IAsyncResult asyncResult)
        {
            WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
            int count = webReadState.ReadStream.EndRead(asyncResult);
            if (count > 0)
            {
                webReadState.MemoryStream.Write(webReadState.Buffer, 0, count);
                try
                {
                    webReadState.ReadStream.BeginRead(webReadState.Buffer, 0, webReadState.Buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
                }
                catch (WebException exp)
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
                }
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestFinishedDelegate(WebRequestFinished), webReadState.MemoryStream);
            }
        }

        /// <summary>
        /// 根据http url地址获取gif
        /// </summary>
        /// <param name="uri">http url地址</param>
        private void GetGifStreamFromHttp(Uri uri)
        {
            try
            {
                WebReadState webReadState = new WebReadState();
                webReadState.MemoryStream = new MemoryStream();
                webReadState.WebRequest = WebRequest.Create(uri);
                webReadState.WebRequest.Timeout = 10000;

                webReadState.WebRequest.BeginGetResponse(new AsyncCallback(WebResponseCallback), webReadState);
            }
            catch (SecurityException)
            {
                CreateNonGifAnimationImage();
            }
        }

        /// <summary>
        /// 异步读取gif流
        /// </summary>
        /// <param name="stream">gif流</param>
        private void ReadGifStreamSynch(Stream stream)
        {
            byte[] gifData;
            MemoryStream memoryStream;
            using (stream)
            {
                memoryStream = new MemoryStream((int)stream.Length);
                BinaryReader br = new BinaryReader(stream);
                gifData = br.ReadBytes((int)stream.Length);
                memoryStream.Write(gifData, 0, (int)stream.Length);
                memoryStream.Flush();
            }

            this.CreateGifAnimation(memoryStream);
        }

        /// <summary>
        /// 根据url地址获取gif流
        /// </summary>
        /// <param name="uri">url地址</param>
        private void GetGifStreamFromPack(Uri uri)
        {
            try
            {
                StreamResourceInfo streamInfo;
                if (uri.IsAbsoluteUri)
                {
                    if (uri.GetLeftPart(UriPartial.Authority).Contains("siteoforigin"))
                    {
                        streamInfo = Application.GetRemoteStream(uri);
                    }
                    else
                    {
                        streamInfo = Application.GetContentStream(uri);
                        if (streamInfo == null)
                        {
                            streamInfo = Application.GetResourceStream(uri);
                        }
                    }
                }
                else
                {
                    streamInfo = Application.GetContentStream(uri);
                    if (streamInfo == null)
                    {
                        streamInfo = Application.GetResourceStream(uri);
                    }
                }

                if (streamInfo == null)
                {
                    throw new FileNotFoundException("Resource not found.", uri.ToString());
                }

                this.ReadGifStreamSynch(streamInfo.Stream);
            }
            catch (Exception exp)
            {
                this.RaiseImageFailedEvent(exp);
            }
        }
    }
}
