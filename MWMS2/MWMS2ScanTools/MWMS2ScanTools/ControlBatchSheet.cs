using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MWMS2ScanTools
{
    public partial class ControlBatchSheet : UserControl
    {
        string _name;
        List<List<object>> _data;
        TabControl _tabControl;
        TabPage _tabPage;
        public ControlBatchSheet(TabControl tabControl, TabPage tabPage, string name, List<List<object>> data)
        {
            _name = name;
            _data = data;
            _tabControl = tabControl;
            _tabPage = tabPage;
            InitializeComponent();
           // listView1.Items.Clear();
           if(data.Count > 0)
            {
                for(int i = 0;i < data[0].Count; i++)
                {
                    listView1.Columns.Add(data[0][i] == null ? "": _data[0][i].ToString());
                }
            }


            for(int i= 1;i < _data.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                for (int j = 0; j< _data[i].Count; j++)
                {
                    if (j == 0) item.Text = _data[i][j] == null ?  "" : _data[i][j].ToString();
                    else item.SubItems.Add(_data[i][j] == null ? "" : _data[i][j].ToString());
                }
                listView1.Items.Add(item);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _tabControl.TabPages.Remove(_tabPage);
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            _tabControl.TabPages.Remove(_tabPage);
        }
    }
}
