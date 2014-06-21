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
        int size = 16;
        int frameCount = 0;
        SpriteFont font;
        List<Texture2D> col_rt = new List<Texture2D>();
        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("EMUFONT");

               for (int i   = 0; i < palette.Length; i++)
               {
                   Texture2D tex = new Texture2D(GraphicsDevice, size, size);
                   Color[] data = new Color[size * size];
                   for (int j = 0; j < data.Length; j++)
			        {
			            data[j] = palette[i];
			        }
                   tex.SetData<Color>(data);
                   col_rt.Add(tex);
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
        Color selectedColor = new Color();
        private void update()
        {
            if (!ClientRectangle.Contains(PointToClient(System.Windows.Forms.Control.MousePosition)))
            {
                return;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                selectedColor = palette[chosenIndex];
            }
        }
        Color[] palette = { new Color(124,124,124),
new Color(0,0,252),
new Color(0,0,188),
new Color(68,40,188),
new Color(148,0,132),
new Color(168,0,32),
new Color(168,16,0),
new Color(136,20,0),
new Color(80,48,0),
new Color(0,120,0),
new Color(0,104,0),
new Color(0,88,0),
new Color(0,64,88),
new Color(0,0,0),
new Color(0,0,0),
new Color(0,0,0),
new Color(188,188,188),
new Color(0,120,248),
new Color(0,88,248),
new Color(104,68,252),
new Color(216,0,204),
new Color(228,0,88),
new Color(248,56,0),
new Color(228,92,16),
new Color(172,124,0),
new Color(0,184,0),
new Color(0,168,0),
new Color(0,168,68),
new Color(0,136,136),
new Color(0,0,0),
new Color(0,0,0),
new Color(0,0,0),
new Color(248,248,248),
new Color(60,188,252),
new Color(104,136,252),
new Color(152,120,248),
new Color(248,120,248),
new Color(248,88,152),
new Color(248,120,88),
new Color(252,160,68),
new Color(248,184,0),
new Color(184,248,24),
new Color(88,216,84),
new Color(88,248,152),
new Color(0,232,216),
new Color(120,120,120),
new Color(0,0,0),
new Color(0,0,0),
new Color(252,252,252),
new Color(164,228,252),
new Color(184,184,248),
new Color(216,184,248),
new Color(248,184,248),
new Color(248,164,192),
new Color(240,208,176),
new Color(252,224,168),
new Color(248,216,120),
new Color(216,248,120),
new Color(184,248,184),
new Color(184,248,216),
new Color(0,252,252),
new Color(248,216,248),
new Color(0,0,0),
new Color(0,0,0),
    };

        int baba = 16;
        int chosenIndex = 0;
        protected override void Draw()
        { 
            update();
            GraphicsDevice.Clear(selectedColor);
            spriteBatch.Begin();
            int lines = 0;
            for (int i = 0; i < col_rt.Count; i++)
            {
                Rectangle rec = new Rectangle(i % baba * size, lines * size, size, size);
                if(rec.Contains(GetMousePosition())) 
                    chosenIndex =i;

                spriteBatch.Draw(col_rt[i], new Vector2(i % baba * size, lines * size), Color.White);
                if (i % baba == baba-1) lines++;
            }
            spriteBatch.DrawString(font, "SELECTED COLOR:", new Vector2(0, 80), Color.Black);
            spriteBatch.DrawString(font, "SELECTED COLOR:", new Vector2(1, 81), Color.White);
            spriteBatch.End();



        }
    }
}
// PaletteControl = new PaletteControl(pictureBox1);