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
    using System.Drawing;
    using System.IO;
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
        public MainForm()
        {
            InitializeComponent();

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


      
    }
}
