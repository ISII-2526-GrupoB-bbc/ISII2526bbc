using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase
{
    public class CreatePurchase_PO : PageObject
    {
        // ===== LOCATORS =====
        private By nameBy = By.Id("Name");
        private By surnameBy = By.Id("Surname");
        private By addressBy = By.Id("Address");
        private By paymentMethodBy = By.Id("PaymentMethod");
        private By submitButtonBy = By.Id("Submit");
        private By modifyCarsButtonBy = By.Id("ModifyCars");
        private By errorsShownBy = By.Id("ErrorsShown");
       // private By submitButtonBy = By.Id("Submit");
       // private By validationSummaryBy = By.ClassName("validation-summary-errors");


        public CreatePurchase_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        // ===== ACTIONS =====

        public void FillInPurchaseInfo(string name, string surname, string address, string paymentMethod)
        {
            WaitForBeingVisible(nameBy);

            _driver.FindElement(nameBy).Clear();
            _driver.FindElement(nameBy).SendKeys(name);

            _driver.FindElement(surnameBy).Clear();
            _driver.FindElement(surnameBy).SendKeys(surname);

            _driver.FindElement(addressBy).Clear();
            _driver.FindElement(addressBy).SendKeys(address);

            if (!string.IsNullOrEmpty(paymentMethod))
            {
                var select = new SelectElement(_driver.FindElement(paymentMethodBy));
                select.SelectByText(paymentMethod);
            }
        }

        public void PressSubmit()
        {
            WaitForBeingClickable(submitButtonBy);
            _driver.FindElement(submitButtonBy).Click();
        }


        public void PressModifyCars()
        {
            _driver.FindElement(modifyCarsButtonBy).Click();
        }

        // ===== VERIFICATIONS =====




        public bool CheckAnyValidationError(string expectedError)
        {
            // Espera hasta que el texto aparezca en algún sitio de la página
            WaitForCondition(d => d.PageSource.Contains(expectedError), 10);
            return _driver.PageSource.Contains(expectedError);
        }

        // Helper (si no tienes uno parecido)
        private void WaitForCondition(Func<IWebDriver, bool> condition, int seconds)
        {
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
            wait.Until(condition);
        }




        public bool CheckListOfPurchaseItems(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, By.Id("TableOfPurchaseItems"));
        }
    }
}
