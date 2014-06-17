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
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System;
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
    class SpinningTriangleControl : GraphicsDeviceControl
    {

        Texture2D chr_map = null;
        SpriteBatch spriteBatch;

        public void bytetotex(byte[] b)
        {
            chr_map = new Texture2D(GraphicsDevice, 128, 128);
            Color[] imageData = new Color[128 * 128];
            //split into 1 


            string output = "";
            int _index = 0; 
            for (int i = 0; i <256; i++)
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
                        if (pat_bit[k+8]) bitstr += "1";
                        else bitstr += "0";
                         
                        Color col = new Color();
                        switch (bitstr)
                        {
                            case "00":
                                col = Color.Black;
                                break;
                            case "01":
                                col = new Color(80, 80, 80);
                                break;
                            case "10":
                                col = new Color(150, 150, 150);
                                break;
                            case "11":
                                col = new Color(255, 255, 255);
                                break;
                            default:
                                break;
                        }

                     //  output += high_bit[k].ToString();

                        imageData[_index] = col;
                        _index++;
                    }
                    
                }
                /*
                byte[] fullbyte = new byte[2];
                fullbyte[0] = b[i];
                fullbyte[1] = b[i+8];
                BitArray patt_bit = new BitArray(b[i]);
                BitArray col_bit = new BitArray(b[i+1]);
                ++i;
                for (int j = 0; j < patt_bit.Length; i++)
                {


                    imageData[_index] = new Color(b[i], b[i], b[i]);
                    ++_index;
                }*/
                

            }

            System.Diagnostics.Debug.WriteLine("debug:" + output);
            chr_map.SetData<Color>(imageData);
        }

        public  void BitmapToTexture2D( System.Drawing.Bitmap image)
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



        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice); 
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
            Texture2D t2d = new Texture2D(GraphicsDevice, 128, 128); 
        
        }



        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (chr_map!=null)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatch.Draw(chr_map, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1);
                spriteBatch.End(); 
            }

             
        }
    }
}
