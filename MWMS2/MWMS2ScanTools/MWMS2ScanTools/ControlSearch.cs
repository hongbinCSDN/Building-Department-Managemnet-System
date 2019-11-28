using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.IO;

namespace MWMS2ScanTools
{
    public partial class ControlSearch : UserControl
    {
        public ControlSearch()
        {
            InitializeComponent();
            listView2.MouseDoubleClick += ListView2_MouseDoubleClick;
        }

        private void ListView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {/*
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != myListView)
            {
                if (obj.GetType() == typeof(ListViewItem))
                {
                    // Do something here
                    MessageBox.Show("A ListViewItem was double clicked!");

                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            JObject jdata = Program.docServerKernal.SearchDoc(null, null, null, null, null);
            listView2.Items.Clear();
            foreach (JToken item in jdata["data"].Children())
            {
                JEnumerable<JProperty> itemProperties = item.Children<JProperty>();
                ListViewItem listViewItem = new ListViewItem();

                //List<JProperty> jPropertys = itemProperties.ToList();
                List<JProperty> jPropertys = new List<JProperty>();
                bool coled = false;
                foreach (JProperty c in itemProperties)
                {
                    jPropertys.Add(c);
                }
                if (!coled)
                {
                    coled = true;
                    listView2.Columns.Clear();
                    for (int i = 0; i < jPropertys.Count; i++)
                    {
                        listView2.Columns.Add(jPropertys[i].Name);
                    }
                }
                for (int i = 0; i < jPropertys.Count; i++)
                {
                    if (i == 0) listViewItem.Text = jPropertys[i].Value.ToString();
                    else listViewItem.SubItems.Add(jPropertys[i].Value.ToString());
                }
                listView2.Items.Add(listViewItem);
            }
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Uploaded Document Data");
                for (int i = 0; i < listView2.Columns.Count; i++)
                {
                    ws.Cells[1, i + 1].Value = listView2.Columns[i].Text;
                }
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    ws.Cells[i + 2, 1].Value = listView2.Items[i].Text;
                    for (int j = 0; j < listView2.Items[i].SubItems.Count; j++)
                    {
                        ws.Cells[i + 2, j + 2].Value = listView2.Items[i].SubItems[j].Text;
                    }
                }

                ws.Cells.AutoFitColumns();

                byte[] bin = package.GetAsByteArray();

                //create a SaveFileDialog instance with some properties
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Title = "Save Excel sheet";
                saveFileDialog1.Filter = "Excel files|*.xlsx|All files|*.*";
                saveFileDialog1.FileName = "ExcelSheet_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

                //check if user clicked the save button
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //write the file to the disk
                    File.WriteAllBytes(saveFileDialog1.FileName, bin);
                }

            }
        }
    }
}