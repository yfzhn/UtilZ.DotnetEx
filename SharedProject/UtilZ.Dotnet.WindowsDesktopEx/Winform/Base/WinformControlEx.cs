using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// Winfrom扩展类
    /// </summary>
    public static class WinformControlEx
    {
        #region 生成控件图片
        /// <summary>
        /// 生成控件图片
        /// </summary>
        /// <param name="control">要生成图片的控件</param>
        /// <param name="isContainBorder">是否包含控件边框[true:包含边框;false:不包含边框;默认为false]</param>
        /// <returns>图片</returns>
        public static System.Drawing.Bitmap CreateControlImg(this System.Windows.Forms.Control control, bool isContainBorder = false)
        {
            if (control == null || control.IsDisposed)
            {
                return null;
            }

            System.Drawing.Bitmap bitmap;
            if (isContainBorder)
            {
                bitmap = CreateControlImgBorder(control);
            }
            else
            {
                bitmap = CreateControlImg(control);
            }

            return bitmap;
        }

        /// <summary>
        /// 生成控件图片，带边框
        /// </summary>
        /// <param name="control">要生成图片的控件</param>
        /// <returns>图片</returns>
        private static System.Drawing.Bitmap CreateControlImgBorder(System.Windows.Forms.Control control)
        {
            var bitmap = new System.Drawing.Bitmap(control.ClientRectangle.Width, control.ClientRectangle.Height);
            var g = System.Drawing.Graphics.FromImage(bitmap);
            var srcScreenPoint = control.PointToScreen(control.Location);
            var screenPoint = new System.Drawing.Point(srcScreenPoint.X - control.Location.X, srcScreenPoint.Y - control.Location.Y);
            g.CopyFromScreen(screenPoint, new System.Drawing.Point(0, 0), new System.Drawing.Size(bitmap.Width, bitmap.Height));
            g.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 生成控件图片
        /// </summary>
        /// <param name="control">要生成图片的控件</param>
        /// <returns>图片</returns>
        private static System.Drawing.Bitmap CreateControlImg(System.Windows.Forms.Control control)
        {
            var bitmap = new System.Drawing.Bitmap(control.ClientRectangle.Width, control.ClientRectangle.Height);
            var g = System.Drawing.Graphics.FromImage(bitmap);
            int titileHeight = control.Height - control.ClientRectangle.Height;
            int borderWidth = (control.Width - control.ClientRectangle.Width) / 2;
            var srcScreenPoint = control.PointToScreen(control.Location);
            var screenPoint = new System.Drawing.Point(srcScreenPoint.X + borderWidth - control.Location.X, srcScreenPoint.Y + titileHeight - borderWidth - control.Location.Y);
            g.CopyFromScreen(screenPoint, new System.Drawing.Point(0, 0), new System.Drawing.Size(bitmap.Width, bitmap.Height));
            g.Dispose();
            return bitmap;
        }
        #endregion

        #region 生成窗体图片
        /// <summary>
        /// 生成窗体图片
        /// </summary>
        /// <param name="form">要生成图片的窗体</param>
        /// <param name="isContainBorder">是否包含控件边框[true:包含边框;false:不包含边框;默认为false]</param>
        /// <param name="isContainTitle">是否包含标题栏</param>
        /// <returns>图片</returns>
        public static System.Drawing.Bitmap CreateFormImg(this System.Windows.Forms.Form form, bool isContainBorder = false, bool isContainTitle = false)
        {
            if (form == null || form.IsDisposed)
            {
                return null;
            }

            System.Drawing.Bitmap bitmap;
            if (isContainTitle)
            {
                if (isContainBorder)
                {
                    bitmap = CreateFormImgBorderTitle(form);
                }
                else
                {
                    bitmap = CreateFormImgTitle(form);
                }
            }
            else
            {
                if (isContainBorder)
                {
                    bitmap = CreateFormImgBorder(form);
                }
                else
                {
                    bitmap = CreateFormImg(form);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// 生成窗体图片，带边框,带标题
        /// </summary>
        /// <param name="form">要生成图片的窗体</param>
        /// <returns>图片</returns>
        private static System.Drawing.Bitmap CreateFormImgBorderTitle(System.Windows.Forms.Form form)
        {
            var bitmap = new System.Drawing.Bitmap(form.Width, form.Height);
            var g = System.Drawing.Graphics.FromImage(bitmap);
            var screenPoint = new System.Drawing.Point(form.Location.X, form.Location.Y);
            g.CopyFromScreen(screenPoint, new System.Drawing.Point(0, 0), new System.Drawing.Size(bitmap.Width, bitmap.Height));
            g.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 生成窗体图片,带标题
        /// </summary>
        /// <param name="form">要生成图片的窗体</param>
        /// <returns>图片</returns>
        private static System.Drawing.Bitmap CreateFormImgTitle(System.Windows.Forms.Form form)
        {
            int borderWidth = (form.Width - form.ClientRectangle.Width) / 2;
            int titileHeight = form.Height - form.ClientRectangle.Height;
            var bitmap = new System.Drawing.Bitmap(form.ClientRectangle.Width, form.ClientRectangle.Height + titileHeight - borderWidth);
            var g = System.Drawing.Graphics.FromImage(bitmap);
            var screenPoint = new System.Drawing.Point(form.Location.X + borderWidth, form.Location.Y);
            g.CopyFromScreen(screenPoint, new System.Drawing.Point(0, 0), new System.Drawing.Size(bitmap.Width, bitmap.Height));
            g.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 生成窗体图片，带边框
        /// </summary>
        /// <param name="form">要生成图片的窗体</param>
        /// <returns>图片</returns>
        private static System.Drawing.Bitmap CreateFormImgBorder(System.Windows.Forms.Form form)
        {
            int borderWidth = (form.Width - form.ClientRectangle.Width) / 2;
            int titileHeight = form.Height - form.ClientRectangle.Height;
            var bitmap = new System.Drawing.Bitmap(form.Width, form.Height - titileHeight + borderWidth);
            var g = System.Drawing.Graphics.FromImage(bitmap);
            var screenPoint = new System.Drawing.Point(form.Location.X, form.Location.Y + titileHeight - borderWidth);
            g.CopyFromScreen(screenPoint, new System.Drawing.Point(0, 0), new System.Drawing.Size(bitmap.Width, bitmap.Height));
            g.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 生成窗体图片
        /// </summary>
        /// <param name="form">要生成图片的窗体</param>
        /// <returns>图片</returns>
        private static System.Drawing.Bitmap CreateFormImg(System.Windows.Forms.Form form)
        {
            var bitmap = new System.Drawing.Bitmap(form.ClientRectangle.Width, form.ClientRectangle.Height);
            var g = System.Drawing.Graphics.FromImage(bitmap);
            int titileHeight = form.Height - form.ClientRectangle.Height;
            int borderWidth = (form.Width - form.ClientRectangle.Width) / 2;
            var screenPoint = new System.Drawing.Point(form.Location.X + borderWidth, form.Location.Y + titileHeight - borderWidth);
            g.CopyFromScreen(screenPoint, new System.Drawing.Point(0, 0), new System.Drawing.Size(bitmap.Width, bitmap.Height));
            g.Dispose();
            return bitmap;
        }
        #endregion
    }
}
