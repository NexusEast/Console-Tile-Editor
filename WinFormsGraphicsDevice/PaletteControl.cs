#region File Description
//-----------------------------------------------------------------------------
// PaletteControl.cs
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
    class PaletteControl : GraphicsDeviceControl
    {

        ContentManager content;
        SpriteBatch spriteBatch; 
        int frameCount = 0;
        int size = 32;
        SpriteFont font;
        List<RenderTarget2D> col_rt = new List<RenderTarget2D>();
        Texture2D sel, sel_col;
        byte[] selected_pal = { 15, 0, 16, 48 };

        CHRViewControl chr_view;
        [Description("SetCHRView"),
      Category("Data"),
      DefaultValue(null),
      Browsable(true)]
        public CHRViewControl SetCHRView
        {
            get
            {
                return chr_view;
            }
            set
            {
                chr_view = value;
            }
        }

        public byte[,] bgPal={{15,0,16,48},{15,1,33,49},{15,6,22,38},{15,9,25,41}};
        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            for (int i = 0; i < 16; i++)
            {
                col_rt.Add(new RenderTarget2D(GraphicsDevice,size,size));
            }
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("EMUFONT");
            sel = content.Load<Texture2D>("sel");
            sel_col = content.Load<Texture2D>("sel_col");
 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Hook the idle event to constantly redraw our animation.
            System.Windows.Forms.Application.Idle += delegate { Invalidate(); }; 

        }
        public Point GetMousePosition()
        {
            System.Drawing.Point p = PointToClient(System.Windows.Forms.Control.MousePosition);
            return new Point(p.X, p.Y);
        } 
        private void update()
        {
            if (!ClientRectangle.Contains(PointToClient(System.Windows.Forms.Control.MousePosition)))
            {
                return;
            }
        }

        Vector2 sel_pos = new Vector2(0, 32);
        Vector2 offset = new Vector2(-6, -5);
        Vector2 offset_col = new Vector2(-5, -4);
        Vector2 sel_col_pos = new Vector2(0, 32);
        int selectedIdx = 0;

        public void updatePalette(byte col)
        {
            if (selectedIdx % 4 == 0)
            {
                bgPal[0, selectedIdx % 4] = col;
                bgPal[1, selectedIdx % 4] = col;
                bgPal[2, selectedIdx % 4] = col;
                bgPal[3, selectedIdx % 4] = col;
            }
            else
            bgPal[(int)selectedIdx / 4, selectedIdx % 4] = col;

          //  chr_view.updatePalette(selected_pal);
        }
        protected override void Draw()
        {
            frameCount++;
            update();
            //reset color


            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {

                    GraphicsDevice.SetRenderTarget(col_rt[i * 4 + j]);
                    GraphicsDevice.Clear(ColorControl.palette[bgPal[i,j]]);

                    GraphicsDevice.SetRenderTarget(null);
                }
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "PALETTE:", new Vector2(6,6), Color.Black);
            spriteBatch.DrawString(font, "PALETTE:", new Vector2(4, 4), Color.White);

            int line = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    int idx =i * 4 + j;
                    Rectangle rec = new Rectangle(idx % 8 * size, line * size + 32, size, size);
                    if (rec.Contains(GetMousePosition()) && Mouse.GetState().LeftButton== ButtonState.Pressed)
                    {
                        sel_pos = new Vector2(idx % 8 * size, line * size + 32);
                        selectedIdx = idx;
                        if (idx >= 0 && idx < 4)
                        {
                            sel_col_pos = new Vector2(0 % 8 * size, line * size + 32);
                        }
                        if (idx >= 4 && idx < 8)
                        {
                            sel_col_pos = new Vector2(4 % 8 * size, line * size + 32);
                        }
                        if (idx >= 8 && idx < 12)
                        {
                            sel_col_pos = new Vector2(8 % 8 * size, line * size + 32);
                        }
                        if (idx >= 12 && idx < 16)
                        {
                            sel_col_pos = new Vector2(12 % 8 * size, line * size + 32);
                        }

                       
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        selected_pal[k] = bgPal[selectedIdx / 4, k];
                    }
                     
                    spriteBatch.Draw(col_rt[idx],new Vector2(idx%8*size,line*size + 32),Color.White);
                    if(idx%8==7)
                        line++;
                }
          
            if (frameCount % 500 == 449)
            {
                chr_view.updatePalette(selected_pal);
            }
            spriteBatch.Draw(sel_col, Vector2.Add(offset_col, sel_col_pos), Color.White);
            spriteBatch.Draw(sel, Vector2.Add(offset, sel_pos), Color.White);

            spriteBatch.End();



        }
    }
}
// PaletteControl = new PaletteControl(pictureBox1);