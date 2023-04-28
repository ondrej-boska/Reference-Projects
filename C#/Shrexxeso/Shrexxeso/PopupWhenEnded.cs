using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shrexxeso
{
    public partial class PopupWhenEnded : Form
    {
        public PopupWhenEnded(string result)
        {
            InitializeComponent();
            Label_Result.Text = result;
            if(result == "Donkey Wins!") Label_Result.Location = new System.Drawing.Point(35, 10);
            else if(result == "You Win!") Label_Result.Location = new System.Drawing.Point(87, 10);
            else if (result == "Shrek Wins!") Label_Result.Location = new System.Drawing.Point(55, 10);
        }

        private void PopupWhenEnded_Load(object sender, EventArgs e)
        {

        }
    }
}
