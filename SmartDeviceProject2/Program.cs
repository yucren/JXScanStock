using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using System.Threading;


namespace JXScanStock
{
    static class Program
    {
        static string currentPath = "";
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [MTAThread]
        static void Main()
        {
            currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
            
            
                UpdateCheck();
         //   Application.Run(new ScanForm());
                
              
            
            
                    
           
        }
        static void  UpdateCheck()
        {
            try
            {


                if (File.Exists(currentPath + @"/JXScanStock.exe"))
                {

                    // MessageBox.Show("文件已存在");
                    Assembly assembly = Assembly.LoadFrom(currentPath + "/JXScanStock.exe");
                    var currentVersion = assembly.GetName().Version;

                    // MessageBox.Show(currentVersion);
                    WebRequest request = WebRequest.Create(@"http://10.10.13.44/stock/NewVersion.xml");
                    request.Timeout = 3000;
                    var newVersion = "";
                    try
                    {
                        

                        WebResponse response = request.GetResponse();
                        Stream resStream = response.GetResponseStream();
                        //var newVersionString = "";
                        //using (StreamReader sr = new StreamReader(resStream, Encoding.UTF8))
                        //{
                        //    newVersionString = sr.ReadToEnd();
                        //}

                       
                        using (XmlReader xr = XmlReader.Create(resStream))
                        {
                            while (!xr.EOF)
                            {
                                if (xr.MoveToContent() == XmlNodeType.Element && xr.Name == "version")
                                {
                                    newVersion = xr.ReadElementString();
                                    //   MessageBox.Show(currentVersion);
                                    //  MessageBox.Show(newVersion);
                                    break;

                                }

                            }
                        }

                    }
                    catch (WebException ex )
                    {

                        if (MessageBox.Show( "无法连接到更新服务器\n确认:打开原程序,取消升级\n取消:退出程序", "继续提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                        {

                            Application.Run(new ScanForm());
                            return;
                        }
                        else
                        {

                            Application.Exit();
                            return;
                            
                        }
                        
                    }
                   


                    if (!string.IsNullOrEmpty(newVersion) && currentVersion.CompareTo(new Version(newVersion)) == -1)
                    {
                        if (MessageBox.Show("有新版本,是否更新", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                        {
                            Process pr = new Process();
                            ProcessStartInfo sinfo = new ProcessStartInfo()
                            {
                                FileName = currentPath + "/Update.exe",
                                WorkingDirectory = currentPath,
                                UseShellExecute = false

                            };
                            pr.StartInfo = sinfo;
                            pr.StartInfo.Arguments = Process.GetCurrentProcess().Id.ToString();
                            Process pp = Process.GetCurrentProcess();
                            pr.Start();
                            pr.Close();
                            pp.Kill();
                            //  pr.Close();




                        }
                        else
                        {
                            MessageBox.Show("不更新会产生预想不到的问题,请知悉");
                            Application.Run(new ScanForm());
                            return;
                        }


                    }
                    else
                    {
                        Application.Run(new ScanForm());
                        return;
                    }




                }
                else
                {

                    Application.Run(new ScanForm());
                    return;


                }




            }
            catch (WebException ex)
            {

                MessageBox.Show("请连接网络后,再重新打开" );
                Application.Exit();
                return;
            }
            catch (Exception ex1)
            {

                MessageBox.Show(ex1.Message,"错误");
                Application.Exit();
                return;

            }

        }

      
    }
}