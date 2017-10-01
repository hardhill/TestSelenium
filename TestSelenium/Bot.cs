using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSelenium
{

    public delegate void LoadPage(List<string> lstLinks);
    public delegate void Error(string err);
    public delegate void StopWork();

    public class Bot
    {
        private string _url;
        private string subLink;
        private Thread mainThread;
        private IWebDriver Browser;
        List<IWebElement> elements;
        private StatusBot _statusbot;
        private List<string> listMainBuffer;
        private List<string> listVisitedLinks;



        public event LoadPage OnLoadpage;
        public event Error OnError;
        public event StopWork OnStopWork;
        //constructor
        public Bot(string url)
        {
            listMainBuffer = new List<string>();
            listVisitedLinks = new List<string>();
            this._url = url;
            _statusbot = StatusBot.Stop;
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

                    elements = Browser.FindElements(By.TagName("a")).ToList();
                    foreach (IWebElement element in elements)
                    {
                        string text = element.GetAttribute("href").ToString();
                        listMainBuffer.Add(text);

                    }
                    //корневая страница вся загружена
                    if (OnLoadpage != null)
                        OnLoadpage(listMainBuffer);
                }
                catch (Exception e)
                {
                    _statusbot = StatusBot.Stop;
                    if (OnError != null)
                        OnError(String.Format("Ошибка обработки главной страницы {0}", e.Message));
                }

                //----------------------------------------------------------
                Console.Out.WriteLine("Working....");
                //работа по стэку ссылок
                while ((listMainBuffer.Count > 0) && (_statusbot == StatusBot.Work))
                {
                    string curLink = listMainBuffer[0].ToString();
                    try
                    {
                        if (PutVisitedLink(curLink))
                        {
                            //переходим по ссылке которую сохранили в словаре ссылок
                            try
                            {
                                Browser.Navigate().GoToUrl(curLink);
                                var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(360));
                                if (wait.Until(driver => driver.FindElements(By.TagName("a")).Any(x => x.Displayed)))
                                {
                                    //elements = Browser.FindElements(By.TagName("a")).ToList();
                                    elements = Browser.FindElements(By.CssSelector("a[href]")).ToList();
                                    Console.Out.WriteLine(String.Format("По корневой ссылке {0} нашли еще {1} ссылок", curLink, elements.Count));
                                    foreach (IWebElement el in elements)
                                    {
                                        if (el.Displayed)   //если ссылка реально на странице
                                        {
                                            //Console.Out.WriteLine(String.Format("элемент: {0}", el.Text));
                                            subLink = el.GetAttribute("href").ToString();
                                            //добавить в конец списка корневых ссылок если нет в словаре ссылок
                                            if (PutVisitedLink(subLink))
                                            {
                                                listMainBuffer.Add(subLink);
                                            }
                                        }
                                    }
                                }
                                // ищем все ссылки на странице



                            }
                            catch (Exception e)
                            {
                                _statusbot = StatusBot.Stop;
                                if (OnError != null)
                                    OnError(String.Format("Ошибка обработки основного списка по ссылке {0}", e.Message));
                            }
                        }
                    }
                    finally
                    {
                        listMainBuffer.RemoveAt(0);
                        OnLoadpage(listMainBuffer);
                    }

                };
                try
                {
                    SaveToFile(listVisitedLinks);
                }
                catch (Exception e)
                {
                    OnError(e.Message);
                }


                Console.Out.WriteLine("Bot work complite");

            }
        }

        private void SaveToFile(List<string> listVisitedLinks)
        {
            TextWriter tw = new StreamWriter("Urls.txt");
            foreach (String line in listVisitedLinks)
            {
                tw.WriteLine(line);
            }
            tw.Close();
        }

        private bool PutVisitedLink(string curLink)
        {
            if (!listVisitedLinks.Exists(x => x.Contains(curLink)))
            {
                listVisitedLinks.Add(curLink);
                return true;
            }
            else
                return false;
        }

        public void KillBot()
        {
            mainThread.Abort();
            Browser.Close();
            Browser.Dispose();
        }

        public StatusBot GetStatus()
        {
            return _statusbot;
        }

    }




    public enum StatusBot
    {
        Pause = 0,
        Work = 1,
        Stop = 2
    }
}
