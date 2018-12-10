using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Update
{
    class Program
    {
        static string currentPath = "";
        static void Main(string[] args)
        {
            
            currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
          //  MessageBox.Show(currentPath);
            GetApp(Convert.ToInt32(args[0]));
            
            
        }
        static void GetApp(int processID)
        {
          //  MessageBox.Show("更新开始");
            WebRequest request = WebRequest.Create(@"http://10.10.13.44/stock/JXScanStock.exe");
            WebResponse response = request.GetResponse();
            Stream resStream = response.GetResponseStream();
            int bytelength = (int)response.ContentLength;
            Byte[] bytevalue = new Byte[bytelength];
            int num = 0;
            int m;

            while (1 > 0)
            {
                m = resStream.Read(bytevalue, num, bytelength);
                if (m == 0) break;
                num += m;           //当前已下载的字节数  
                bytelength -= m;    //剩余字节数   总字节数：(int)response.ContentLength                              
            }
            resStream.Close();
            //写入文件
            string path = currentPath + "/JXScanStock.exe";
            try

            {
                try 
            {	         Process MainProcess = Process.GetProcessById(processID);
            if (MainProcess.StartInfo.FileName.Contains("JXScanStock.exe"))
            {
                MainProcess.WaitForExit();
            }
                         
                         

            }
            catch (Exception)
            {
        		
                
            }
                       
               

                FileStream fs = new FileStream(path, FileMode.Create);

                //将byte数组写入文件中
                fs.Write(bytevalue, 0, bytevalue.Length);
                fs.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show("发生错误:" + ex.Message);
            }



            Process pr = new Process();
            ProcessStartInfo sinfo = new ProcessStartInfo()
            {
                FileName = currentPath+ "/JXScanStock.exe",
                WorkingDirectory = currentPath,
                UseShellExecute = false

            };
           
            pr.StartInfo = sinfo;
            MessageBox.Show("更新完成");
            pr.Start();

        }
        
    }
}
