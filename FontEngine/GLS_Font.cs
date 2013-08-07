using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;

namespace FontEngine
{
    public class GLS_Font : IEnumerable
    {
        private Dictionary<char, GLS_Letter> letters;

        private string path = "";
        GraphicsDevice device;
        int size = 1;
        int spacing = 1;

        public GLS_Font(string path)
        {
            this.path = path;
            this.letters = new Dictionary<char,GLS_Letter>();
            Load();
        }

        public GLS_Font(GraphicsDevice device, string path, int size)
        {
            this.path = path;
            this.letters = new Dictionary<char, GLS_Letter>();
            this.device = device;
            this.size = size;
            Load();
        }

        private void Load()
        {
            XmlDocument d = new XmlDocument();
            d.Load(this.path);
            foreach (XmlNode v in d["root"].ChildNodes)
            {
                GLS_Letter l = (GLS_Letter)new JavaScriptSerializer().Deserialize(v.InnerText, typeof(GLS_Letter));
                letters.Add(l.letter, l);
            }
        }

        public void Save()
        {
            XmlDocument d = new XmlDocument();
            d.AppendChild(d.CreateElement("root"));

            foreach (GLS_Letter l in this.letters.Values)
            {
                var v = new JavaScriptSerializer().Serialize(l);

                XmlElement element = d.CreateElement("item");
                element.InnerText = v;

                d["root"].AppendChild(element);
            }
            d.Save(this.path);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return letters.Values.GetEnumerator();
        }

        public GLS_Letter addLetter(GLS_Letter v)
        {
            if (!this.letters.ContainsKey(v.letter))
            {
                this.letters.Add(v.letter, v);
                return this.letters[v.letter];
            }
            return null;
        }

        public System.Drawing.Point addPoints(System.Drawing.Point a, System.Drawing.Point b)
        {
            return new System.Drawing.Point(a.X + b.X, a.Y + b.Y);
        }

        public void drawString(string str, System.Drawing.Point target, Graphics gra)
        {
            int across = 10;
            foreach (char c in str)
            {
                if (letters.ContainsKey(c))
                {
                    GLS_Letter l = letters[c];
                    if (l.letter == c)
                    {
                        Image img = l.getBitmapImage();
                        if (img != null)
                        {
                            gra.DrawImage(img, addPoints(target, new System.Drawing.Point(across, 0)));
                            across += l.TrueWidth + spacing;
                        }
                    }
                }
            }
        }

        public Vector2 Measure(string p)
        {
            int across = 0;
            foreach (char c in p)
            {
                across += this.letters[c].TrueWidth * size + spacing;
            }

            Vector2 v;
            v.X = across;
            v.Y = 20;

            return v;
        }

        public void Draw(SpriteBatch batch, string p, Vector2 vector2, Microsoft.Xna.Framework.Color color)
        {
            int across = 0;

            //device.SamplerStates[0].Filter = TextureFilter.Point;

            foreach (char c in p)
            {
                if (letters.ContainsKey(c))
                {
                    GLS_Letter l = letters[c];
                    if (l.letter == c)
                    {
                        Image img = l.getBitmapImage();
                        if (img != null)
                        {
                            batch.Draw(
                                l.getTextureImage(device),
                                new Microsoft.Xna.Framework.Rectangle(
                                    (int)(vector2.X + across),
                                    (int)vector2.Y, 
                                    l.TrueWidth * size,
                                    l.Height * size),
                                Microsoft.Xna.Framework.Color.White);

                            across += l.TrueWidth + spacing;
                        }
                    }
                }
            }
        }

        public Vector2 MeasureString(string p)
        {
            return Measure(p);
        }
    }
}
