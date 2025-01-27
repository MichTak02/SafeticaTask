using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SafeticaTask.Actions;

public class GenericActions
{
    public WebDriver WebDriver { get; }
    public WebDriverWait Wait { get; }

    public GenericActions(WebDriver webDriver, WebDriverWait wait)
    {
        WebDriver = webDriver;
        Wait = wait;
    }
    
    public void FillField(By by, string text)
    {
        IWebElement element = WaitForDisplayed(by);
        element.SendKeys(text);
    }
    
    public void ClickButton(By by, bool mustBeDisplayed = true)
    {
        IWebElement button = mustBeDisplayed ? WaitForDisplayed(by) : Wait.Until(driver => driver.FindElement(by));
        button.Click();
    }
    
    public IWebElement WaitForDisplayed(By by)
    {
        return Wait.Until(driver =>
        {
            var element = driver.FindElement(by);
            return element.Displayed ? element : null;
        });
    }

    public ReadOnlyCollection<IWebElement> GetMultipleElements(By by, int number)
    {
        return GetMultipleElementsRange(by, number, number);
    }

    public ReadOnlyCollection<IWebElement> GetMultipleElementsRange(By by, int min, int max)
    {
        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(by);
            return elements.Count >= min && elements.Count <= max ? elements : null;
        });
    }
}