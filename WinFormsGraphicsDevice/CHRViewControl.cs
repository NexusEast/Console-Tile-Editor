#region File Description
//-----------------------------------------------------------------------------
// SpinningTriangleControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Diagnostics;
//using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.ComponentModel; 
#endregion

namespace WinFormsGraphicsDevice
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to draw animating
    /// 3D graphics inside a WinForms application. It hooks the Application.Idle
    /// event, using this to invalidate the control, which will cause the animation
    /// to constantly redraw.
    /// </summary>
    class CHRViewControl : GraphicsDeviceControl
    {

        ContentManager content;
        Texture2D chr_map = null;
        SpriteBatch spriteBatch;
        List<Texture2D> grids = new List<Texture2D>(16 * 16);
        Rectangle selectedRect = new Rectangle();
        SpriteFont font;
        RenderTarget2D monotex;
        System.Windows.Forms.PictureBox _pb;
        byte[] chr_buffer;

        [Description("SetPictureBox"),
        Category("Data"),
        DefaultValue(null),
        Browsable(true)]
        public System.Windows.Forms.PictureBox SetPictureBox
        {
            get
            {
                return _pb;
            }
            set
            {
                _pb = value;
            }
        }

        Color[] paletteCol = {
                                 Color.Black,
                                 new Color(80, 80, 80),
                                 new Color(150, 150, 150),
                                 new Color(255, 255, 255)
                             };
        public void updatePalette(byte[] pal)
        {
            //update col first
            for (int i = 0; i < 4; i++)
                paletteCol[i] = ColorControl.palette[pal[i]];
            bytetotex(chr_buffer);
        }

        public void bytetotex(byte[] b)
        {
            if (b == null) return;
            chr_buffer = (byte[])b.Clone();
            chr_map = new Texture2D(GraphicsDevice, 128, 128);
            Color[] imageData = new Color[128 * 128];
            Color[] imageDataMono = new Color[128 * 128];
            //split into 1 


            int _index = 0;

            int cube_line = 0;
            int cube_row = 0;

            int line = 0;
            int colum = 0;

            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int idx = (i * 16) + j;



                    byte[] fullbyte = new byte[2];
                    fullbyte[0] = b[idx];
                    fullbyte[1] = b[idx + 8];
                    BitArray pat_bit = new BitArray(fullbyte);

                    for (int k = 0; k < pat_bit.Length / 2; k++)
                    {
                        string bitstr = "";
                        if (pat_bit[k]) bitstr += "1";
                        else bitstr += "0";
                        if (pat_bit[k + 8]) bitstr += "1";
                        else bitstr += "0";

                        Color col = new Color();
                        Color col_mono = new Color();
                        switch (bitstr)
                        {
                            case "00":
                                col = paletteCol[0];
                                col_mono = Color.Black;
                                break;
                            case "01":
                                col = paletteCol[1];
                                col_mono = new Color(80, 80, 80);
                                break;
                            case "10":
                                col = paletteCol[2];
                                col_mono = new Color(150, 150, 150);
                                break;
                            case "11":
                                col = paletteCol[3];
                                col_mono = new Color(255, 255, 255);
                                break;
                            default:
                                break;
                        }

                        if (cube_row == 8)
                        {
                            cube_row = 0;
                            cube_line++;
                        }

                        if (cube_line == 8)
                        {
                            cube_line = 0;
                            colum++;

                        }

                        if (colum == 16)
                        {
                            colum = 0;
                            if (line < 15)
                                line++;

                        }

                        int caonvertidx = (7 - cube_row) + 128 * (cube_line) + 8 * colum + 1024 * line;
                        imageData[caonvertidx] = col;
                        imageDataMono[caonvertidx] = col_mono;
                        cube_row++;
                        _index++;
                    }

                }

            }
            monotex.SetData<Color>(imageDataMono);
            chr_map.SetData<Color>(imageData);
        }

        /*public SpinningTriangleControl(System.Windows.Forms.PictureBox pb)
        {
            _pb = pb;
        }*/
        public void BitmapToTexture2D(System.Drawing.Bitmap image)
        {
            // Buffer size is size of color array multiplied by 4 because   
            // each pixel has four color bytes  
            int bufferSize = image.Height * image.Width * 4;

            // Create new memory stream and save image to stream so   
            // we don't have to save and read file  
            System.IO.MemoryStream memoryStream =
                new System.IO.MemoryStream(bufferSize);
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // Creates a texture from IO.Stream - our memory stream  
            chr_map = Texture2D.FromStream(
                GraphicsDevice, memoryStream);


        }
      

        int frameCount = 0;
        /// <summary>
        /// Initializes the control.
        /// </summary>
        /// 
        public List<byte> selected_metatile = new List<byte>();
        protected override void Initialize()
        {
            monotex = new RenderTarget2D(GraphicsDevice, 128, 128);
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("EMUFONT");
            Color[] imageData = new Color[8 * 8];
            for (int i = 0; i < imageData.Length; i++)
            {
                imageData[i] = Color.Blue;
            }
            for (int i = 0; i < grids.Capacity; i++)
            {
                Texture2D temp = new Texture2D(GraphicsDevice, 8, 8);
                grids.Add(temp);
                temp.SetData<Color>(imageData);
            }

            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Hook the idle event to constantly redraw our animation.
            System.Windows.Forms.Application.Idle += delegate { Invalidate(); }; 

        }
        public Point GetMousePosition()
        {
            System.Drawing.Point p = PointToClient(System.Windows.Forms.Control.MousePosition);
            return new Point(p.X, p.Y);
        }
        Point startPoint = new Point(-999, -999);
        private void update()
        {
            if (!ClientRectangle.Contains(PointToClient(System.Windows.Forms.Control.MousePosition)))
            {
                return;
            }
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (startPoint.X == -999)
                {
                    selectedRect = Rectangle.Empty;
                    startPoint.X = GetMousePosition().X;
                    startPoint.Y = GetMousePosition().Y;
                    selectedRect.Location = startPoint;
                }
                int wid = (GetMousePosition().X - startPoint.X);
                int hei = (GetMousePosition().Y - startPoint.Y);

                if (wid >= 0)
                    selectedRect.X = startPoint.X;
                else
                    selectedRect.X = GetMousePosition().X;


                if (hei >= 0)
                    selectedRect.Y = startPoint.Y;
                else
                    selectedRect.Y = GetMousePosition().Y;

                selectedRect.Width = Math.Abs(GetMousePosition().X - startPoint.X);
                selectedRect.Height = Math.Abs(GetMousePosition().Y - startPoint.Y);
            }
            else
            { 
                
                startPoint.X = -999;
            }
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        ///  
        private void DrawRectangle(Rectangle coords, Color color)
        {
            var rect = new Texture2D(GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });
            spriteBatch.Draw(rect, coords, color);
        }
        Rectangle formatted = new Rectangle();
        System.Drawing.Image tempImg = null;

        public RenderTarget2D ColoredClipedTex; 

        protected override void Draw()
        {
            update();
            RenderTarget2D clippedTex = null;
            if (formatted != Rectangle.Empty)
            {
                clippedTex = new RenderTarget2D(GraphicsDevice, formatted.Width, formatted.Height);
                ColoredClipedTex = new RenderTarget2D(GraphicsDevice, formatted.Width, formatted.Height);


                GraphicsDevice.SetRenderTarget(clippedTex);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                if (monotex != null)
                    spriteBatch.Draw(monotex, Vector2.Zero, new Rectangle(formatted.X / 2, formatted.Y / 2, formatted.Width / 2, formatted.Height / 2), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1);
                spriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);



                GraphicsDevice.SetRenderTarget(ColoredClipedTex);
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                if (chr_map != null)
                    spriteBatch.Draw(chr_map, Vector2.Zero, new Rectangle(formatted.X / 2, formatted.Y / 2, formatted.Width / 2, formatted.Height / 2), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1);
                spriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);


            }

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            if (chr_map != null)
                spriteBatch.Draw(chr_map, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1);


            int grid_line = 0;
            Point minPos = new Point(-1, -1);
            Point maxPos = new Point(-1, -1); 
            int lastIdx = -1;
            selected_metatile.Clear();
            for (int i = 0; i < grids.Count; i++)
            {
                Vector2 pos = new Vector2(i % 16 * 16, grid_line * 16);
                Rectangle gridRec = new Rectangle((int)pos.X, (int)pos.Y, (int)grids[i].Bounds.Width * 2, (int)grids[i].Bounds.Height * 2);

                if (selectedRect.Intersects(gridRec) || selectedRect.Contains(gridRec))
                {
                    if (minPos.X == -1)
                        minPos = gridRec.Location;
                    maxPos = gridRec.Location;
                    spriteBatch.Draw(grids[i], pos, null, new Color(255, 255, 255, 100), 0f, Vector2.Zero, 2f, SpriteEffects.None, 1);
                    selected_metatile.Add((byte)i);
                }


                if (i % 16 == 15)
                    grid_line++;
               // if (i == 19)
              //      spriteBatch.DrawString(font, "gridRec:" + gridRec.ToString(), new Vector2(1, 23), Color.White);
            }
            formatted.Location = minPos;
            formatted.Width = maxPos.X - minPos.X +16;
            formatted.Height = maxPos.Y - minPos.Y + 16;

                
              // GraphicsDevice.SetRenderTarget
           // if(clippedTex!=null)
           // spriteBatch.Draw(clippedTex, new Vector2(50, 50), Color.White);
           // DrawRectangle(formatted, Color.RoyalBlue);
           // spriteBatch.DrawString(font, " this.Bounds:" + this.Bounds.ToString(), new Vector2(0, 50), Color.White);
            spriteBatch.End();
            frameCount++;
            if (frameCount % 30 == 29)
            {
                if (clippedTex != null)
                {

                   _pb.Image =  TextureExtensions.TextureToPng(clippedTex, clippedTex.Width, clippedTex.Height); 
                }
            }
             

        }
    }
}
// spinningTriangleControl = new SpinningTriangleControl(pictureBox1);