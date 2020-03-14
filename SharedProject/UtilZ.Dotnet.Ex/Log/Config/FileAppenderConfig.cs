using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志追加器配置
    /// </summary>
    public class FileAppenderConfig : BaseConfig
    {
        /// <summary>
        /// 日志保留天数小于1表示永不清除
        /// </summary>
        public int Days { get; set; } = 7;

        /// <summary>
        /// 最多产生的日志文件数，超过则只保留最新的n个,小于1为不限文件数
        /// </summary>
        public int MaxFileCount { get; set; } = -1;

        private int _maxFileSize = 0;
        /// <summary>
        /// 日志文件上限大小,当文件超过此值则分隔成多个日志文件,小于1不限制,单位/KB
        /// </summary>
        public int MaxFileSize
        {
            get { return _maxFileSize; }
            set
            {
                _maxFileSize = value;
                _maxFileLength = _maxFileSize * 1024;
            }
        }

        private long _maxFileLength = 0;
        /// <summary>
        /// 获取日志单个文件最大大小,小于等于0不限制,单位/字节
        /// </summary>
        internal long MaxFileLength
        {
            get { return _maxFileLength; }
        }

        /// <summary>
        /// 日志存放路径
        /// </summary>
        public string FilePath { get; set; } = @"Log/*yyyy-MM-dd_HH_mm_ss.fffffff*.log";

        /// <summary>
        /// 是否追加日志
        /// </summary>
        public bool IsAppend { get; set; } = true;

        /// <summary>
        /// 日志安全策略,该类型为实现接口ILogSecurityPolicy的子类,必须实现Encryption方法
        /// </summary>
        public string SecurityPolicy { get; set; } = null;

        /// <summary>
        /// 锁类模型[Exclusive,InterProcess,Minimal]
        /// </summary>
        public LockingModel LockingModel { get; set; } = LockingModel.Exclusive;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素</param>
        public FileAppenderConfig(XElement ele)
            : base(ele)
        {
            //base.EnableOutputCache = true;
            this.MaxFileSize = 10 * 1024;//默认值10MB

            if (ele == null)
            {
                return;
            }

            int days;
            if (int.TryParse(LogUtil.GetChildXElementValue(ele, nameof(this.Days)), out days))
            {
                this.Days = days;
            }

            int maxFileCount;
            if (int.TryParse(LogUtil.GetChildXElementValue(ele, nameof(this.MaxFileCount)), out maxFileCount))
            {
                if (maxFileCount < 1 && maxFileCount != -1)
                {
                    maxFileCount = -1;
                }

                this.MaxFileCount = maxFileCount;
            }

            int maxFileSize;
            if (int.TryParse(LogUtil.GetChildXElementValue(ele, nameof(this.MaxFileSize)), out maxFileSize))
            {
                this.MaxFileSize = maxFileSize;
            }

            string filePath = LogUtil.GetChildXElementValue(ele, nameof(this.FilePath)).Trim();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                this.FilePath = filePath.Trim();
            }

            bool isAppend;
            if (bool.TryParse(LogUtil.GetChildXElementValue(ele, nameof(this.IsAppend)).Trim(), out isAppend))
            {
                this.IsAppend = isAppend;
            }

            this.SecurityPolicy = LogUtil.GetChildXElementValue(ele, nameof(this.SecurityPolicy)).Trim();

            LockingModel lockingType;
            if (Enum.TryParse<LockingModel>(LogUtil.GetChildXElementValue(ele, nameof(this.LockingModel)).Trim(), out lockingType))
            {
                this.LockingModel = lockingType;
            }
        }
    }

    /// <summary>
    /// 锁类模型
    /// </summary>
    public enum LockingModel
    {
        /// <summary>
        /// 独占
        /// </summary>
        Exclusive,

        /// <summary>
        /// 相互写
        /// </summary>
        InterProcess,

        /// <summary>
        /// 最小,用完就关
        /// </summary>
        Minimal
    }
}
