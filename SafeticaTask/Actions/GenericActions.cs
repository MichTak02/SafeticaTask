using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SafeticaTask.Actions;

/// <summary>
/// Class for utility methods to perform common actions on web pages using Selenium
/// </summary>
public class GenericActions
{
    public WebDriverWait Wait { get; }

    public GenericActions(WebDriverWait wait)
    {
        Wait = wait;
    }
    
    /// <summary>
    /// Waits for input field to load and fills in a text
    /// </summary>
    /// <param name="by">input field identifier</param>
    /// <param name="text">text to input</param>
    public void FillField(By by, string text)
    {
        IWebElement element = WaitForDisplayed(by);
        element.SendKeys(text);
    }
    
    /// <summary>
    /// Waits for button / clickable element to load and clicks it
    /// </summary>
    /// <param name="by">button identifier</param>
    /// <param name="mustBeDisplayed">true if button must be displayed to click it</param>
    public void ClickButton(By by, bool mustBeDisplayed = true)
    {
        IWebElement button = mustBeDisplayed ? WaitForDisplayed(by) : Wait.Until(driver => driver.FindElement(by));
        button.Click();
    }
    
    /// <summary>
    /// Waits for element to load and be displayed
    /// </summary>
    /// <param name="by">element identifier</param>
    /// <returns></returns>
    public IWebElement WaitForDisplayed(By by)
    {
        return Wait.Until(driver =>
        {
            var element = driver.FindElement(by);
            return element.Displayed ? element : null;
        });
    }

    /// <summary>
    /// Waits for exact number of elements
    /// </summary>
    /// <param name="by">elements identifier</param>
    /// <param name="number">number of elements to get</param>
    /// <returns></returns>
    public ReadOnlyCollection<IWebElement> GetMultipleElements(By by, int number)
    {
        return GetMultipleElementsRange(by, number, number);
    }

    /// <summary>
    /// Waits for elements in range
    /// </summary>
    /// <param name="by">elements identifier</param>
    /// <param name="min">minimum elements to load</param>
    /// <param name="max">maximum elements to load or -1 for no upper bound</param>
    /// <returns></returns>
    public ReadOnlyCollection<IWebElement> GetMultipleElementsRange(By by, int min, int max)
    {
        if (max == -1)
        {
            max = int.MaxValue;
        }
        
        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(by);
            bool inRange = elements.Count >= min && elements.Count <= max;
            bool areDisplayed = elements.All(element => element.Displayed);
            return inRange && areDisplayed ? elements : null;
        });
    }
}