using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Demo
{
    /// <summary>
    /// 态势公共方法类
    /// </summary>
    internal class PostureCommon
    {
        /// <summary>
        /// 经度或纬度度分秒格式化字符串
        /// </summary>
        private readonly static string _longitudeLatitudeAMSStr = @"{0}°{1}′{2}″";

        /// <summary>
        /// 经度或纬度坐标字符串(80°20′52″格式)拆分字符数组
        /// </summary>
        private readonly static char[] _longitudeLatitudeSplitArr = new char[] { '°', '′', '″' };

        #region 无效坐标判断
        /// <summary>
        /// 判断经度或纬度是否是无效的[true:无效,false:有效]
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="invalidLongitude">无效经度值</param>
        /// <param name="invalidLatitude">无效纬度值</param>
        /// <returns>经度或纬度是否是无效的[true:无效,false:有效]</returns>
        public static bool IsNoValid(double longitude, double latitude, double invalidLongitude, double invalidLatitude)
        {
            return PostureCommon.LongitudeIsNoValid(longitude, invalidLongitude) || PostureCommon.LatitudeIsNoValid(latitude, invalidLatitude);
        }

        /// <summary>
        /// 判断经度是否是无效的[true:无效,false:有效]
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="invalidLongitudeLatitude">无效经度值</param>
        /// <returns>经度是否是无效的[true:无效,false:有效]</returns>
        public static bool LongitudeIsNoValid(double longitude, double invalidLongitudeLatitude)
        {
            //若经度为PostureConstants.InvalidLongitudeLatitude度度则为无效的经度
            return Math.Abs(longitude - invalidLongitudeLatitude) < 0.0001;
        }

        /// <summary>
        /// 判断纬度是否是无效的[true:无效,false:有效]
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="invalidLatitude">无效纬度值</param>
        /// <returns>纬度是否是无效的[true:无效,false:有效]</returns>
        public static bool LatitudeIsNoValid(double latitude, double invalidLatitude)
        {
            //若纬度为PostureConstants.InvalidLongitudeLatitude度则为无效的纬度
            return Math.Abs(latitude - invalidLatitude) < 0.0001;
        }
        #endregion

        /// <summary>
        /// 转换坐标值转换为坐标显示字符串,小数格式坐标为度分秒格式字符串(110.54->110°45′00″)
        /// </summary>
        /// <param name="longitude">经度字符串</param>
        /// <returns>度分秒格式字符串</returns>
        public static string ConvertToCoorString(double longitude)
        {
            int a = 0, m = 0, s = 0;//度分秒
            a = (int)longitude;
            double md = (longitude - a) * 60;
            m = (int)md;
            s = (int)((md - m) * 60);
            return string.Format(PostureCommon._longitudeLatitudeAMSStr, a, m, s);
        }

        #region 坐标显示字符串转换为坐标值
        /// <summary>
        /// 转换经纬度坐标字符串为经纬度坐标值[转换成功返回true,否则返回false]
        /// </summary>
        /// <param name="objLongitude">经度坐标字符串值</param>
        /// <param name="objLatitude">纬度坐标字符串值</param>
        /// <param name="longitude">经度值</param>
        /// <param name="latitude">纬度值</param>
        /// <returns>转换成功返回true,否则返回false</returns>
        public static bool ConvertCoordStrToCoord(object objLongitude, object objLatitude, out double longitude, out double latitude)
        {
            longitude = 0;
            latitude = 0;

            try
            {
                //80°20′52″
                if (!PostureCommon.ConvertCoordStrToCoord(objLongitude, out longitude))
                {
                    return false;
                }

                return PostureCommon.ConvertCoordStrToCoord(objLatitude, out latitude);
            }
            catch (Exception ex)
            {
                Loger.Debug(ex);
                return false;
            }
        }

        /// <summary>
        /// 转换坐标字符串为坐标值[转换成功返回true,否则返回false]
        /// </summary>
        /// <param name="objValue">坐标字符串值</param>
        /// <param name="value">坐标值</param>
        /// <returns>转换成功返回true,否则返回false</returns>
        public static bool ConvertCoordStrToCoord(object objValue, out double value)
        {
            value = 0d;
            //如果经度值或纬度值为null则获取失败,返回false
            if (objValue == null || objValue is System.DBNull)
            {
                return false;
            }

            double a = 0d, m = 0d, s = 0d;
            //坐标字符串
            string coordStr = objValue.ToString();
            string[] longitudeArr = coordStr.Split(PostureCommon._longitudeLatitudeSplitArr, StringSplitOptions.RemoveEmptyEntries);
            if (longitudeArr.Length > 3)
            {
                return false;
            }

            if (longitudeArr.Length > 0)
            {
                a = double.Parse(longitudeArr[0]);
            }

            if (longitudeArr.Length > 1)
            {
                m = double.Parse(longitudeArr[1]);
            }

            if (longitudeArr.Length > 2)
            {
                s = double.Parse(longitudeArr[2]);
            }

            value = a + m / 60 + s / 3600;
            return true;
        }
        #endregion
    }
}
