using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACGImageByter
{
    public partial class MainForm : Form
    {

        Bitmap srcBitmap;

        Bitmap nowBitmap;

        int bitmapIndex = 0;

        int indexMax = 0;

        int iTag = 1;

        int jTag = 1;

        int level = 128;

        Bitmap[] bitmapBox = new Bitmap[100];

        ImageChangeTool tool = new ImageChangeTool();

        bool ifClose;

        int BitmapIndex { get{
                return bitmapIndex + 1;
            } 
        }


        public MainForm()
        {
            InitializeComponent();
            ifClose = false;
            saveFileDialog1.Filter = "Jpg 图片|*.jpg|Bmp 图片|*.bmp|Gif 图片|*.gif|Png 图片|*.png|Wmf  图片|*.wmf";
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                init();
                string path = openFileDialog1.FileName;
                srcBitmap = (Bitmap)Image.FromFile(path);
                bitmapBox[bitmapIndex] = srcBitmap.Clone() as Bitmap;
                nowBitmap = bitmapBox[bitmapIndex];
                srcPictureBox.Image = srcBitmap.Clone() as Image;
                showPictureBox.Image = srcBitmap.Clone() as Image;
            }
        }

        private void init()
        {

            srcBitmap = null;

            nowBitmap = null;

            bitmapIndex = 0;

            indexMax = 0;

            iTag = 1;

            Tag = 1;

            level = 128;

            for (int i = 0; i < bitmapBox.Length; i++)
            {
                bitmapBox[i] = null;
            }

        }

        private void changeProcessBar()
        {
            int value = (int)(iTag*100 / jTag);
            progressBar.Value = value;
            textBox1.Text = BitmapIndex.ToString();
        } 

        private void processBarThread()
        {
            while (!ifClose)
            {
                try
                {
                    Thread.Sleep(200);
                    if (this.IsHandleCreated)
                    {
                        Action ac = new Action(changeProcessBar);
                        this.Invoke(ac);
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private void balanceButton_Click(object sender, EventArgs e)
        {
            if (nowBitmap != null)
            {
                new Action(processBarThread).BeginInvoke(null, null);
                Thread thread1 = new Thread(new ThreadStart(balanceThread));
                thread1.Start();
            }
        }

        private void balanceThread()
        {
            Bitmap newbtm = tool.getHistogramEqualizationBitmap(nowBitmap, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
        }

        private void grayButton_Click(object sender, EventArgs e)
        {
            if (nowBitmap != null)
            {
                new Action(processBarThread).BeginInvoke(null, null);
                Thread thread1 = new Thread(new ThreadStart(grayThread));
                thread1.Start();
            }
        }

        private void grayThread()
        {
            Bitmap newbtm = tool.getGrayBitmap(nowBitmap, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ifClose = true;
            Thread.Sleep(1000);
            System.Environment.Exit(0);
        }


        private void avgButton_Click(object sender, EventArgs e)
        {
            if (nowBitmap != null)
            {
                new Action(processBarThread).BeginInvoke(null, null);
                Thread thread1 = new Thread(new ThreadStart(avgThread));
                thread1.Start();
            }
        }

        private void avgThread()
        {
            Bitmap newbtm = tool.getMidPointBitMap(nowBitmap,1, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
        }

        private void otsuButton_Click(object sender, EventArgs e)
        {
            if (nowBitmap != null)
            {
                new Action(processBarThread).BeginInvoke(null, null);
                Thread thread1 = new Thread(new ThreadStart(otsuThread));
                thread1.Start();
            }
        }

        private void otsuThread()
        {
            Bitmap newbtm = tool.getOtsuBitmap(nowBitmap,ref level, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (bitmapIndex-1 >= 0)
            {
                bitmapIndex--;
                textBox1.Text = BitmapIndex.ToString();
                nowBitmap = bitmapBox[bitmapIndex];
                showPictureBox.Image = nowBitmap.Clone() as Image;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (bitmapIndex+1 <= indexMax)
            {
                bitmapIndex++;
                textBox1.Text = BitmapIndex.ToString();
                nowBitmap = bitmapBox[bitmapIndex];
                showPictureBox.Image = nowBitmap.Clone() as Image;
            }
        }

        private void backSrcButton_Click(object sender, EventArgs e)
        {
            backToSrc();
        }

        private void backToSrc()
        {
            nowBitmap = srcBitmap;

            bitmapIndex = 0;

            iTag = 1;

            Tag = 1;

            level = 128;

            bitmapBox[0] = srcBitmap;

            for (int i = 1; i < bitmapBox.Length; i++)
            {
                bitmapBox[i] = null;
            }

            showPictureBox.Image = nowBitmap.Clone() as Image;
        }

        private void bernsenButton_Click(object sender, EventArgs e)
        {
            if (nowBitmap != null)
            {
                new Action(processBarThread).BeginInvoke(null, null);
                Thread thread1 = new Thread(new ThreadStart(bernsenThread));
                thread1.Start();
            }
        }

        private void bernsenThread()
        {
            Bitmap newbtm = tool.getBernsenBitmap(nowBitmap, 15,1,level, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
        }

        private void outButton_Click(object sender, EventArgs e)
        {
            bool isSave = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName.ToString();

                if (fileName != "" && fileName != null)
                {
                    string fileExtName = fileName.Substring(fileName.LastIndexOf(".") + 1).ToString();

                    System.Drawing.Imaging.ImageFormat imgformat = null;

                    if (fileExtName != "")
                    {
                        switch (fileExtName)
                        {
                            case "jpg":
                                imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                break;
                            case "bmp":
                                imgformat = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                            case "gif":
                                imgformat = System.Drawing.Imaging.ImageFormat.Gif;
                                break;
                            default:
                                MessageBox.Show("只能存取为: jpg,bmp,gif 格式");
                                isSave = false;
                                break;
                        }

                    }

                    //默认保存为JPG格式   
                    if (imgformat == null)
                    {
                        imgformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    }

                    if (isSave)
                    {
                        try
                        {
                            this.showPictureBox.Image.Save(fileName, imgformat);
                            MessageBox.Show("图片已经成功导出!");   
                        }
                        catch
                        {
                            MessageBox.Show("保存失败,你还没有截取过图片或已经清空图片!");
                        }
                    }
                }
            }
        }

        private void byteButton_Click(object sender, EventArgs e)
        {
            if (nowBitmap != null)
            {
                new Action(processBarThread).BeginInvoke(null, null);
                Thread thread1 = new Thread(new ThreadStart(byteThread));
                thread1.Start();
            }
        }

        private void byteThread()
        {
            backToSrc();
            Bitmap newbtm = tool.getHistogramEqualizationBitmap(nowBitmap, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
            newbtm = tool.getGrayBitmap(nowBitmap, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
            newbtm = tool.getMidPointBitMap(nowBitmap, 1, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
            newbtm = tool.getOtsuBitmap(nowBitmap, ref level, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
            nowBitmap = bitmapBox[bitmapIndex - 1];
            newbtm = tool.getBernsenBitmap(nowBitmap, 15, 1, level, ref iTag, ref jTag);
            bitmapIndex++;
            indexMax = bitmapIndex;
            bitmapBox[bitmapIndex] = newbtm;
            nowBitmap = newbtm;
            showPictureBox.Image = nowBitmap.Clone() as Image;
        }

    }
}
