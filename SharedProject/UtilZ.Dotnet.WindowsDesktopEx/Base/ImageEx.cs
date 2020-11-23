using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// 图片扩展类
    /// </summary>
    public static class ImageEx
    {
        /// <summary>
        /// 将各种图片转换为Ico文件，
        /// (以下四种文件经测试可行jpg-bmp-png-gif,其它不常见格式未测试)
        /// </summary>
        /// <param name="srcImgFilePath">待转换图片路径</param>
        /// <param name="iconFilePath">转换后icon文件路径</param>
        public static void ConvertToIcon(string srcImgFilePath, string iconFilePath)
        {
            if (string.IsNullOrWhiteSpace(srcImgFilePath))
            {
                throw new ArgumentNullException(nameof(srcImgFilePath));
            }

            if (!File.Exists(srcImgFilePath))
            {
                throw new FileNotFoundException("待转换图片不存在", srcImgFilePath);
            }

            if (string.IsNullOrWhiteSpace(iconFilePath))
            {
                throw new ArgumentNullException(nameof(iconFilePath));
            }

            DirectoryInfoEx.CheckFilePathDirectory(iconFilePath);

            using (var src = new Bitmap(srcImgFilePath))
            {
                using (var dst = new Bitmap(src, src.Size))
                {
                    Icon icon = Icon.FromHandle(dst.GetHicon());
                    using (var stream = new FileStream(iconFilePath, FileMode.Create))
                    {
                        icon.Save(stream);
                    }
                }
            }
        }












        /// <summary>
        /// 改变图片的透明度
        /// </summary>
        /// <param name="srcImgFilePath">原图片路径</param>
        /// <param name="targetImgFilePath">处理后的图片存放路径</param>
        /// <param name="opacity">透明度，默认值为125</param>
        public static void ChangeImageOpacity(string srcImgFilePath, string targetImgFilePath, byte opacity = 125)
        {
            if (!System.IO.File.Exists(srcImgFilePath))
            {
                throw new System.IO.FileNotFoundException("图片不存在", srcImgFilePath);
            }

            var image = System.Drawing.Image.FromFile(srcImgFilePath);
            var targetImg = ChangeImageOpacity(image, opacity);
            targetImg.Save(targetImgFilePath);
        }

        private static void LockUnlockBitsExample()
        {
            // Create a new bitmap.
            Bitmap bmp = new Bitmap("c:\\fakePhoto.jpg");

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Set every third value to 255. A 24bpp bitmap will look red.  
            for (int counter = 2; counter < rgbValues.Length; counter += 3)
            {
                rgbValues[counter] = 255;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            // Draw the modified image.
            //e.Graphics.DrawImage(bmp, 0, 150);

        }

        /// <summary>
        /// 改变图片的透明度
        /// </summary>
        /// <param name="image">目标图片</param>
        /// <param name="opacity">透明度，默认值为125</param>
        /// <returns>修改透明度之后的图片</returns>
        public static System.Drawing.Image ChangeImageOpacity(this System.Drawing.Image image, byte opacity = 125)
        {
            if (image == null)
            {
                return null;
            }

            var bmp = new System.Drawing.Bitmap(image);
            //System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            IntPtr ptr = bmpData.Scan0;
            int dataLength = Math.Abs(bmpData.Stride) * bmp.Height / 4;
            int[] rgbValues = new int[dataLength];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, dataLength);

            //for (int counter = 0; counter < dataLength; counter++)
            //{
            //    System.Drawing.Color c = Color.FromArgb(rgbValues[counter]);
            //    rgbValues[counter] = System.Drawing.Color.FromArgb(opacity, c.R, c.G, c.B).ToArgb();
            //}

            int processCoreCount = dataLength / Environment.ProcessorCount;
            int mod = dataLength % Environment.ProcessorCount;
            List<Tuple<int, int>> items = new List<Tuple<int, int>>();
            int begin = 0, end, lastProcessprIndex = Environment.ProcessorCount - 1;
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                end = (i + 1) * processCoreCount;
                if (i == lastProcessprIndex)
                {
                    end += mod;
                }

                items.Add(new Tuple<int, int>(begin, end));
                begin = end;
            }

            Parallel.ForEach(items, (item) =>
            {
                for (int i = item.Item1; i < item.Item2; i++)
                {
                    System.Drawing.Color c = Color.FromArgb(rgbValues[i]);
                    rgbValues[i] = System.Drawing.Color.FromArgb(opacity, c.R, c.G, c.B).ToArgb();
                }
            });

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, dataLength);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        /// <summary>
        /// 改变图片的透明度
        /// </summary>
        /// <param name="image">目标图片</param>
        /// <param name="opacity">透明度，默认值为125</param>
        /// <returns>修改透明度之后的图片</returns>
        public static System.Drawing.Image ChangeImageOpacity_bk2(this System.Drawing.Image image, byte opacity = 125)
        {
            if (image == null)
            {
                return null;
            }

            var img = new System.Drawing.Bitmap(image);
            using (var bmp = new System.Drawing.Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.DrawImage(img, 0, 0);

                    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    int dataLength = bmpData.Stride * bmpData.Height / 4;
                    int[] data = new int[dataLength];
                    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, data, 0, dataLength);

                    int processCoreCount = dataLength / Environment.ProcessorCount;
                    int mod = dataLength % Environment.ProcessorCount;
                    List<Tuple<int, int>> items = new List<Tuple<int, int>>();
                    int begin = 0, end, lastProcessprIndex = Environment.ProcessorCount - 1;
                    for (int i = 0; i < Environment.ProcessorCount; i++)
                    {
                        end = (i + 1) * processCoreCount;
                        if (i == lastProcessprIndex)
                        {
                            end += mod;
                        }

                        items.Add(new Tuple<int, int>(begin, end));
                        begin = end;
                    }

                    Parallel.ForEach(items, (item) =>
                    {
                        for (int i = item.Item1; i < item.Item2; i++)
                        {
                            System.Drawing.Color c = Color.FromArgb(data[i]);
                            data[i] = System.Drawing.Color.FromArgb(opacity, c.R, c.G, c.B).ToArgb();
                        }
                    });

                    System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpData.Scan0, dataLength);
                    bmp.UnlockBits(bmpData);
                    return bmp;
                }
            }
        }

        /// <summary>
        /// 改变图片的透明度
        /// </summary>
        /// <param name="image">目标图片</param>
        /// <param name="opacity">透明度，默认值为125</param>
        /// <returns>修改透明度之后的图片</returns>
        public static System.Drawing.Image ChangeImageOpacity_bk(this System.Drawing.Image image, byte opacity = 125)
        {
            if (image == null)
            {
                return null;
            }

            var img = new System.Drawing.Bitmap(image);
            using (var bmp = new System.Drawing.Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.DrawImage(img, 0, 0);
                    for (int h = 0; h <= img.Height - 1; h++)
                    {
                        for (int w = 0; w <= img.Width - 1; w++)
                        {
                            System.Drawing.Color c = img.GetPixel(w, h);
                            bmp.SetPixel(w, h, System.Drawing.Color.FromArgb(opacity, c.R, c.G, c.B));
                        }
                    }

                    return (System.Drawing.Image)bmp.Clone();
                }
            }
        }

        /*
        public class DirectBitmap
        {
            System.Drawing.Bitmap _bitmap;
            System.Drawing.Imaging.BitmapData _bitmapData;
            int[] _data;

            public DirectBitmap(System.Drawing.Image image)
            {
                _bitmap = new System.Drawing.Bitmap(image);
                Open();
            }

            private void Open()
            {
                _bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                _data = new int[_bitmapData.Stride * _bitmapData.Height / 4];
                System.Runtime.InteropServices.Marshal.Copy(_bitmapData.Scan0, _data, 0, _bitmapData.Stride * _bitmapData.Height / 4);
            }

            public Image Image
            {
                get
                {
                    System.Runtime.InteropServices.Marshal.Copy(_data, 0, _bitmapData.Scan0, _bitmapData.Stride * _bitmapData.Height / 4);
                    _bitmap.UnlockBits(_bitmapData);
                    Image result = new Bitmap(_bitmap);
                    Open();
                    return result;
                }
            }

            public Color GetPixel(int x, int y)
            {
                return Color.FromArgb(_data[y * _bitmapData.Stride / 4 + x]);
            }

            public void SetPixel(int x, int y, Color c)
            {
                _data[y * _bitmapData.Stride / 4 + x] = c.ToArgb();
            }
        }
        */
    }
}
