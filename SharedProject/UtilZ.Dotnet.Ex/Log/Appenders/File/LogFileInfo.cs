using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    internal class LogFileInfo
    {
        public DateTime CreateTime { get; private set; }

        public string FilePath { get; private set; }

        public LogFileInfo(DateTime createTime, string filePath)
        {
            this.CreateTime = createTime;
            this.FilePath = filePath;
        }
    }
}
