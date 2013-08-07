using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using FontEngine;

namespace FontCreator
{
    public partial class Form1 : Form
    {
        BufferedGraphics bufferedGraphics;
        BufferedGraphics subBufferedGraphics;

        GLS_Font font;

        GLS_Letter selectedLetter
        {
            get
            {
                return (GLS_Letter)listBox1.SelectedItem;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("path.xml"))
            {
                load();
            }
            else
            {
                var list = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

                foreach (var c in list)
                {
                    var v = new GLS_Letter(c, (int)numericUpDown1.Value, (int)numericUpDown2.Value);
                    v.data = new byte[v.Width][];
                    for (int i = 0; i < v.Width; i++)
                        v.data[i] = new byte[v.Height];
                    listBox1.Items.Add(v);
                }
            }

            string str = "";
            foreach (char c in "abcdefghijklmnopqrstuvwxyz".ToCharArray())
            {
                str += c.ToString().ToUpper() + c;
            }
            textBox1.Text = str;

            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(panel1.CreateGraphics(), panel1.ClientRectangle);
            subBufferedGraphics = BufferedGraphicsManager.Current.Allocate(panel2.CreateGraphics(), panel2.ClientRectangle);

            Timer renderEngine = new Timer();
            renderEngine.Tick += renderEngine_Tick;
            renderEngine.Interval = 50;
            renderEngine.Start();
        }

        void save()
        {
            try
            {
                font.Save();
                File.Copy("path.xml", Environment.CurrentDirectory + "\\..\\..\\..\\Mojo\\bin\\WindowsGL\\Debug\\path.xml", true);
            }
            catch (Exception ee)
            {

            }
        }

        void load()
        {
            font = new GLS_Font("path.xml");
            foreach (GLS_Letter letter in font)
            {
                listBox1.Items.Add(letter);
            }
        }

        void renderEngine_Tick(object sender, EventArgs e)
        {
            Graphics g = bufferedGraphics.Graphics;

            g.Clear(Color.White);

            if (selectedLetter != null)
            {
                Point relP = new Point(panel1.Width / selectedLetter.Width, panel1.Height / selectedLetter.Height);
                for (int x = 0; x < selectedLetter.Width; x++)
                {
                    for (int y = 0; y < selectedLetter.Height; y++)
                    {
                        g.FillRectangle(selectedLetter.data[x][y] != 0 ? Brushes.Black : Brushes.White, new Rectangle(relP.X * x, relP.Y * y, relP.X, relP.Y));
                    }
                }
                for (int x = 0; x < 10; x += 5)
                {
                    g.DrawLine(Pens.Black, new Point(x * relP.X, 0), new Point(x * relP.X, 20 * relP.Y));
                }
                for (int y = 0; y < 20; y += 5)
                {
                    g.DrawLine(Pens.Black, new Point(0, y * relP.X), new Point(10 * relP.X, y * relP.Y));
                }
            }

            Graphics sub_g = subBufferedGraphics.Graphics;
            {
                sub_g.Clear(Color.White);
                font.drawString(textBox1.Text, new Point(5, 10), sub_g);
            }
            subBufferedGraphics.Render();

            bufferedGraphics.Render();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            selectedLetter.Width = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            selectedLetter.Height = (int)numericUpDown2.Value;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Width = selectedLetter.Width * 20;
            panel1.Height = selectedLetter.Height * 20;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            DrawToLetter(e);
        }

        private void DrawToLetter(MouseEventArgs e)
        {
            if (selectedLetter != null)
            {
                Point p = e.Location;
                Point relP = new Point(panel1.Width / selectedLetter.Width, panel1.Height / selectedLetter.Height);
                Point pos = new Point(p.X / relP.X, p.Y / relP.Y);
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    changeData(pos, 1);
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    changeData(pos, 0);
            }
        }

        public void changeData(Point target, byte value)
        {
            if (target.X < selectedLetter.Width && target.Y < selectedLetter.Height && target.X >= 0 && target.Y >= 0)
            {
                if (selectedLetter.data[target.X][target.Y] != value)
                {
                    selectedLetter.data[target.X][target.Y] = value;
                    selectedLetter.dirty = true;
                    save();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            char c = '\0';
            if (char.TryParse(textBox2.Text.Trim(), out c))
            {
                var v = new GLS_Letter(c, (int)numericUpDown1.Value, (int)numericUpDown2.Value);
                v.data = new byte[v.Width][];
                for (int i = 0; i < v.Width; i++)
                    v.data[i] = new byte[v.Height];
                GLS_Letter letter = font.addLetter(v);
                if (letter != null)
                    listBox1.Items.Add(letter);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            DrawToLetter(e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

   
}
