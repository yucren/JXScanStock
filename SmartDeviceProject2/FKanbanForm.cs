using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace JXScanStock
{
    public partial class FKanbanForm : Form
    {
        
        public FKanbanForm()
        {
            InitializeComponent();
        }

        private void FKanbanForm_Closed(object sender, EventArgs e)
        {
            Dispose();
            GC.Collect();
            ScanForm.mainForm.Show();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FKanbanForm_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection sqlconn = new SqlConnection("server=192.168.50.68;database=LonKing_MES_JX;uid=sa;pwd=;"))
                {
                    DataTable fkanbantable = new DataTable();
                    SqlDataAdapter fkanbansdp = new SqlDataAdapter();
                    //   SqlParameter itemlistParam = new SqlParameter("itemlist", SqlDbType.NVarChar, 200);
                    StringBuilder strbuilder = new StringBuilder();
                    strbuilder.Append("SELECT  lenkanban.fbillno ,case lenkanban.fstatus  when 2 then '禁用' when 0  then '可用' when 1 then '可用' else  lenkanban.fstatuName end as kanbanstatus ,lksc.fCreatedBy,lksc.fCreatedOn,case lksc.fBusType  when 1 then '工序扫描'  when -1 then '领料' when 0 then '盘点' end as 'fbusType' ");
                    strbuilder.Append("FROM LonKing_MES_JX.dbo.lkm_exec_kanban_entry lenkanban ");
                    strbuilder.Append("left join  dbo.LKM_KanbanScheduling lksc on lenkanban.fCurScanNo = lksc.fBillNo ");
                    strbuilder.Append("where lenkanban.fbasic_billno ='" + ScanForm.mainForm.mainKanbanNo.Split('-')[0]  +"' ");   
               
                    SqlCommand smd = new SqlCommand(strbuilder.ToString(), sqlconn);
                    //  itemlistParam.Value = ScanForm.mainForm.kanbanItemList;
                    //  smd.Parameters.Add(itemlistParam);
                    sqlconn.Open();
                    fkanbansdp.SelectCommand = smd;
                  //  fkanbantable.Clear();
                    fkanbansdp.Fill(fkanbantable);
                    dataGrid1.TableStyles.Clear();
                    DataGridTableStyle dt = new DataGridTableStyle();
                    dt.MappingName = fkanbantable.Namespace;
                    dataGrid1.TableStyles.Add(dt);
                    dataGrid1.TableStyles[0].GridColumnStyles.Clear();
                    DataGridTextBoxColumn fExecKBNoColumn = new DataGridTextBoxColumn();
                    fExecKBNoColumn.Width = 60;
                    fExecKBNoColumn.HeaderText = "执行看板号";
                    fExecKBNoColumn.MappingName = fkanbantable.Columns[0].ColumnName; ;
                    dataGrid1.TableStyles[0].GridColumnStyles.Add(fExecKBNoColumn);
                    //DataGridTextBoxColumn itemcodeColumn = new DataGridTextBoxColumn();
                    //itemcodeColumn.Width = 85;
                    //itemcodeColumn.HeaderText = "料号";
                    //itemcodeColumn.MappingName = "fItemCode";
                    //dataGrid1.TableStyles[0].GridColumnStyles.Add(itemcodeColumn);
                    DataGridTextBoxColumn fStatuNameColumn = new DataGridTextBoxColumn();
                    fStatuNameColumn.Width = 60;
                    fStatuNameColumn.HeaderText = "看板状态";
                    fStatuNameColumn.MappingName = fkanbantable.Columns[1].ColumnName;
                    dataGrid1.TableStyles[0].GridColumnStyles.Add(fStatuNameColumn);

                    DataGridTextBoxColumn fCreatedByColumn = new DataGridTextBoxColumn();
                    fCreatedByColumn.Width = 60;
                    fCreatedByColumn.HeaderText = "最后操作人员";
                    fCreatedByColumn.MappingName = fkanbantable.Columns[2].ColumnName;
                    dataGrid1.TableStyles[0].GridColumnStyles.Add(fCreatedByColumn);
                    DataGridTextBoxColumn lastOpTimeColumn = new DataGridTextBoxColumn();  
                    lastOpTimeColumn.Width = 85;
                    lastOpTimeColumn.HeaderText = "最后操作时间";
                    lastOpTimeColumn.MappingName = fkanbantable.Columns[3].ColumnName;
                    dataGrid1.TableStyles[0].GridColumnStyles.Add(lastOpTimeColumn);
                    DataGridTextBoxColumn fbusTypeColumn = new DataGridTextBoxColumn();
                    fbusTypeColumn.Width = 60;
                    fbusTypeColumn.HeaderText = "最后操作类型";
                    fbusTypeColumn.MappingName = fkanbantable.Columns[4].ColumnName;
                    dataGrid1.TableStyles[0].GridColumnStyles.Add(fbusTypeColumn);
                     

                    dataGrid1.DataSource = fkanbantable;
                    // ScanForm.mainForm.kanbanItemList = "";
                }
            }
            catch (Exception ex)
            {

                errmsg.Text = ex.Message;
                MessageBox.Show(ex.Message);
            }
            
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void errmsg_ParentChanged(object sender, EventArgs e)
        {

        }
    }
}
