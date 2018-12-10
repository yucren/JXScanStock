using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace JXScanStock
{
    public partial class ScanForm : Form
    {
        internal static ScanForm mainForm;
        internal string mainKanbanNo = "";
       // TaskBar tb = new TaskBar();
        public ScanForm()
        {
            InitializeComponent();
            
            GetStockData(false);
            errMessage.Text = "请扫描看板二维码";
            kanbanNo.Focus();
            kanbanNo.LostFocus += new EventHandler(kanbanNo_LostFocus);
            mainForm =this;
            
            
            

           
        }

        void kanbanNo_LostFocus(object sender, EventArgs e)
        {




         //   GetStockData();
      

        }
        


        DataTable dt = new DataTable();
        SqlDataAdapter sdp = new SqlDataAdapter();
       
       

        private void kanbanNo_TextChanged(object sender, EventArgs e)
        {

            //if (kanbanNo.Text.Length == 0)
            //{
            //    GetStockData(false);
            //    errMessage.Text = "请扫描看板二维码";
            //}

        }            
               
                


           

          

               
        




        

      

        private void button1_Click(object sender, EventArgs e)
        {
            kanbanNo.Text = "";
            GetStockData(false);
            errMessage.Text = "请扫描看板二维码";
            
            kanbanNo.Focus();

        }

        private void GetStockData(bool isAll)
        {

            try
            {
                using (SqlConnection sqlconn = new SqlConnection("server=192.168.50.68;database=LonKing_MES_JX;uid=sa;pwd=LonkingMES;"))
                {


                    

                       if (!string.IsNullOrEmpty(kanbanNo.Text) || isAll)
                        {
                            sqlconn.Open();
                            StringBuilder strbuild = new StringBuilder();
                            strbuild.Append("select master.fItemCode,master.fItemName,master.fModel,lc.fName, ISNULL(inv.fcount,0) fcount from lkm_exec_kanban_entry lkexek inner join ");
                            strbuild.Append("lkm_kanban lkanban on lkexek.fbasic_billno =lkanban.fbillno INNER join lkm_Materials master on lkanban.fitemid =master.fInterID ");
                            strbuild.Append("inner join LKM_MCCItemEntry lke on master.fItemID =lke.fitemid inner join LKM_MCCPTEntry lmentry on lmentry.fConfigID =lke.fConfigID ");
                            strbuild.Append("inner join lkm_CommonBill lc on lc.fInterID =lmentry.fProTecID left join lkm_blank_inventory inv on inv.fitemid =lkanban.fitemid ");
                            if (isAll)
                            {
                                strbuild.Append("and inv.fproid = lmentry.fProTecID  and lmentry.fProTecID != 56 ");

                            }
                            else
                            {

                                strbuild.Append("and inv.fproid = lmentry.fProTecID where lkexek.fbillno ='" + kanbanNo.Text + "' and lmentry.fProTecID != 56 ");

                            }
                            SqlCommand scanStoceCmd = new SqlCommand(strbuild.ToString(), sqlconn);
                            sdp.SelectCommand = scanStoceCmd;
                            dt.Clear();
                            sdp.Fill(dt);
                          //  MessageBox.Show(dt.Rows.Count.ToString());

                            var dtformatValue = from kanbanNo2 in dt.AsEnumerable()
                                                select new KanbanInv
                                                {
                                                    工序 = kanbanNo2["fName"].ToString(),
                                                    数量 = Convert.ToDecimal(kanbanNo2["fcount"]),
                                                    料号 = kanbanNo2["fItemCode"].ToString(),
                                                    品名 = kanbanNo2["fItemName"].ToString().Split('~')[0],
                                                    型号 = kanbanNo2["fModel"].ToString(),
                                                };

                            dataGrid1.TableStyles.Clear();
                            DataGridTableStyle dts = new DataGridTableStyle();

                            //  注意：必须加上这一句，否则自定义列格式无法使用
                            dts.MappingName = "KanbanInv[]";
                            dataGrid1.TableStyles.Add(dts);
                            dataGrid1.TableStyles[0].GridColumnStyles.Clear();
                            //========================设置表头栏位===========================
                            DataGridTableStyle dtsLog = new DataGridTableStyle();
                            DataGridTextBoxColumn colID = new DataGridTextBoxColumn();
                            colID.Width = 70;
                            colID.HeaderText = "工序";
                            colID.MappingName = "工序";
                            dataGrid1.TableStyles[0].GridColumnStyles.Add(colID);

                            DataGridTextBoxColumn colLog = new DataGridTextBoxColumn();
                            colLog.Width = 40;
                            colLog.HeaderText = "数量";
                            colLog.MappingName = "数量";
                            dataGrid1.TableStyles[0].GridColumnStyles.Add(colLog);

                            DataGridTextBoxColumn colTime = new DataGridTextBoxColumn();
                            colTime.Width = 85;
                            colTime.HeaderText = "料号";
                            colTime.MappingName = "料号";
                            dataGrid1.TableStyles[0].GridColumnStyles.Add(colTime);

                            DataGridTextBoxColumn itemName = new DataGridTextBoxColumn();
                            itemName.Width = 60;
                            itemName.HeaderText = "品名";
                            itemName.MappingName = "品名";
                            dataGrid1.TableStyles[0].GridColumnStyles.Add(itemName);

                            DataGridTextBoxColumn colCatalog = new DataGridTextBoxColumn();
                            colCatalog.Width = 150;
                            colCatalog.HeaderText = "型号";
                            colCatalog.MappingName = "型号";
                            dataGrid1.TableStyles[0].GridColumnStyles.Add(colCatalog);
                            dataGrid1.DataSource = dtformatValue.ToArray();
                            if (dtformatValue.Count() == 0)
                            {
                                errMessage.Text = "没有找到数据";
                            }
                            else
                            {
                                errMessage.Text = "查找成功";
                            }


                        }
                        else
                        {
                            errMessage.Text = "没有找到数据";
                            dt.Clear();
                            var dtformatValue = from kanbanNo2 in dt.AsEnumerable()
                                                select new
                                                {
                                                    工序 = kanbanNo2["fName"],
                                                    数量 = kanbanNo2["fcount"],
                                                    料号 = kanbanNo2["fItemCode"],
                                                    品名 = kanbanNo2["fItemName"].ToString().Split('~')[0],
                                                    型号 = kanbanNo2["fModel"],
                                                };



                            dataGrid1.DataSource = dtformatValue.ToArray();
                            //  tb.ShowTaskBar();



                        }

                    }

                }
            
            catch (Exception ex)
            {

                errMessage.Text = "发生错误:" + ex.Message;
                //  tb.ShowTaskBar();
            }
        }
        
        




        

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
         
        }

        private void errMessage_ParentChanged(object sender, EventArgs e)
        {
            
        }

        private void ScanForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void ScanForm_KeyDown(object sender, KeyEventArgs e)
        {
          //  MessageBox.Show(e.KeyCode.ToString());

            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                if (kanbanNo.Text =="")
                {
                    kanbanNo.Focus();
                    return;
                }
                GetStockData(false);
                mainKanbanNo = kanbanNo.Text;
                kanbanNo.Text = "";


            }

        }

        private void ScanForm_Load(object sender, EventArgs e)
        {

        }

     

        private void kanbanNo_GotFocus(object sender, EventArgs e)
        {
            if (kanbanNo.Text=="")
            {
                errMessage.Text = "请扫描看板二维码";
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
          

    
    }

        private void statusBth_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(mainKanbanNo))
            {
                FKanbanForm fkf = new FKanbanForm();


                fkf.Show();
                this.Hide();
            }
            else
            {
                errMessage.Text = "请先查找看板数据";
            }
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            // var path = @"/Program Files" + @"/startup/MES_SHJX_PD.exe";
            Process pr = new Process();
            ProcessStartInfo sinfo = new ProcessStartInfo()
            {
                FileName = @"Program Files" + @"/startapp/" + "MES_SHJX_PD.exe",
                WorkingDirectory = @"Program Files" + @"/startapp/",
                UseShellExecute = false

            };
            pr.StartInfo = sinfo;

            pr.Start();
        
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {  //  var path = @"/Program Files" + @"/startup/MES_SHJX_PD.exe";
            Process pr = new Process();
            ProcessStartInfo sinfo = new ProcessStartInfo()
            {
                FileName = @"Program Files" + @"/mes/" + "LonKingSHJX_正式.exe",
                WorkingDirectory = @"Program Files" + @"/startapp/",
                UseShellExecute = false

            };
            pr.StartInfo = sinfo;

            pr.Start();
            applciathi
           
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
           MessageBox.Show("当前版本:" +Assembly.GetExecutingAssembly().GetName().Version.ToString());

        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            GetStockData(true);
            kanbanNo.Text = "";
            kanbanNo.Focus();
        }
        }





























}