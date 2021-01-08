using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace ACGImageByter
{
    class ImageChangeTool
    {
        public Bitmap getGrayBitmap(Bitmap btm ,ref int itag, ref int jtag) {
            Bitmap newbitmap = btm.Clone() as Bitmap;
            int width = btm.Width;
            int height = btm.Height;
            Rectangle rect = new Rectangle(0, 0, newbitmap.Width, newbitmap.Height);
            BitmapData data = null;
            data = newbitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, newbitmap.PixelFormat);
            IntPtr srcPtr = data.Scan0;
            int scanWidth = data.Stride;
            int src_bytes = height * scanWidth;
            byte[] srcRGBValues = new byte[src_bytes];
            Marshal.Copy(srcPtr, srcRGBValues, 0, src_bytes);
            newbitmap.UnlockBits(data);
            itag = 0;
            jtag = width * height;
            int k = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    k = j * 3;
                    byte R = srcRGBValues[i * scanWidth + k + 2];
                    byte G = srcRGBValues[i * scanWidth + k + 1];
                    byte B = srcRGBValues[i * scanWidth + k + 0];
                    int gray = (int)(R * 0.3 + G * 0.59 + B * 0.11);
                    newbitmap.SetPixel(j, i, Color.FromArgb(gray, gray, gray));
                    itag++;
                }
            }
            return newbitmap;
        }

        public Bitmap getHistogramEqualizationBitmap(Bitmap btm, ref int itag, ref int jtag)
        {
            Bitmap newbtm = btm.Clone() as Bitmap;
            int width = btm.Width;
            int height = btm.Height;
            Rectangle rect = new Rectangle(0, 0, newbtm.Width, newbtm.Height);
            BitmapData data = null;
            data = newbtm.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, newbtm.PixelFormat);
            IntPtr srcPtr = data.Scan0;
            int scanWidth = data.Stride;
            int src_bytes = height * scanWidth;
            byte[] srcRGBValues = new byte[src_bytes];
            Marshal.Copy(srcPtr, srcRGBValues, 0, src_bytes);
            //解锁位图
            newbtm.UnlockBits(data);
            double[] Rnumberbox = new double[256];
            int[] Rlevelbox = new int[256];
            double[] Gnumberbox = new double[256];
            int[] Glevelbox = new int[256];
            double[] Bnumberbox = new double[256];
            int[] Blevelbox = new int[256];
            double number = width * height;
            int index = 0;
            itag = 0;
            jtag = width * height * 2;
            int k = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    k = j * 3;
                    index = srcRGBValues[i * scanWidth + k + 2];
                    Rnumberbox[index]++;
                    index = srcRGBValues[i * scanWidth + k + 1];
                    Gnumberbox[index]++;
                    index = srcRGBValues[i * scanWidth + k + 0];
                    Bnumberbox[index]++;
                    itag++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                Rnumberbox[i] /= number;
                Gnumberbox[i] /= number;
                Bnumberbox[i] /= number;
            }
            for (int i = 1; i < 256; i++)
            {
                Rnumberbox[i] += Rnumberbox[i - 1];
                Gnumberbox[i] += Gnumberbox[i - 1];
                Bnumberbox[i] += Bnumberbox[i - 1];
            }
            for (int i = 0; i < 256; i++)
            {
                Rlevelbox[i] = (int)(Rnumberbox[i] * 255 + 0.5);
                Glevelbox[i] = (int)(Gnumberbox[i] * 255 + 0.5);
                Blevelbox[i] = (int)(Bnumberbox[i] * 255 + 0.5);
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    k = j * 3;
                    byte R = srcRGBValues[i * scanWidth + k + 2];
                    byte G = srcRGBValues[i * scanWidth + k + 1];
                    byte B = srcRGBValues[i * scanWidth + k + 0];
                    int tR = Rlevelbox[R];
                    int tG = Glevelbox[G];
                    int tB = Blevelbox[B];
                    Color color = Color.FromArgb(tR, tG, tB);
                    newbtm.SetPixel(j, i, color);
                    itag++;
                }
            }
            return newbtm;
        }

        public Bitmap getBernsenBitmap(Bitmap btm, int S, int m,int level, ref int itag, ref int jtag)
        {
            Bitmap newbitmap = btm.Clone() as Bitmap;
            int width = btm.Width;
            int height = btm.Height;

            Rectangle rect = new Rectangle(0, 0, newbitmap.Width, newbitmap.Height);
            BitmapData data = null;
            data = newbitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, newbitmap.PixelFormat);
            IntPtr srcPtr = data.Scan0;
            int scanWidth = data.Stride;
            int src_bytes = height * scanWidth;
            byte[] srcRGBValues = new byte[src_bytes];
            Marshal.Copy(srcPtr, srcRGBValues, 0, src_bytes);
            newbitmap.UnlockBits(data);
            itag = 0;
            int nk = 0;

            int max = 0;
            int min = 256;
            int mid = 0;
            int upleft, upright, downleft, downright;
            int limt = level;
            jtag = width * height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    upleft = (i - m) > 0 ? (i - m) : 0;
                    upright = (i + m) < height ? (i + m) : (height - 1);
                    downleft = (j - m) > 0 ? (j - m) : 0;
                    downright = (j + m) < width ? (j + m) : (width - 1);
                    max = 0;
                    min = 256;
                    for (int k = upleft; k <= upright; k++)
                    {
                        for (int l = downleft; l <= downright; l++)
                        {
                            nk = l * 3;
                            int b = srcRGBValues[k * scanWidth + nk + 0];
                            if (b > max)
                            {
                                max = b;
                            }
                            if (b < min)
                            {
                                min = b;
                            }
                        }
                    }
                    mid = (max + min) / 2;
                    nk = j * 3;
                    int colorB = srcRGBValues[i * scanWidth + nk + 0];
                    if (max - min > S)
                    {
                        Color color = colorB < mid ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                        newbitmap.SetPixel(j, i, color);
                    }
                    else
                    {
                        Color color = colorB < limt ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                        newbitmap.SetPixel(j, i, color);
                    }
                    itag++;
                }
            }
            return newbitmap;
        }

        public Bitmap getMidPointBitMap(Bitmap btm, int m, ref int itag, ref int jtag)
        {
            Bitmap newbitmap = btm.Clone() as Bitmap;
            int width = btm.Width;
            int height = btm.Height;

            Rectangle rect = new Rectangle(0, 0, newbitmap.Width, newbitmap.Height);
            BitmapData data = null;
            data = newbitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, newbitmap.PixelFormat);
            IntPtr srcPtr = data.Scan0;
            int scanWidth = data.Stride;
            int src_bytes = height * scanWidth;
            byte[] srcRGBValues = new byte[src_bytes];
            Marshal.Copy(srcPtr, srcRGBValues, 0, src_bytes);
            newbitmap.UnlockBits(data);
            itag = 0;
            int nk = 0;

            int max = 0;
            int min = 256;
            int upleft, upright, downleft, downright;
            jtag = width * height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    upleft = (i - m) > 0 ? (i - m) : 0;
                    upright = (i + m) < height ? (i + m) : (height - 1);
                    downleft = (j - m) > 0 ? (j - m) : 0;
                    downright = (j + m) < width ? (j + m) : (width - 1);
                    max = 0;
                    min = 256;
                    for (int k = upleft; k <= upright; k++)
                    {
                        for (int l = downleft; l <= downright; l++)
                        {
                            nk = l * 3;
                            int b = srcRGBValues[k * scanWidth + nk + 0];
                            if (b > max)
                            {
                                max = b;
                            }
                            if (b < min)
                            {
                                min = b;
                            }
                        }
                    }
                    int mid = (max + min) / 2;
                    newbitmap.SetPixel(j, i, Color.FromArgb(mid, mid, mid));
                    itag++;
                }
            }
            return newbitmap;
        }

        public Bitmap getOtsuBitmap(Bitmap btm, ref int level ,ref int itag, ref int jtag)
        {
            Bitmap newbtm = btm.Clone() as Bitmap;
            int width = btm.Width;
            int height = btm.Height;

            Rectangle rect = new Rectangle(0, 0, newbtm.Width, newbtm.Height);
            BitmapData data = null;
            data = newbtm.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, newbtm.PixelFormat);
            IntPtr srcPtr = data.Scan0;
            int scanWidth = data.Stride;
            int src_bytes = height * scanWidth;
            byte[] srcRGBValues = new byte[src_bytes];
            Marshal.Copy(srcPtr, srcRGBValues, 0, src_bytes);
            newbtm.UnlockBits(data);
            itag = 0;
            int k = 0;

            jtag = width * height * 2;
            int[] box = new int[256];
            int L = 0;
            double g = 0;
            double maxG = 0;
            int totalPixl = 0;
            int wNumber = 0;
            double wWeight = 0;
            int bNumber = 0;
            double bWeight = 0;
            double u0 = 0;
            double u1 = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    k = j * 3;
                    int color = srcRGBValues[i * scanWidth + k + 0];
                    box[color]++;
                    totalPixl++;
                    itag++;
                }
            }
            for (int i = 1; i < 255; i++)
            {
                wNumber = 0;
                bNumber = 0;
                wWeight = 0;
                bWeight = 0;
                for (int j = 0; j < i; j++)
                {
                    wNumber += box[j];
                    wWeight += box[j] * j;
                }
                for (int j = i; j < 256; j++)
                {
                    bNumber += box[j];
                    bWeight += box[j] * j;
                }
                u0 = wWeight / wNumber;
                u1 = bWeight / bNumber;
                g = (u1 - u0) * (u1 - u0) * wNumber * bNumber / totalPixl / totalPixl;
                if (g > maxG)
                {
                    maxG = g;
                    L = i;
                }
            }
            level = L;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    k = j * 3;
                    int B = srcRGBValues[i * scanWidth + k + 0];
                    Color color = B < level ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    newbtm.SetPixel(j, i, color);
                    itag++;
                }
            }
            return newbtm;
        }

        public Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)
        {
            System.Drawing.Image imgSource = b;
            System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;
            int sW = 0, sH = 0;
            // 按比例缩放           
            int sWidth = imgSource.Width;
            int sHeight = imgSource.Height;
            if (sHeight > destHeight || sWidth > destWidth)
            {
                if ((sWidth * destHeight) > (sHeight * destWidth))
                {
                    sW = destWidth;
                    sH = (destWidth * sHeight) / sWidth;
                }
                else
                {
                    sH = destHeight;
                    sW = (sWidth * destHeight) / sHeight;
                }
            }
            else
            {
                sW = sWidth;
                sH = sHeight;
            }
            Bitmap outBmp = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);
            // 设置画布的描绘质量         
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量     
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            imgSource.Dispose();
            return outBmp;
        }

    }
}
