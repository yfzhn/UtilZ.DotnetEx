using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using mshtml;
using System.IO;
using System.Collections;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.LMQ;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 日志显示控件
    /// </summary>
    public partial class LogControlWeb : UserControl
    {
        /// <summary>
        /// 最多显示项数
        /// </summary>
        private int _maxItemCount = 100;
        /// <summary>
        /// 获取或设置最多显示项数
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("获取或设置最多显示项数,默认为100项,超过之后则会将旧的项移除")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxItemCount
        {
            get { return _maxItemCount; }
            set
            {
                if (_maxItemCount == value)
                {
                    return;
                }

                if (value < 1)
                {
                    _maxItemCount = 1;
                }
                else
                {
                    _maxItemCount = value;
                }

                this.RemoveOutElements();
            }
        }

        private bool _isLock = false;
        /// <summary>
        /// 是否锁定滚动
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("是否锁定滚动[true:锁定;false:不锁定;默认为false]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsLock
        {
            get { return _isLock; }
            set
            {
                if (_isLock == value)
                {
                    return;
                }

                _isLock = value;
                if (_isLock)
                {
                    this._logShowQueue.Stop();
                }
                else
                {
                    this._logShowQueue.Start();
                }
            }
        }
        /*
        /// <summary>
        /// 获取选中文本
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("是否锁定滚动[true:锁定;false:不锁定;默认为false]")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectText
        {
            get
            {
                IHTMLDocument2 doc2 = webBrowser.Document.DomDocument as IHTMLDocument2;
                dynamic range = doc2.selection.createRange();
                return range.htmlText;
            }
        }*/

        private HtmlElement _logContainerEle;
        private string _logContainerEleId;
        private string _logItemEleName;
        /// <summary>
        /// 模板类型[true:内置模板;false:外部文件模板]
        /// </summary>
        private bool _templateType;
        private string _urlPath;

        private AsynQueue<ShowLogItem> _logShowQueue = null;
        private readonly object _logShowQueueLock = new object();

        /// <summary>
        /// key:ClassId,value:style
        /// </summary>
        private readonly Hashtable _htStyle = new Hashtable();

        ///// <summary>
        ///// 获取控件的同步上下文
        ///// </summary>
        //private System.Threading.SynchronizationContext _synContext;

        /// <summary>
        /// 单次最大刷新日志条数
        /// </summary>
        private int _refreshCount = 0;

        /// <summary>
        /// 日志缓存容量
        /// </summary>
        private int _cacheCapcity = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogControlWeb()
        {
            InitializeComponent();

            //this._synContext = System.Threading.SynchronizationContext.Current;            
            this.webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            this.webBrowser.ScriptErrorsSuppressed = true;
            //this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.Init("uid", "li");
            this._templateType = true;
            this.webBrowser.DocumentText = this.GetLogHtmlTemplate();
        }

        /// <summary>
        /// 启动刷新日志线程
        /// </summary>
        /// <param name="refreshCount">单次最大刷新日志条数</param>
        /// <param name="cacheCapcity">日志缓存容量,建议等于日志最大项数</param>
        public void StartRefreshLogThread(int refreshCount, int cacheCapcity)
        {
            if (refreshCount < 1)
            {
                throw new ArgumentException(string.Format("单次最大刷新日志条数参数值:{0}无效,该值不能小于1", refreshCount), "refreshCount");
            }

            if (cacheCapcity < refreshCount)
            {
                throw new ArgumentException(string.Format("日志缓存容量参数值:{0}无效,该值不能小于单次最大刷新日志条数参数值:{1}", cacheCapcity, refreshCount), "cacheCapcity");
            }

            lock (this._logShowQueueLock)
            {
                if (this._refreshCount == refreshCount && this._cacheCapcity == cacheCapcity)
                {
                    //参数相同,忽略
                    return;
                }

                if (this._logShowQueue != null)
                {
                    this._logShowQueue.Stop();
                    this._logShowQueue.Dispose();
                }

                this._logShowQueue = new AsynQueue<ShowLogItem>(this.ShowLog, refreshCount, 10, "日志显示线程", true, true, cacheCapcity);
            }
        }

        private void ShowLog(List<ShowLogItem> items)
        {
            try
            {
                //this._synContext.Post(new System.Threading.SendOrPostCallback(this.ShowLog), item);
                this.Invoke(new Action(() => { this.ShowLogInfo(items); }));
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void ShowLogInfo(List<ShowLogItem> items)
        {
            try
            {
                if (this._logContainerEle == null)
                {
                    return;
                }

                foreach (ShowLogItem item in items)
                {
                    HtmlElement logEle = this.CreateLogItemEle(item);
                    this._logContainerEle.AppendChild(logEle);

                    if (!this._isLock)
                    {
                        this.RemoveOutElements();
                    }

                    if (item.Color == Color.Green)
                    {
                        LMQCenter.Publish("123", null);
                    }
                }

                this.webBrowser.Document.Window.ScrollTo(0, this.webBrowser.Document.Window.Size.Height);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private HtmlElement CreateLogItemEle(ShowLogItem item)
        {
            //this.webBrowser.Document.InvokeScript("")
            HtmlElement logEle = this.webBrowser.Document.CreateElement(this._logItemEleName);
            logEle.SetAttribute("id", Guid.NewGuid().ToString());
            logEle.InnerText = item.LogText;
            switch (item.Type)
            {
                case StyleType.Color:
                    logEle.Style = string.Format("color:{0}", ColorTranslator.ToHtml(item.Color));
                    break;
                case StyleType.Style:
                    logEle.Style = item.Css;
                    break;
                case StyleType.ClassId:
                    logEle.Style = this.GetStyle(item.Css);
                    break;
                case StyleType.None:
                default:
                    break;
            }

            return logEle;
        }

        #region 生成Style
        private string GetStyle(string classId)
        {
            string styleText;
            if (this._htStyle.ContainsKey(classId))
            {
                styleText = this._htStyle[classId] as string;
            }
            else
            {
                string styleStr = this.GetTyleStr();
                if (string.IsNullOrWhiteSpace(styleStr))
                {
                    styleText = null;
                }
                else
                {
                    styleText = this.GetCssText(styleStr, classId);
                }

                this._htStyle.Add(classId, styleText);
            }

            return styleText;
        }

        private string GetCssText(string styleStr, string classId)
        {
            string classStr = "." + classId;
            int index = styleStr.IndexOf(classStr);
            if (index < 0)
            {
                return null;
            }
            else
            {
                index = index + classStr.Length;
            }

            int endIndex = -1, leftCount = 0, rightCount = 0;
            char ch;
            for (int i = index; i < styleStr.Length; i++)
            {
                ch = styleStr[i];
                if (ch == '{')
                {
                    leftCount++;
                    if (leftCount == 1)
                    {
                        index = i + 1;
                    }
                }
                else if (ch == '}')
                {
                    rightCount++;
                    if (rightCount == leftCount)
                    {
                        endIndex = i;
                        break;
                    }
                }
                else
                {

                }
            }

            if (endIndex == -1)
            {
                return null;
            }

            return styleStr.Substring(index, endIndex - index).Trim();
        }

        private string GetTyleStr()
        {
            HtmlElement headEle = null;
            foreach (HtmlElement ele in this.webBrowser.Document.All)
            {
                if (string.Equals(ele.TagName, "HEAD", StringComparison.OrdinalIgnoreCase))
                {
                    headEle = ele;
                    break;
                }
            }

            if (headEle == null)
            {
                return null;
            }

            HtmlElement styleEle = null;
            foreach (HtmlElement ele in headEle.Children)
            {
                if (string.Equals(ele.TagName, "STYLE", StringComparison.OrdinalIgnoreCase))
                {
                    styleEle = ele;
                    break;
                }
            }

            if (styleEle == null)
            {
                return null;
            }

            return styleEle.InnerHtml;
        }
        #endregion

        /// <summary>
        /// 移除超出限制的日志项
        /// </summary>
        private void RemoveOutElements()
        {
            if (this._logContainerEle == null)
            {
                return;
            }

            var logItemElementCollection = this._logContainerEle.Children;
            int offset = logItemElementCollection.Count - this._maxItemCount;
            if (offset <= 0)
            {
                return;
            }

            //IHTMLDOMNode node;
            //for (int i = 0; i < offset; i++)
            //{
            //    node = logItemElementCollection[0].DomElement as IHTMLDOMNode;
            //    node.parentNode.removeChild(node);
            //}

            string id;
            for (int i = 0; i < offset; i++)
            {
                HtmlElement ele = logItemElementCollection[0];
                id = ele.GetAttribute("id");
                this.webBrowser.Document.InvokeScript("removeEle", new object[] { id });
            }
        }

        private string GetLogHtmlTemplate()
        {
            string logHtmlTemplate = @"<!DOCTYPE html>
            <html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
            <head>
                <meta charset=""utf-8"" />
                <title></title>
                <style type=""text/css"">
        body {
            background-color: black
        }
        ul {
            margin: 0px 0px 0px 0px;
        }
                    li {
                        color: gray;
                        list-style: none;
                    }
                </style>

<script type=""text/javascript"">
//document.oncontextmenu = function(){return false;};  
        document.oncontextmenu = function () {
            var selecTextLen;
            var selectionObj = window.getSelection();
            if (selectionObj == null) {
                selecTextLen = 0;
            }
            else {
                var selectedText = selectionObj.toString();
                selecTextLen = selectedText.length;
            }

            return selecTextLen > 0; 
        };

        function removeEle(id)
            {
                var ele = document.getElementById(id);
                if (ele != null){
                    try {
                     ele.parentElement.removeChild(ele);
                    }
                    catch (err) {
                        //window.alert(err.message);
                    }
                }
                else{
                //window.alert(""ele == null"");
                }
            };
    </script>

            </head>
            <body>
            <ul  id=""uid"">
            </ul>
            </body>
            </html>";
            return logHtmlTemplate;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var doc = this.webBrowser.Document;
            if (doc == null)
            {
                this._logContainerEle = null;
            }
            else
            {
                this._logContainerEle = doc.GetElementById(this._logContainerEleId);
            }
        }

        private void Init(string logContainerEleId, string logItemEleName)
        {
            this._logContainerEleId = logContainerEleId;
            this._logItemEleName = logItemEleName;
        }

        /// <summary>
        /// 设置日志显示模板
        /// </summary>
        /// <param name="templateFilePath">日志模板html文件路径</param>
        /// <param name="logContainerEleId">显示日志的容器元素id</param>
        /// <param name="logItemEleName">日志项内容名称</param>
        public void SetTemplate(string templateFilePath, string logContainerEleId, string logItemEleName)
        {
            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException("日志模板文件不存在", templateFilePath);
            }

            this.Init(logContainerEleId, logItemEleName);
            //file://F:/Project/Git/UtilZ/LogControl.html
            this._urlPath = Uri.UriSchemeFile + Uri.SchemeDelimiter + Path.GetFullPath(templateFilePath).Replace(Path.DirectorySeparatorChar, '/');
            this._templateType = false;
            this._htStyle.Clear();
            this.webBrowser.Navigate(this._urlPath);
        }

        private readonly object _innerAddLogLock = new object();
        private void InnerAddLog(ShowLogItem item)
        {
            bool result;
            var logShowQueue = this._logShowQueue;
            if (logShowQueue == null)
            {
                return;
            }

            lock (this._innerAddLogLock)
            {
                while (true)
                {
                    result = logShowQueue.Enqueue(item);
                    if (result)
                    {
                        break;
                    }
                    else
                    {
                        logShowQueue.Remove(1);
                    }
                }
            }
        }

        /// <summary>
        /// 添加显示日志
        /// </summary>
        /// <param name="logText">日志文本</param>
        public void AddLog(string logText)
        {
            this.InnerAddLog(new ShowLogItem(logText));
        }

        /// <summary>
        /// 添加显示日志,样式指定为颜色
        /// </summary>
        /// <param name="logText">日志文本</param>
        /// <param name="color">该条记录文本所显示的颜色</param>
        public void AddLogStyleForColor(string logText, Color color)
        {
            this.InnerAddLog(new ShowLogItem(logText, color));
        }

        /// <summary>
        /// 添加显示日志,样式指定为style
        /// </summary>
        /// <param name="logText">日志文本</param>
        /// <param name="style">样式</param>
        public void AddLogStyleForCss(string logText, string style)
        {
            this.InnerAddLog(new ShowLogItem(logText, style, StyleType.Style));
        }

        /// <summary>
        /// 添加显示日志,样式指定为css中定义的类名
        /// </summary>
        /// <param name="logText">日志文本</param>
        /// <param name="className">css class名称</param>
        public void AddLogStyleForClass(string logText, string className)
        {
            this.InnerAddLog(new ShowLogItem(logText, className, StyleType.ClassId));
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        public void Clear()
        {
            if (this._logContainerEle == null)
            {
                return;
            }

            if (this._templateType)
            {
                this.webBrowser.DocumentText = this.GetLogHtmlTemplate();
            }
            else
            {
                this.webBrowser.Navigate(this._urlPath);
            }
        }

        internal class ShowLogItem
        {
            private StyleType _type;
            internal StyleType Type { get { return _type; } }

            public Color Color { get; private set; }

            public string Css { get; private set; }

            private string _logText;
            public string LogText { get { return _logText; } }

            public ShowLogItem(string logText)
            {
                this._type = StyleType.None;
                this._logText = logText;
            }

            public ShowLogItem(string logText, Color color)
            {
                this._type = StyleType.Color;
                this._logText = logText;
                this.Color = color;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="logText"></param>
            /// <param name="css"></param>
            /// <param name="styleType">样式类型</param>
            public ShowLogItem(string logText, string css, StyleType styleType)
            {
                this._type = styleType;
                this._logText = logText;
                this.Css = css;
            }
        }

        internal enum StyleType
        {
            None,

            Color,

            Style,

            ClassId
        }
    }
}
