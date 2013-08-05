using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontEngine
{
    public class GLS_Letter
    {
        public char letter;
        private int height = 0;
        private int width = 0;

        public byte[][] data;

        public bool dirty = true;

        public int Height
        {
            set
            {
                height = value;
            }
            get
            {
                return height;
            }
        }

        public int Width
        {
            set
            {
                width = value;
            }
            get
            {
                return width;
            }
        }

        public GLS_Letter()
        {
            letter = '\0';
            this.Height = 0;
            this.Width = 0;
        }

        public GLS_Letter(char c, int width, int height)
        {
            letter = c;
            this.Width = width - 1;
            this.Height = height - 1;
        }

        public override string ToString()
        {
            return letter.ToString();
        }

        private Bitmap cacheImage = null;
        private Texture2D cacheTexture = null;

        public Texture2D getTextureImage(GraphicsDevice device)
        {
            if ((dirty || cacheTexture == null) && this.TrueWidth != 0)
            {
                Texture2D img = new Texture2D(device, this.TrueWidth, this.height);
                Microsoft.Xna.Framework.Color[] colData = new Microsoft.Xna.Framework.Color[this.TrueWidth * this.Height];

                int i = 0;
                for (int y = 0; y < this.Height; y++)
                {
                    for (int x = 0; x < this.TrueWidth; x++)
                    {
                        if (data[x][y] != 0)
                        {
                            colData[i++] = Microsoft.Xna.Framework.Color.Black;
                        }
                        else
                        {
                            colData[i++] = Microsoft.Xna.Framework.Color.Transparent;
                        }
                    }
                }
                img.SetData<Microsoft.Xna.Framework.Color>(colData);
                img.GetData<Microsoft.Xna.Framework.Color>(colData);
                cacheTexture = img;
                dirty = false;
            }
            return cacheTexture;
        }

        public Bitmap getBitmapImage()
        {
            if ((dirty || cacheImage == null) && this.TrueWidth != 0)
            {
                Bitmap img = new Bitmap(this.TrueWidth, this.Height);
                for (int x = 0; x < this.TrueWidth; x++)
                {
                    for (int y = 0; y < this.Height; y++)
                    {
                        if (data[x][y] != 0)
                            img.SetPixel(x, y, System.Drawing.Color.Black);
                    }
                }
                cacheImage = img;
                dirty = false;
            }
            return cacheImage;
        }

        public int TrueWidth
        {
            get
            {
                if (this.letter == ' ') return 2;

                for (int x = 0; x < Width; x++)
                {
                    bool pass = false;
                    for (int y = 0; y < Height; y++)
                        if (data[x][y] != 0) pass = true;
                    if (!pass)
                        return x;
                }
                return Width;
            }
        }
    }
}
