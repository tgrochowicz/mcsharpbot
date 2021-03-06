﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using mcsharpbot.bots;
using System.IO.IsolatedStorage;
using System.IO;

namespace mcsharpbot.ui
{
    public partial class MCSharpBotManager : Form
    {
        public MCSharpBotManager()
        {
            InitializeComponent();
            LoadSettings();
            txtUserName.DataBindings.Add("Text", _settings, "Username");
            txtPassword.DataBindings.Add("Text", _settings, "Password");
            txtServerAddress.DataBindings.Add("Text", _settings, "ServerName");
            txtServerPort.DataBindings.Add("Text", _settings, "Port");
            checkBox1.DataBindings.Add("Checked", _settings, "Auth");
        }

        private void LoadSettings()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (isoStore.FileExists(settings))
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(MCSharpBotSettings));
                IsolatedStorageFileStream fs = new IsolatedStorageFileStream(settings, FileMode.Open, isoStore);
                StreamReader stream = new StreamReader(fs);
                _settings = (MCSharpBotSettings)ser.Deserialize(stream);
                stream.Close();
                isoStore.DeleteFile(settings);
                cbRemember.Checked = true;
            }
            else
            {
                _settings = new MCSharpBotSettings();
                _settings.Port = "25565";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;

            bot.BotFeedbackReceived += new MCBotBase.BotFeedbackEventHandler(bot_BotFeedbackReceived);
            bot.Start(txtUserName.Text, txtPassword.Text, txtServerAddress.Text, int.Parse(txtServerPort.Text), checkBox1.Checked);
        }

        void bot_BotFeedbackReceived(object sender, BotFeedbackEventArgs args)
        {
            this.Invoke((MethodInvoker)delegate { lbFeedback.Items.Add(args.Message); });
            
        }
        private bots.MCBotBase bot;

        private void button2_Click(object sender, EventArgs e)
        {
            bot.Stop();

            btnStop.Enabled = false;
            btnStart.Enabled = true;
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            groupBox4.Enabled = true;
        }

        private void MCSharpBotManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bot != null && bot.Running)
                bot.Stop();
            if (cbRemember.Checked)
                SaveSettings();
        }

        private void MCSharpBotManager_Load(object sender, EventArgs e)
        {
            var botTypes = AvailableBots();
            cmbBotTypes.DataSource = botTypes;
        }

        //Load Bot Types
        private List<MCBotBase> AvailableBots()
        {

            List<MCBotBase> returnable = new List<MCBotBase>();

            returnable.Add(new SimpleBot());
            returnable.Add(new MoveBot());
            returnable.Add(new FollowBot());

            return returnable;
        }

        private void cmbBotTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            bot = (MCBotBase)cmbBotTypes.SelectedValue;

            //Clear previous properties
            tlpBotSettings.Controls.Clear();

            //Populate Properties
            var y = from x in bot.GetType().GetProperties()
                    select x;

            int rows = 0;
            foreach (var prop in y)
            {
                object[] attrib = prop.GetCustomAttributes(false);
                if (attrib.Length > 0)
                {
                    UserEditableAttribute edit = (UserEditableAttribute)attrib[0];
                    Label l = new Label { Text = edit.Name };
                    l.Dock = DockStyle.Fill;
                    tlpBotSettings.Controls.Add(l, 0, rows);
                    if (prop.PropertyType.Name == "Boolean")
                    {
                        CheckBox cb = new CheckBox();
                        cb.Checked = (bool)prop.GetValue(bot, null);
                        tlpBotSettings.Controls.Add(cb, 1, rows);
                    }
                    else if (prop.PropertyType.Name == "String")
                    {
                        TextBox tb = new TextBox();
                        tb.Text = (string)prop.GetValue(bot, null);
                        tlpBotSettings.Controls.Add(tb, 1, rows);
                    }
                    else if (prop.PropertyType.BaseType.Name == "Enum")
                    {
                        ComboBox cmb = new ComboBox();
                        foreach (var x in Enum.GetValues(prop.PropertyType))
                            cmb.Items.Add(x);
                        tlpBotSettings.Controls.Add(cmb, 1, rows);
                    }
                    rows++;
                }
            }

        }
        string settings = "botsettings.xml";
        public void SaveSettings()
        {
            try
            {
                IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                IsolatedStorageFileStream stream = new IsolatedStorageFileStream(settings, System.IO.FileMode.OpenOrCreate, store);
                StreamWriter writer = new StreamWriter(stream);
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(_settings.GetType());
                ser.Serialize(writer, _settings);
            }
            catch { }
        }
        private MCSharpBotSettings _settings;
    }
    public class MCSharpBotSettings
    {
        public string Username
        { get; set; }
        public string Password
        { get; set; }
        public string ServerName
        { get; set; }
        public string Port
        { get; set; }
        public bool Auth
        {
            get;
            set;
        }
    }

}
