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
    public delegate void Error(Exception e);
    
    public class Bot
    {
        private string _url;
        private Thread mainThread;
        private IWebDriver Browser;
        private StatusBot _statusbot;

        public event LoadPage OnLoadpage;
        public event Error OnError;
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
        public StatusBot GetStatus()
        {
            return _statusbot;
        }
        public void Start()
        {
            mainThread = new Thread(new ThreadStart(ScanPages));
            mainThread.Start();
        }

        public void Stop()
        {
            mainThread.Abort();
        }

        private void ScanPages()
        {
            if (Browser != null)
            {
                _statusbot = StatusBot.Work;
                try
                {
                    Browser.Navigate().GoToUrl(_url);
                    var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(360));
                    wait.Until(driver => driver.FindElements(By.TagName("div")).Any(x => x.Displayed));
                    if(OnLoadpage!=null)
                        OnLoadpage(Browser);
                }catch(Exception e)
                {
                    _statusbot = StatusBot.Stop;
                    if(OnError!=null)
                        OnError(e);
                }
                //статус бота
                _statusbot = StatusBot.Pause;
                // загрузка прошла
                
            }
        }

        public void KillBot()
        {
            Stop();
            Browser.Close();
            Browser.Dispose();
        }

        
    }

    public enum StatusBot
    {
        Pause=0,Work=1,
        Stop = 2
    }
}
