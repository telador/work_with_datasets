using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = button3.Enabled = button4.Enabled = false;
        }

        Bitmap image = new Bitmap(960, 540);
        int n = 0;
        int[,] m = new int[960 * 540, 2];

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            string filename = openFileDialog1.FileName;
            TextReader f = File.OpenText(filename);
            
            
            string[] s = (f.ReadToEnd()).Split();
            int i = 0;
            n = 0;
            Array.Clear(m, 0, m.Length);
            while(i + 2 < s.Length)
            {
                m[n, 1] = int.Parse(s[i]);
                m[n, 0] = int.Parse(s[i + 1]);
                image.SetPixel(m[n, 0], image.Height - m[n, 1], Color.Black);
                n++;
                i += 3;
            }
            pictureBox1.Image = image;

            button2.Enabled = button3.Enabled = button4.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(960, 540); 
            folderBrowserDialog1.ShowDialog();
            pictureBox1.Image.Save(folderBrowserDialog1.SelectedPath + "\\result.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public bool LeftArea(int a, int b, int c)
        {
            return (m[a, 1] * (m[b, 0] - m[c, 0]) + m[b, 1] * (m[c, 0] - m[a, 0]) + m[c, 1] * (m[a, 0] - m[b, 0]) < 0);
        }

        public bool RightArea(int a, int b, int c)
        {
            return (m[a, 1] * (m[b, 0] - m[c, 0]) + m[b, 1] * (m[c, 0] - m[a, 0]) + m[c, 1] * (m[a, 0] - m[b, 0]) > 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Size = new Size(960, 540);
            int[] ans = new int[n];
            ans[0] = 0;
            int t = 1;
            for(int i = 0; i < n; i++)
            {
                if(i == n - 1 || RightArea(0, i, n - 1))
                {
                    while (t >= 2 && !RightArea(ans[t - 2], ans[t - 1], i))
                    {
                        t--;
                    }
                    ans[t] = i;
                    t++;
                }
            }
            int p = t;
            for (int i = n - 2; i >= 1; i--)
            {
                if (LeftArea(0, i, n - 1))
                {
                    while (t > p + 1 && !LeftArea(i, ans[t - 1], ans[t - 2]))
                    {
                        t--;
                    }
                    ans[t] = i;
                    t++;
                }
            }
            folderBrowserDialog1.ShowDialog();
            TextWriter f = File.CreateText(folderBrowserDialog1.SelectedPath + "\\ConvexHullDataset.txt");
            Graphics gr = Graphics.FromImage(image);
            gr.Clear(Color.Transparent);
            for(int i = 0; i < t; i++)
            {
                image.SetPixel(m[ans[i], 0], image.Height - m[ans[i], 1], Color.Black);
                if(i != 0)
                {
                    gr.DrawLine(Pens.Blue, m[ans[i-1], 0], image.Height - m[ans[i-1], 1], m[ans[i], 0], image.Height - m[ans[i], 1]);
                }
               
                f.Write((m[ans[i], 1].ToString() + " " + m[ans[i], 0].ToString()+"\n"));
            }
            pictureBox1.Image = image;
            f.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap affine = new Bitmap(960, 960);
            double angle = 6*Math.Acos(-1)/18;
            /*
            for (int i = 0; i < n; i++)
            {
                affine.SetPixel(m[i, 0], affine.Height - m[i, 1], Color.Black);
            }
            */
            for (int i = 0; i < n; i++)
            {
                affine.SetPixel(480 + Convert.ToInt32((m[i, 0] - 480) * Math.Cos(angle) - (m[i, 1] - 480) * Math.Sin(angle)), affine.Height - (480 + Convert.ToInt32((m[i, 1] - 480) * Math.Cos(angle) + (m[i, 0] - 480) * Math.Sin(angle))), Color.Blue);
            }
            /*
            Graphics gr = Graphics.FromImage(affine);
            gr.FillEllipse(Brushes.Red, 478, 478, 5, 5);
            */
            pictureBox1.Size = new Size(960, 960);
            pictureBox1.Image = affine;
        }
    }
}
