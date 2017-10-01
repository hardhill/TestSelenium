using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestSelenium
{
    

    public partial class Form1 : Form
    {
        private Bot mainBot;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lbURLs.Items.Clear();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           if(mainBot.GetStatus() == StatusBot.Stop)
            {
                mainBot.Start();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            (sender as Button).Enabled = false;
            mainBot = new Bot("https://yandex.ru/");
            mainBot.OnLoadpage += MainBot_OnLoadpage;
            mainBot.OnError += MainBot_OnError;
            mainBot.OnStopWork += MainBot_OnStopWork;
            timer1.Enabled = true;
        }

        private void MainBot_OnStopWork()
        {
            Action action = () =>
            {
                Console.Out.WriteLine("Working break");
               
            };
           Invoke(action);
           
        }

        private void MainBot_OnError(string err)
        {
            Console.Out.WriteLine(err);
        }

        private void MainBot_OnLoadpage(List<string> lstLinks)
        {
                Action action = () => {
                    lbURLs.Items.Clear();
                    lbURLs.Items.AddRange(lstLinks.ToArray());
                    stbText.Text = String.Format("Всего ссылок:{0}", lstLinks.Count);
                };
                Invoke(action);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mainBot != null)
            {
                mainBot.StopWorking();
                timer1.Enabled = false;
                mainBot.KillBot();
                mainBot = null;
                button1.Enabled = true;
            }
        }
    }
}
