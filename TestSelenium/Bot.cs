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
    
    public delegate void LoadPage(List<string> lstLinks );
    public delegate void Error(Exception e);
    public delegate void StopWork();
    
    public class Bot
    {
        private string _url;
        private Thread mainThread;
        private IWebDriver Browser;
        private StatusBot _statusbot;
        private List<string> listMainBuffer;

        public event LoadPage OnLoadpage;
        public event Error OnError;
        public event StopWork OnStopWork;
        //constructor
        public Bot(string url)
        {
            listMainBuffer = new List<string>();
            this._url = url;
            Browser = new ChromeDriver();
            if (Browser != null)
            {
                Browser.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(4);
            }

        }
        
        public void Start()
        {
            mainThread = new Thread(new ThreadStart(ScanPages));
            mainThread.Start();
        }

        public void StopWorking()
        {
            _statusbot = StatusBot.Stop;
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

                    List<IWebElement> elements = Browser.FindElements(By.TagName("a")).ToList();
                    foreach (IWebElement element in elements)
                    {
                        string text = element.GetAttribute("href").ToString();
                        listMainBuffer.Add(text);
                     
                    }
                    //страница вся загружена
                    if (OnLoadpage!=null)
                        OnLoadpage(listMainBuffer);
                }catch(Exception e)
                {
                    _statusbot = StatusBot.Stop;
                    if(OnError!=null)
                        OnError(e);
                }
                //статус бота
                while (_statusbot==StatusBot.Work)
                {
                    Console.Out.WriteLine("Working....");
                    Thread.Sleep(500);
                    if (_statusbot == StatusBot.Stop)
                    {
                        if (OnStopWork != null)
                            OnStopWork();
                        break;
                    }
                    
                }
               
                
                Console.Out.WriteLine("Bot work complite");
               
            }
        }

        public void KillBot()
        {
            mainThread.Abort();
            Browser.Close();
            Browser.Dispose();
        }
        
        
    }

    internal enum StatusBot
    {
        Pause=0,
        Work =1,
        Stop = 2
    }
}
