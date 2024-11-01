using KedoBull;
using PowerArgs;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

const int InteractionPauseDurationMs = 500;

ArgumentsData arguments = Args.Parse<ArgumentsData>();

Console.WriteLine("Download has been started...");

using var driver = new ChromeDriver();
driver.Manage().Window.Maximize();
driver.Navigate().GoToUrl(arguments.Url);
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

{
    var btnLogin = driver.FindElement(By.XPath("//*[@id=\"root\"]/div/div/div[1]/div[1]/a[1]"));
    Thread.Sleep(InteractionPauseDurationMs);
    btnLogin.Click();
}

driver.FindElement(By.XPath("//*[@id=\"email\"]")).SendKeys(arguments.Login);
Thread.Sleep(InteractionPauseDurationMs);
driver.FindElement(By.XPath("//*[@id=\"password\"]")).SendKeys(arguments.Password.ConvertToNonsecureString());
Thread.Sleep(InteractionPauseDurationMs);
driver.FindElement(By.XPath("//*[@id=\"root\"]/div/div/div/span/form/div[3]/div[2]/span/button")).Click();

HashSet<string> processedDocumentIds = new HashSet<string>();

var links = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div[1]/div[2]/ul"))
    .FindElements(By.CssSelector("li > a:nth-child(1)"));

int pageIndex = 0;
var pageLink = GetPageLink();
int pageLoopCount = 100;
while (pageLink != null)
{
    pageLink.Click();
    Thread.Sleep(InteractionPauseDurationMs);

    int loopCount = 1000;
    IWebElement? link = GetDocumentLink();
    while (link != null)
    {
        string nextLinkId = GetLinkId(link);

        Thread.Sleep(InteractionPauseDurationMs);
        link.Click();
        Thread.Sleep(InteractionPauseDurationMs);
        driver.FindElement(By.XPath("//*[@id=\"content-container\"]/div/div[1]/div/div[3]/div/div[1]/span/span/button")).Click();
        Thread.Sleep(InteractionPauseDurationMs);
        driver.FindElement(By.XPath("//*[@data-tid=\"Select__menu\"]/div/div/div/div/button[1]")).Click();
        Thread.Sleep(InteractionPauseDurationMs);

        processedDocumentIds.Add(nextLinkId);
        driver.Navigate().Back();

        link = GetDocumentLink();
        loopCount--;
        if (loopCount == 0)
            throw new Exception("Iteratinon limit reached for documents.");
    }

    pageIndex++;
    pageLink = GetPageLink();
    pageLoopCount--;
    if (pageLoopCount == 0)
        throw new Exception("Iteratinon limit reached for pages.");
}

Console.WriteLine("Download complete.");

IWebElement? GetDocumentLink() => 
    driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div[1]/div[2]/ul"))
        .FindElements(By.CssSelector("li > a:nth-child(1)"))
        .FirstOrDefault(link => !processedDocumentIds.Contains(GetLinkId(link)));

string GetLinkId(IWebElement link) =>
    link.FindElement(By.XPath("./..")).GetAttribute("id");

IWebElement? GetPageLink() =>
    driver.FindElements(By.XPath("//*[@data-tid=\"Paging__root\"]/*[@data-tid=\"Paging__pageLinkWrapper\"]"))
        .ElementAtOrDefault(pageIndex);