using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志信息类
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="thread">线程</param>
        /// <param name="skipFrames">调用堆栈跳过帧数</param>
        /// <param name="getStackTraceMethodParameterNameType">获取堆栈方法参数名称类型</param>
        /// <param name="logerName">日志记录器名称</param>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public LogItem(DateTime time, Thread thread, int skipFrames, bool getStackTraceMethodParameterNameType, string logerName,
            LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.Time = time;
            this.ThreadID = thread.ManagedThreadId;
            this.ThreadName = thread.Name;
            this.EventID = eventId;
            this.Tag = tag;
            this.Level = level;
            this.StackTrace = new StackTrace(skipFrames, true);
            this.Format = format;
            this.Exception = ex;
            this.LogerName = logerName;
            this._getStackTraceMethodParameterNameType = getStackTraceMethodParameterNameType;
            this.Args = args;
        }

        /// <summary>
        /// 获取堆栈方法参数名称类型[true:代码方式false:系统堆栈方式(eg:List`string),默认为true]
        /// </summary>
        private bool _getStackTraceMethodParameterNameType;

        /// <summary>
        /// 日志项是否已分析过
        /// </summary>
        private bool _isAnalyzed = false;

        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public object ID { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadID { get; private set; }

        /// <summary>
        /// 线程名称
        /// </summary>
        public string ThreadName { get; private set; }

        /// <summary>
        /// 日志记录器名称
        /// </summary>
        public string LogerName { get; private set; }

        /// <summary>
        /// 事件ID
        /// </summary>
        public int EventID { get; private set; }

        /// <summary>
        /// 与对象关联的用户定义数据
        /// </summary>
        public object Tag { get; private set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// 调用堆栈跟踪信息
        /// </summary>
        public StackTrace StackTrace { get; private set; }

        /// <summary>
        /// 复合格式字符串,参数为空或null表示无格式化
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// 一个对象数组，其中包含零个或多个要设置格式的对象
        /// </summary>
        public object[] Args { get; private set; }

        /// <summary>
        /// 完整的日志信息
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 日志产生类名称
        /// </summary>
        public string Logger { get; private set; }

        /// <summary>
        /// 堆栈信息
        /// </summary>
        public string StackTraceInfo { get; private set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; private set; }
        #endregion

        /// <summary>
        /// 方法参数类型与参数名之间的间隔
        /// </summary>
        private const string MethodParameterTypeParameterNameSpacing = " ";

        /// <summary>
        /// 方法参数之间的间隔
        /// </summary>
        private const string MethodParameterSpacing = ", ";

        /// <summary>
        /// 日志处理
        /// </summary>
        public void LogProcess()
        {
            if (this._isAnalyzed)
            {
                return;
            }

            var ex = this.Exception;
            this.GenerateStackTraceInfo(ex);
            this.GenerateContent(ex);
            this._isAnalyzed = true;
        }

        private void GenerateContent(Exception ex)
        {
            StringBuilder sbContent = new StringBuilder();
            string message = this.Format;
            if (!string.IsNullOrEmpty(message))
            {
                if (this.Args != null && this.Args.Length > 0)
                {
                    try
                    {
                        message = string.Format(message, this.Args);
                    }
                    catch (Exception exi)
                    {
                        LogSysInnerLog.OnRaiseLog(this, exi);
                    }
                }

                sbContent.Append(message);
            }
            this.Message = message;

            if (ex != null)
            {
                if (sbContent.Length > 0)
                {
                    sbContent.Append("。");
                }

                Exception innerEx = ex;
                while (innerEx != null)
                {
                    sbContent.Append(string.Format("{0}: {1}", innerEx.GetType().FullName, innerEx.Message));
                    innerEx = innerEx.InnerException;
                    if (innerEx != null)
                    {
                        sbContent.Append(" ---> ");
                    }
                }
            }

            this.Content = sbContent.ToString();
        }

        private void GenerateStackTraceInfo(Exception ex)
        {
            string stackTraceInfo;
            if (ex == null)
            {
                stackTraceInfo = this.GetStackTraceInfo();
            }
            else
            {
                if (ex.InnerException != null)
                {
                    var innerExList = new List<Exception>();
                    Exception innerEx = ex;
                    while (innerEx.InnerException != null)
                    {
                        innerExList.Add(innerEx);
                        innerEx = innerEx.InnerException;
                    }

                    StringBuilder sbStackTraceInfo = new StringBuilder();
                    sbStackTraceInfo.AppendLine(innerEx.StackTrace);
                    for (int i = innerExList.Count - 1; i >= 0; i--)
                    {
                        sbStackTraceInfo.AppendLine("   --- 内部异常堆栈跟踪的结尾 ---");
                        sbStackTraceInfo.Append(innerExList[i].StackTrace);
                        if (i > 0)
                        {
                            sbStackTraceInfo.AppendLine();
                        }
                    }

                    stackTraceInfo = sbStackTraceInfo.ToString();
                }
                else
                {
                    stackTraceInfo = ex.StackTrace;
                }
            }

            this.StackTraceInfo = stackTraceInfo;
        }

        /// <summary>
        /// 获取堆栈信息字符串
        /// </summary>
        /// <returns>堆栈信息字符串</returns>
        private string GetStackTraceInfo()
        {
            StackFrame sf = this.StackTrace.GetFrame(0);
            string fileName = sf.GetFileName();
            int lineNo = sf.GetFileLineNumber();
            //int colNo = sf.GetFileColumnNumber();

            MethodBase methodBase = sf.GetMethod();
            this.Logger = methodBase.DeclaringType.FullName;
            string methodName = methodBase.Name;
            ParameterInfo[] parameters = methodBase.GetParameters();
            string parameterStr;
            if (parameters.Length > 0)
            {
                parameterStr = this.GetMethodParamtersString(parameters);
            }
            else
            {
                parameterStr = string.Empty;
            }

            //在 NTest.FTestLMQ.btnTest_Click(Object sender, EventArgs e) 位置 E:\Projects\Zhanghn\UtilitiesLib\NTest\FTestLMQ.cs:行号 88
            return string.Format(@"   在 {0}.{1}({2}) 位置 {3}:行号 {4}", this.Logger, methodName, parameterStr, fileName, lineNo);
        }

        /// <summary>
        /// 获得方法参数字符串
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns>方法参数字符串</returns>
        private string GetMethodParamtersString(ParameterInfo[] parameters)
        {
            StringBuilder sbParameter = new StringBuilder();
            bool getStackTraceMethodParameterNameType = _getStackTraceMethodParameterNameType;
            StringBuilder sbGenericTypeParameter = new StringBuilder();
            string parameterTypeName;
            foreach (ParameterInfo parameter in parameters)
            {
                if (getStackTraceMethodParameterNameType)
                {
                    if (parameter.ParameterType.IsGenericType)
                    {
                        try
                        {
                            sbGenericTypeParameter.Clear();
                            this.AppendGenericArgumentType(sbGenericTypeParameter, parameter.ParameterType);
                            parameterTypeName = sbGenericTypeParameter.ToString();
                        }
                        catch (Exception ex)
                        {
                            parameterTypeName = this.GetTypeNameStr(parameter.ParameterType);
                            LogSysInnerLog.OnRaiseLog(null, ex);
                        }
                    }
                    else
                    {
                        parameterTypeName = this.GetTypeNameStr(parameter.ParameterType);
                    }
                }
                else
                {
                    parameterTypeName = parameter.ParameterType.Name;
                }

                sbParameter.Append(parameterTypeName);
                sbParameter.Append(MethodParameterTypeParameterNameSpacing);
                sbParameter.Append(parameter.Name);
                sbParameter.Append(MethodParameterSpacing);
            }

            sbParameter = sbParameter.Remove(sbParameter.Length - MethodParameterSpacing.Length, MethodParameterSpacing.Length);
            return sbParameter.ToString();
        }

        /// <summary>
        /// 追加泛型类型参数类型名称
        /// </summary>
        /// <param name="sbParameter">参数StringBuilder</param>
        /// <param name="parameterType">参数类型</param>
        private void AppendGenericArgumentType(StringBuilder sbParameter, Type parameterType)
        {
            sbParameter.Append(this.GetTypeNameStr(parameterType));
            sbParameter.Append('<');
            var genericArguments = parameterType.GetGenericArguments();
            int lastItemIndex = genericArguments.Length - 1;
            for (int i = 0; i < genericArguments.Length; i++)
            {
                var argsType = genericArguments[i];
                if (argsType.IsGenericType)
                {
                    this.AppendGenericArgumentType(sbParameter, argsType);
                }
                else
                {
                    sbParameter.Append(this.GetTypeNameStr(argsType));
                }

                if (i < lastItemIndex)
                {
                    sbParameter.Append(", ");
                }
            }

            sbParameter.Append('>');
        }

        /// <summary>
        /// 获取类型名称字符串
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型名称字符串</returns>
        private string GetTypeNameStr(Type type)
        {
            string typeNameStr;
            try
            {
                if (type.IsPrimitive)
                {
                    typeNameStr = type.Name;
                }
                else
                {
                    if (type.IsGenericType)
                    {
                        int splitIndex = type.Name.IndexOf('`');
                        if (splitIndex < 1)
                        {
                            typeNameStr = type.FullName;
                        }
                        else
                        {
                            typeNameStr = type.Name.Substring(0, splitIndex);
                        }
                    }
                    else
                    {
                        if (TypeCode.String == Type.GetTypeCode(type))
                        {
                            typeNameStr = type.Name;
                        }
                        else
                        {
                            typeNameStr = type.FullName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(null, ex);
                typeNameStr = type.FullName;
            }

            return typeNameStr;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            string logMsg;
            try
            {
                logMsg = this.Message;
                if (string.IsNullOrWhiteSpace(logMsg))
                {
                    if (this.Exception != null)
                    {
                        logMsg = this.Exception.ToString();
                    }
                }
                else
                {
                    if (this.Exception != null)
                    {
                        logMsg = string.Format("{0},{1}", logMsg, this.Exception.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog("LogItem", ex);
                logMsg = "日志异常";
            }

            return logMsg;
        }
    }
}
