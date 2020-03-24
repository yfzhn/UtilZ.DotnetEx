using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.DBIBase.Model
{
    /// <summary>
    /// 数据库发展信息
    /// </summary>
    public class DatabasePropertyInfo
    {
        /// <summary>
        /// 获取内存占用大小，单位/字节
        /// </summary>
        public long MemorySize { get; protected set; }

        /// <summary>
        /// 获取磁盘空间占用大小，单位/字节
        /// </summary>
        public long DiskSize { get; protected set; }

        /// <summary>
        /// 获取最大连接数
        /// </summary>
        public int MaxConnectCount { get; protected set; }

        /// <summary>
        /// 获取总连接数
        /// </summary>
        public int TotalConnectCount { get; protected set; }

        /// <summary>
        /// 获取并发连接数
        /// </summary>
        public int ConcurrentConnectCount { get; protected set; }

        /// <summary>
        /// 获取活动连接数
        /// </summary>
        public int ActiveConnectCount { get; protected set; }

        /// <summary>
        /// 获取所有用户名列表
        /// </summary>
        public List<string> AllUserNameList { get; protected set; }

        /// <summary>
        /// 获取数据库启动时间
        /// </summary>
        public DateTime StartTime { get; protected set; }

        /// <summary>
        /// 获取数据库创建时间
        /// </summary>
        public DateTime CreatetTime { get; protected set; }

        /// <summary>
        /// 获取数据库平均CPU占用率,用百分比表示
        /// </summary>
        public float CPURate { get; protected set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memorySize">获取内存占用大小，单位/字节</param>
        /// <param name="diskSize">磁盘空间占用大小，单位/字节</param>
        /// <param name="maxConnectCount">获取最大连接数</param>
        /// <param name="totalConnectCount">总连接数</param>
        /// <param name="concurrentConnectCount">并发连接数</param>
        /// <param name="activeConnectCount">活动连接数</param>
        /// <param name="allUserNameList">所有用户名列表</param>
        /// <param name="startTime">数据库启动时间</param>
        /// <param name="createtTime">数据库创建时间</param>
        /// <param name="cpuRate">数据库平均CPU占用率,用百分比表示</param>
        public DatabasePropertyInfo(long memorySize, long diskSize, int maxConnectCount, int totalConnectCount,
            int concurrentConnectCount, int activeConnectCount, List<string> allUserNameList, DateTime startTime, DateTime createtTime, float cpuRate)
        {
            this.MemorySize = memorySize;
            this.DiskSize = diskSize;
            this.MaxConnectCount = maxConnectCount;
            this.TotalConnectCount = totalConnectCount;
            this.ConcurrentConnectCount = concurrentConnectCount;
            this.ActiveConnectCount = activeConnectCount;
            this.AllUserNameList = allUserNameList;
            this.StartTime = startTime;
            this.CreatetTime = createtTime;
            this.CPURate = cpuRate;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return $@"
MemorySize;{this.MemorySize};
DiskSize:{this.DiskSize};
MaxConnectCount:{this.MaxConnectCount};
ConnectCount:{this.TotalConnectCount};
ConcurrentConnectCount:{this.ConcurrentConnectCount};
ActiveConnectCount:{ActiveConnectCount};
StartTime:{this.StartTime.ToString(UtilConstant.DateTimeFormat)};
CreatetTime:{this.CreatetTime.ToString(UtilConstant.DateTimeFormat)};
AllUserNameList:{string.Join(";", AllUserNameList.ToArray())};
CPURate:{CPURate}";
        }
    }
}
