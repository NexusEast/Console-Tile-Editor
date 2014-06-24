using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormsGraphicsDevice
{
    public partial class MapSize : Form
    {
        public MapSize()
        {
            InitializeComponent();
        }
        static public int[] map_size= {16,15}; 
        private void MapSize_Load(object sender, EventArgs e)
        {
 

            numericUpDown1.Value = map_size[0];
            numericUpDown2.Value = map_size[1];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MapEditControl.updateSize((int)numericUpDown1.Value, (int)numericUpDown2.Value); 
            this.Dispose();
        }
    }
}
