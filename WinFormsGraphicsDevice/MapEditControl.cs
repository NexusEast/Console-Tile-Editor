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
using System.Runtime.InteropServices;
using System.Reflection;
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
        static byte[,] map_data = null;
        static float[,] map_bgCol = null;
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
        protected override void Update(float dt)
        {

        }
        /// <summary>
        /// Initializes the control.
        /// </summary>
        /// 
        public static System.Windows.Forms.Cursor LoadCustomCursor(string path)
        {
            IntPtr hCurs = LoadCursorFromFile(path);
            if (hCurs == IntPtr.Zero) throw new Win32Exception();
            var curs = new System.Windows.Forms.Cursor(hCurs);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof(System.Windows.Forms.Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(curs, true);
            return curs;
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursorFromFile(string path);

        System.Windows.Forms.Cursor drag;
        System.Windows.Forms.Cursor cursor_drag;

        protected override void Initialize()
        {

            drag = LoadCustomCursor(@"Content\cursor.cur");
            cursor_drag = LoadCustomCursor(@"Content\cursor_drag.cur");

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
            MainForm.lastMouseScroll++;
            if (!MainForm.isStopped && MainForm.lastMouseScroll > 2)
            {
                MainForm.isStopped = true;
            }
            if (!ClientRectangle.Contains(PointToClient(System.Windows.Forms.Control.MousePosition)))
            {
               // if (this.Focused)
                //   System.Windows.Forms.SendKeys.Send("{TAB}");
                return;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                this.Focus();
            }
            if (!MainForm.isStopped)
            {

                scale += MainForm.wheel_value*0.02f;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                this.Cursor = drag;

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    this.Cursor = cursor_drag;
                    panel_position.X += GetMousePosition().X - mouse_last.X;
                    panel_position.Y += GetMousePosition().Y - mouse_last.Y;
                }
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            mouse_last = GetMousePosition();
           // if(this.Parent.Focused)
             //   this.Focus();
        }

        static public void updateSize(int w, int h)
        {
            byte[,] tempByte = new byte[w, h];
            float[,] tempCol = new float[w, h];
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
            {
                if (i < MapSize.map_size[0] && j < MapSize.map_size[1])
                {
                    tempByte[i, j] = map_data[i, j];
                    tempCol[i, j] = map_bgCol[i, j];
                }
                else
                {
                    tempByte[i, j] = 0x00;
                    tempCol[i, j] = .1f;

                }
            }
            MapSize.map_size[0] = w;
            MapSize.map_size[1] = h;
            map_data = tempByte;
            map_bgCol = tempCol;

            //copy existing value 
        }

        float alpha = 0.1f;
        Vector2 panel_position = new Vector2(0, 0);
        Point mouse_last = new Point(0, 0);
        protected override void Draw()
        {
           
            update();
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
            Vector2 pos = new Vector2(0, 0);
            for (int i = 0; i < MapSize.map_size[0]; i++)
                for (int j = 0; j < MapSize.map_size[1]; j++)
            {

                pos.X = i * 16 * scale;
                pos.Y = j * 16 * scale;
                int sizee = (int)Math.Round( size * scale);
                Rectangle rec = new Rectangle((int)pos.X + (int)panel_position.X, (int)pos.Y + (int)panel_position.Y, sizee, sizee); 
                     if (rec.Contains(GetMousePosition()))
                     {
                         map_bgCol[i, j] += 0.1f;
                         if (map_bgCol[i, j] >= 1f)
                             map_bgCol[i, j] = 1f;
                     }
                map_bgCol[i, j] -= 0.01f;
                if (map_bgCol[i, j] <= 0.1f)
                    map_bgCol[i, j] = 0.1f;
                spriteBatch.Draw(cell, pos + panel_position, null, Color.White * map_bgCol[i, j], 0f, Vector2.Zero, new Vector2(scale, scale), SpriteEffects.None, 0f);

            }

            spriteBatch.DrawString(font, "DEBUG size : " + size, new Vector2(10, this.Size.Height-10), Color.White);
            spriteBatch.End();



        }
    }
}
// PaletteControl = new PaletteControl(pictureBox1);