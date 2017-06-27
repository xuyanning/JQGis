using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevComponents.DotNetBar;
using ModelInfo;
using CCWin;

namespace RailwayGIS
{
    public partial class LoginForm : Skin_Mac
    {
        //string loginFile = CGisDataSettings.gDataPath + @".usr";//GlobalSettings.gDataPath
        public LoginForm()
        {
            InitializeComponent();

            foreach (ProjectConfig pc in CGisDataSettings.gProjectList)
            {
                comboBoxEx1.Items.Add(pc.projectName);                
            }
            comboBoxEx1.SelectedIndex = 0;
            textBoxX1.Text = CGisDataSettings.gProjectList[comboBoxEx1.SelectedIndex].userName;   
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            CGisDataSettings.gCurrentProject.projectUrl = CGisDataSettings.gProjectList[comboBoxEx1.SelectedIndex].projectUrl;//ipAddressInput1.Value;
            CGisDataSettings.gCurrentProject.userName = textBoxX1.Text.Trim();
            CGisDataSettings.gCurrentProject.userPSD = textBoxX2.Text.Trim();
            CGisDataSettings.gCurrentProject.projectLocalPath = CGisDataSettings.gProjectList[comboBoxEx1.SelectedIndex].projectLocalPath;
            CServerWrapper.isConnected = !(checkBoxX1.CheckState == CheckState.Checked);
            if (CServerWrapper.isConnected)
                CServerWrapper.isConnected = CServerWrapper.ConnectToServer(CGisDataSettings.gCurrentProject.projectUrl);

            //string url = "http://" + GlobalSettings.gProjectUrl + "/webservice/usrlogin.asmx";

            string resultStr = null;
            if (CServerWrapper.isConnected)
            {
                resultStr = CServerWrapper.webLogin(CGisDataSettings.gCurrentProject.userName, CGisDataSettings.gCurrentProject.userPSD);
                if (resultStr == "登录成功")
                {
                    CGisDataSettings.UpdateConfigInfo();
                    this.DialogResult = DialogResult.OK; // Setting the DialogResult, will close the dialog, and the ShowDialog call will return.
                }
                else if (resultStr != null)
                {
                    MessageBox.Show("用户名或密码错误");
                    //Environment.Exit(-1);
                    //this.DialogResult 
                }
            }
            else
            {
                MessageBox.Show("脱机状态使用本系统");
                CGisDataSettings.UpdateConfigInfo();
                this.DialogResult = DialogResult.OK;
            }
            //this.btnOK.Enabled = true;
            //}           
            //GlobalVar.useLocalDB = true;
            //this.DialogResult = DialogResult.OK;

          
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}