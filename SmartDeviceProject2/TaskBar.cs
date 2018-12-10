using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JXScanStock
{
  


/// <summary>
/// 隐藏/显示WINCE任务栏
/// </summary>
public class TaskBar
{
[DllImport("coredll.dll", EntryPoint = "FindWindow")]
public static extern int FindWindow(string lpWindowName, string lpClassName);
[DllImport("coredll.dll", EntryPoint = "ShowWindow")]
public static extern int ShowWindow(int hwnd, int nCmdShow);
public const int SW_SHOW = 5; //显示窗口常量
public const int SW_HIDE = 0; //隐藏窗口常量
public bool HideTaskBar()
{
int Hwnd = FindWindow("HHTaskBar", null);
if (Hwnd == 0) return false;
else
{
ShowWindow(Hwnd, SW_HIDE);
}
return true;
}
public bool ShowTaskBar()
{
int Hwnd = FindWindow("HHTaskBar", null);
if (Hwnd == 0) return false;
else
{
ShowWindow(Hwnd, SW_SHOW);


}
return true;
}
}
}

  

