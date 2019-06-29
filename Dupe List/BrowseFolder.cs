using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DupeList
{
    public partial class BrowseFolder : Form
    {
        string FolderLoc = "";

        public BrowseFolder()
        {
            InitializeComponent();
        }

        private void bBrowse_Click(object sender, EventArgs e)
        {
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                FolderLoc = FBD.SelectedPath;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            tPath.Text = FolderLoc;
            if (!bScan.Enabled)
            {
                bScan.Enabled = true;
            }
        }

        private void bScan_Click(object sender, EventArgs e)
        {
            this.Hide();
            new Progress(FolderLoc).Show();
        }

        private void BrowseFolder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void tPath_Enter(object sender, EventArgs e)
        {
            if (tPath.Text == "No folder selected.")
            {
                tPath.Text = "";
            }
        }

        private void tPath_Leave(object sender, EventArgs e)
        {
            if (tPath.Text == "")
            {
                tPath.Text = "No folder selected.";
            }
        }

        private void tPath_TextChanged(object sender, EventArgs e)
        {
            tPath.ForeColor = tPath.Text == "No folder selected." ? SystemColors.GrayText : SystemColors.ControlText;
            bScan.Enabled = tPath.Text != "No folder selected." && tPath.Text != "";
            FolderLoc = tPath.Text;
        }
    }
}
