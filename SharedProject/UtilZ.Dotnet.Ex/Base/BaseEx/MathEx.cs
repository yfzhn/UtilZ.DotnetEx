using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Math扩展类
    /// </summary>
    public class MathEx
    {
        /// <summary>
        /// double0
        /// </summary>
        public const double ZERO_D = 0d;

        /// <summary>
        /// int0
        /// </summary>
        public const int ZERO_I = 0;

        /// <summary>
        /// 角度0°
        /// </summary>
        public const double ANGLE_0 = 0d;

        /// <summary>
        /// 角度90°
        /// </summary>
        public const double ANGLE_90 = 90d;

        /// <summary>
        /// 角度180°
        /// </summary>
        public const double ANGLE_180 = 180d;

        /// <summary>
        /// 角度270°
        /// </summary>
        public const double ANGLE_270 = 270d;

        /// <summary>
        /// 角度360°
        /// </summary>
        public const double ANGLE_360 = 360d;

        /// <summary>
        /// 角度转换为弧度
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static double AngleToRadians(double angle)
        {
            return angle * Math.PI / ANGLE_180;
        }

        /// <summary>
        /// 弧度转换为角度
        /// </summary>
        /// <param name="radians">弧度</param>
        /// <returns></returns>
        public static double RadiansToAngle(double radians)
        {
            return radians * ANGLE_180 / Math.PI;
        }

        /// <summary>
        /// 将角度值转换为0-360°
        /// </summary>
        /// <param name="angle">角度值</param>
        /// <returns>角度值</returns>
        public static double AbsAngle(double angle)
        {
            if (angle < ZERO_D || angle - ANGLE_360 >= ZERO_D)
            {
                angle = angle % ANGLE_360;
                if (angle < ZERO_D)
                {
                    angle += ANGLE_360;
                }
            }

            return angle;
        }

        /// <summary>
        /// 获取指定角度所在象限
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>角度所在象限</returns>
        public static Quadrant GetQuadrantByAngle(double angle)
        {
            angle = AbsAngle(angle);
            Quadrant quadrant;
            if (angle - MathEx.ANGLE_270 > ZERO_D)
            {
                //360°>angle>270°
                quadrant = Quadrant.Four;
            }
            else if (angle - MathEx.ANGLE_270 == ZERO_D)
            {
                //angle=270°
                quadrant = Quadrant.NegativeYAxisAngle;
            }
            else if (angle - MathEx.ANGLE_180 > ZERO_D)
            {
                //270°>angle>180°
                quadrant = Quadrant.Three;
            }
            else if (angle - MathEx.ANGLE_180 == ZERO_D)
            {
                //angle=180°
                quadrant = Quadrant.NegativeXAxisAngle;
            }
            else if (angle - MathEx.ANGLE_90 > ZERO_D)
            {
                //180°>angle>90°
                quadrant = Quadrant.Two;
            }
            else if (angle - MathEx.ANGLE_90 == ZERO_D)
            {
                //angle=90°
                quadrant = Quadrant.PositiveYAxisAngle;
            }
            else if (angle - MathEx.ANGLE_0 > ZERO_D)
            {
                //90°>angle>0°
                quadrant = Quadrant.One;
            }
            else
            {
                //angle=0°
                quadrant = Quadrant.PositiveXAxisAngle;
            }

            return quadrant;
        }
    }

    /// <summary>
    /// XY坐标系象限类型枚举
    /// </summary>
    public enum Quadrant
    {
        /// <summary>
        /// 正X轴轴线角,即0°角
        /// </summary>
        PositiveXAxisAngle,

        /// <summary>
        /// <![CDATA[第一象限(x>0,y>0)]]>
        /// </summary>
        One,

        /// <summary>
        /// 正Y轴轴线角,即90°角
        /// </summary>
        PositiveYAxisAngle,

        /// <summary>
        /// <![CDATA[第二象限(x<0,y>0)]]>
        /// </summary>
        Two,

        /// <summary>
        /// 负X轴轴线角,即180°角
        /// </summary>
        NegativeXAxisAngle,

        /// <summary>
        /// <![CDATA[第三象限(x<0,y<0)]]>
        /// </summary>
        Three,

        /// <summary>
        /// 负Y轴轴线角,即270°角
        /// </summary>
        NegativeYAxisAngle,

        /// <summary>
        /// <![CDATA[第四象限(x>0,y<0)]]>
        /// </summary>
        Four
    }
}
