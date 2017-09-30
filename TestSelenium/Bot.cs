using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSelenium
{
    
    public delegate void LoadPage(IWebDriver browser);
    
    public class Bot
    {
        private string _url;
        private Thread mainThread;
        private IWebDriver Browser;
        public event LoadPage OnLoadpage;
        //constructor
        public Bot(string url)
        {
            this._url = url;
            Browser = new ChromeDriver();
            if (Browser != null)
            {
                Browser.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(4);
            }

        }

        public void Start()
        {
            mainThread = new Thread(new ThreadStart(LoadPage));
            mainThread.Start();
        }

        public void Stop()
        {
            mainThread.Abort();
        }

        private void LoadPage()
        {
            if (Browser != null)
            {
                Browser.Navigate().GoToUrl(_url);
                var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(360));
                wait.Until(driver => driver.FindElements(By.TagName("div")).Any(x=>x.Displayed));
                OnLoadpage(Browser);
                
            }
        }

        public void KillBot()
        {
            Stop();
            Browser.Close();
            Browser.Dispose();
        }

        
    }
}
