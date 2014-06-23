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
    class MapEditControl : GraphicsDeviceControl
    {

        ContentManager content;
        SpriteBatch spriteBatch;
        int size = 16;
        public static float scale = 1;
        int frameCount = 0;
        SpriteFont font;
        List<Texture2D> col_rt = new List<Texture2D>();
        byte[,] map_data = null;
        float[,] map_bgCol = null;
        CHRViewControl chr_view;
        Texture2D cell = null;
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

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            map_data = new byte[MapSize.map_size[0], MapSize.map_size[1]];
            map_bgCol = new float[MapSize.map_size[0], MapSize.map_size[1]];

            for (int i = 0; i < MapSize.map_size[0]; i++)
                for (int j = 0; j < MapSize.map_size[1]; j++)
            {
                map_bgCol[i, j] = 0.1f;
            }
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("EMUFONT");
            cell = content.Load<Texture2D>("cell");
            

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
                if (this.Focused)
                   System.Windows.Forms.SendKeys.Send("{TAB}");
                return;
            }
            this.Focus();
        }

        float alpha = 0.1f;
        protected override void Draw()
        {
            update();
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
            Vector2 pos = new Vector2(0, 0);
            for (int i = 0; i < MapSize.map_size[0]; i++)
                for (int j = 0; j < MapSize.map_size[1]; j++)
            {

                pos.X = j * 16 * scale;
                pos.Y = i * 16 * scale;
                int sizee = (int)Math.Round( size * scale);
                Rectangle rec = new Rectangle((int)pos.X, (int)pos.Y, sizee, sizee);
                     if (rec.Contains(GetMousePosition()))
                     {
                         map_bgCol[i, j] += 0.1f;
                         if (map_bgCol[i, j] >= 1f)
                             map_bgCol[i, j] = 1f;
                     }
                map_bgCol[i, j] -= 0.01f;
                if (map_bgCol[i, j] <= 0.1f)
                    map_bgCol[i, j] = 0.1f;
                spriteBatch.Draw(cell, pos, null, Color.White * map_bgCol[i, j], 0f, Vector2.Zero, new Vector2(scale,scale), SpriteEffects.None, 0f);
            }

            spriteBatch.End();



        }
    }
}
// PaletteControl = new PaletteControl(pictureBox1);