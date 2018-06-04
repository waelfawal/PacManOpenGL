using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenglFirst
{
    public partial class MainForm : Form
    {
        public GlClass view;
        public MainForm()
        {
            InitializeComponent();

            this.view = new GlClass();
            this.view.Parent = this;
            this.view.Dock = DockStyle.Fill; // Will fill whole form

            this.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

    }
}
