#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Windows.Forms;
#endregion

namespace WinFormsGraphicsDevice
{
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    // System.Drawing and the XNA Framework both define Color types.
    // To avoid conflicts, we define shortcut names for them both.
    using GdiColor = System.Drawing.Color;
    using XnaColor = Microsoft.Xna.Framework.Color;

    
    /// <summary>
    /// Custom form provides the main user interface for the program.
    /// In this sample we used the designer to add a splitter pane to the form,
    /// which contains a SpriteFontControl and a SpinningTriangleControl.
    /// </summary>
    public partial class MainForm : Form
    {
        [System.Runtime.InteropServices.DllImport("user32", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hwnd, int wBar, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool bShow);
        int SB_HORZ = 0;
        int SB_VERT = 1;
        int SB_BOTH = 3;
        private void HideHorizontalScrollBar()
        {
            ShowScrollBar((IntPtr)listView1.Handle.ToInt64(),
                SB_VERT
                ,false);
        }
        public MainForm()
        {
            InitializeComponent();
            HideHorizontalScrollBar();
        }
        public static byte[] ConvertFileToByteArray(string fileName)
        {

            byte[] returnValue = null;



            using (FileStream fr = new FileStream(fileName, FileMode.Open))
            {

                using (BinaryReader br = new BinaryReader(fr))
                {

                    returnValue = br.ReadBytes((int)fr.Length);

                }

            }



            return returnValue;



        }
        public static Bitmap byteArrayToImage(byte[] byteArrayIn)
            {
                ImageConverter ic = new ImageConverter();
                Image img = (Image)ic.ConvertFrom(byteArrayIn);
                return new Bitmap(img); 
               /* if (byteArrayIn == null)
                    return null;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
                {
                    System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                    ms.Flush();
                    return returnImage;
                }

            */
            }
        private string SelectTextFile(string initialDirectory)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "CHR files (*.CHR)|*.CHR|All files (*.*)|*.*";
            dialog.InitialDirectory = initialDirectory;
            dialog.Title = "Select a text file";
            return (dialog.ShowDialog() == DialogResult.OK)
               ? dialog.FileName : null;
        }
        public byte[] CHR_BUFFER = null; 
        private void cHRAsTileSetToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            string file_path = SelectTextFile("\\");
            if (file_path != null)
            {
                CHR_BUFFER = ConvertFileToByteArray(file_path);
                spinningTriangleControl.bytetotex(CHR_BUFFER);

                
            }
        }

        private void spinningTriangleControl_Click(object sender, System.EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            
        }

        int total_image_idx = 0;
        static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode =
                    InterpolationMode.NearestNeighbor;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
             
            imageList1.Images.Add(FixedSize(pictureBox1.Image, 32, 32));
            ListViewItem item = new ListViewItem();
            item.ImageIndex = total_image_idx;
            this.listView1.Items.Add(item);
            total_image_idx++;

        }


      
    }
}
