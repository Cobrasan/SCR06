using System;
using System.Configuration.Install;
using System.IO;
using System.Security.AccessControl;
using Microsoft.Win32;
using System.Collections;

namespace CustomAction
{
    [System.ComponentModel.RunInstaller(true)]
    public class CustomInstaller : Installer
    {
        // インストール時の動作
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            stateSaver.Add("TargetDir", Context.Parameters["DP_TargetDir"].ToString());
            stateSaver.Add("ProductID", Context.Parameters["DP_ProductID"].ToString());
        }

        // 確定時の動作
        public override void Commit(System.Collections.IDictionary savedState)
        {
            // Alchemistディレクトリを取得する
            string dirName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "JAM\\Alchemist");

            var dirInfo = new DirectoryInfo(dirName);
            var dirSecurity = dirInfo.GetAccessControl();

            var rule = new FileSystemAccessRule("Users", FileSystemRights.CreateFiles | 
                                                         FileSystemRights.Read | 
                                                         FileSystemRights.Write | 
                                                         FileSystemRights.Modify |
                                                         FileSystemRights.Delete |
                                                         FileSystemRights.ExecuteFile | 
                                                         FileSystemRights.ListDirectory,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, 
                PropagationFlags.InheritOnly, AccessControlType.Allow);
            
            dirSecurity.AddAccessRule(rule);
            dirInfo.SetAccessControl(dirSecurity);

            // InstallLocationを追加する
            string productId = savedState["ProductID"].ToString();
            RegistryKey applicationRegistry = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + productId, true);
            if (applicationRegistry != null)
            {
                applicationRegistry.SetValue("InstallLocation", savedState["TargetDir"].ToString());
                applicationRegistry.Close();
            }

            base.Commit(savedState);
        }
    }
}
