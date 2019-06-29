using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections;

namespace DupeList
{
    public partial class ResultWindow : Form
    {
        ContextMenu cm = new ContextMenu();

        public ResultWindow(Hashtable duplicateResults)
        {
            InitializeComponent();

            new Thread((ThreadStart)delegate()
            {
                BeginProcessing(duplicateResults);
            }).Start();
        }

        private void BeginProcessing(Hashtable duplicateResults)
        {
            DoInvoke((MethodInvoker)delegate()
            {
                duplicateList.BeginUpdate();
            });

            int max = 0;
            foreach (DictionaryEntry ss in duplicateResults)
            {
                max += ((List<string>)ss.Value).Count;
            }

            ProcessProgress progress = new ProcessProgress(max, "Processing");
            DoInvoke((MethodInvoker)delegate()
            {
                progress.Show(this);
            });

            int count = 0;
            int total = 0;

            foreach (DictionaryEntry dupeGroup in duplicateResults)
            {
                List<string> dupeGroupFiles = (List<string>)dupeGroup.Value;

                string headerKey = "Hasg: " + dupeGroup.Key.ToString() + ", Size: " + new FileInfo(dupeGroupFiles[0]).Length.ToString();

                DoInvoke((MethodInvoker)delegate ()
                {
                    duplicateList.Groups.Add(headerKey, headerKey);
                });

                foreach (string file in dupeGroupFiles)
                {
                    ListViewItem i = new ListViewItem(duplicateList.Groups[headerKey]);
                    i.Text = file;

                    DoInvoke((MethodInvoker)delegate()
                    {
                        duplicateList.Items.Add(i);
                    });

                    count++;
                    total++;
                    if (count > 50)
                    {
                        count = 0;
                        progress.UpdateProgress(total);
                    }
                    Application.DoEvents();
                }
            }
            DoInvoke((MethodInvoker)delegate()
            {
                progress.Close();
            });

            DoInvoke((MethodInvoker)delegate()
            {
                duplicateList.EndUpdate();
            });
        }

        private void DoInvoke(MethodInvoker method)
        {
            if (duplicateList.InvokeRequired)
            {
                Invoke(method);
            }
            else
            {
                method();
            }
        }

        private void duplicateList_Resize(object sender, EventArgs e)
        {
            duplicateList.Columns[0].Width = duplicateList.Width - 10;
        }

        private void ResultWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void bSelAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in duplicateList.Items)
            {
                i.Checked = true;
            }
        }

        private void b_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in duplicateList.Items)
            {
                i.Checked = false;
            }
        }

        private void bSelInverse_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in duplicateList.Items)
            {
                i.Checked = !i.Checked;
            }
        }

        private void bSelLowerPriority_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in duplicateList.Items)
            {
                i.Checked = ExcludeFiles.IsLowerPriority(i.Text); 
            }

            foreach (ListViewGroup g in duplicateList.Groups)
            {
                if(g.Items.OfType<ListViewItem>().All(x => !x.Checked))
                {
                    for(int index  = 0; index < g.Items.Count; index ++)
                    {
                        (g.Items[index] as ListViewItem).Checked = index != 0;
                    }
                }
            }
        }

        private void bDelSelected_Click(object sender, EventArgs e)
        {
            new Thread((ThreadStart)delegate()
            {
                _deleteFiles();
            }).Start();
        }

        private void _deleteFiles()
        {
            if (MessageBox.Show("WARNING: This will permanently delete all checked files in this list. This action cannot be undone.\r\n\r\nAre you sure you want to delete the selected files?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                ProcessProgress progress = new ProcessProgress(duplicateList.Items.Count, "Deleting");
                DoInvoke((MethodInvoker)delegate()
                {
                    progress.Show(this);
                });

                List<ListViewItem> toRemove = new List<ListViewItem>();

                int total = 0;

                DoInvoke((MethodInvoker)delegate()
                {
                    foreach (ListViewItem i in duplicateList.Items)
                    {
                        if (i.Checked)
                        {
                            try
                            {
                                FileInfo f = new FileInfo(i.Text);
                                f.Delete();
                                toRemove.Add(i);
                            }
                            catch
                            {
                                // Delete failed. Skip the file.
                            }
                        }
                        total++;
                        progress.UpdateProgress(total);
                    }
                });

                DoInvoke((MethodInvoker)delegate()
                {
                    progress.Close();
                });

                ProcessProgress cleanup = new ProcessProgress(toRemove.Count, "Cleaning up");
                int count = 0;
                DoInvoke((MethodInvoker)delegate()
                {
                    cleanup.Show(this);

                    // Remove deleted files
                    duplicateList.BeginUpdate();
                    foreach (ListViewItem i in toRemove)
                    {
                        duplicateList.Items.Remove(i);
                        cleanup.UpdateProgress(count++);
                    }
                    duplicateList.EndUpdate();
                    cleanup.Close();
                });

                if (toRemove.Count != count)
                {
                    MessageBox.Show("Some files could not be deleted. These items were not removed from the list.", "Partial File Deletion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("All selected files have been deleted.", "All Files Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
