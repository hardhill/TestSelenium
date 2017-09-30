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
            //

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text;
            (sender as Button).Enabled = false;
            mainBot = new Bot("https://yandex.ru");
            mainBot.OnLoadpage += MainBot_OnLoadpage;
            mainBot.OnError += MainBot_OnError;
            mainBot.OnStopWork += MainBot_OnStopWork;
            mainBot.Start();
            //using (Browser = new ChromeDriver())
            //{
            //    Browser.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(1);
            //    Browser.Navigate().GoToUrl("https://yandex.ru");
            //    IWebElement quirystring = Browser.FindElement(By.Id("text"));
            //    quirystring.SendKeys("giant");
            //    quirystring.Submit();
            //    var wait = new WebDriverWait(Browser, TimeSpan.FromMinutes(1)).Until(driver => driver.FindElements(By.TagName("a")));
            //    List<IWebElement> elements = Browser.FindElements(By.TagName("a")).ToList();
            //    foreach (IWebElement el in elements)
            //    {
            //        text = el.GetAttribute("href").ToString();
            //        lbURLs.Items.Add(text);
            //    }
            //    //do
            //    //{
            //    //    string newurl = lbURLs.Items[0].ToString();
            //    //    Browser.Navigate().GoToUrl(newurl);
            //    //    elements = Browser.FindElements(By.TagName("a")).ToList();
            //    //    foreach (IWebElement el in elements)
            //    //    {
            //    //        text = el.GetAttribute("href").ToString();
            //    //        if(lbURLs.Items.Count<64000)
            //    //            lbURLs.Items.Add(text);
            //    //    }
            //    //    lbURLs.Items.RemoveAt(0);
            //    //} while (lbURLs.Items.Count > 0);
            //}
        }

        private void MainBot_OnStopWork()
        {
            Action action = () =>
            {
                Console.Out.WriteLine("Working break");
               
            };
           Invoke(action);
           
        }

        private void MainBot_OnError(Exception e)
        {
            // обработка ошибки действия страницы
        }

        private void MainBot_OnLoadpage(List<string> lstLinks)
        {
                Action action = () => { lbURLs.Items.AddRange(lstLinks.ToArray()); };
                Invoke(action);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mainBot != null)
                mainBot.StopWorking();
        }
    }
}
