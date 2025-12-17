using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppForSEII2526.UIT.UC_Rental
{
    public class CreateRental_PO : PageObject
    {
        private By _nameBy = By.Id("Name");
        private By _surnameBy = By.Id("Surname");
        private By _deliveryDealerBy = By.Id("DCDealer");
        private By _paymentMethodBy = By.Id("PaymentMethod");
        private By _userNameBy = By.Id("UserName");

        private IWebElement _userName() => _driver.FindElement(_userNameBy);



        public CreateRental_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FillInRentalInfo(string username, string name, string surname, string deliveryDealer, string paymentMethod)
        {
            // Esperamos a que el formulario esté visible
            WaitForBeingVisible(_nameBy);

            // Name
            _driver.FindElement(_nameBy).Clear();
            _driver.FindElement(_nameBy).SendKeys(name);

            // Username
            _driver.FindElement(_userNameBy).Clear();
            _driver.FindElement(_userNameBy).SendKeys(username);

            // Surname
            _driver.FindElement(_surnameBy).Clear();
            _driver.FindElement(_surnameBy).SendKeys(surname);

            // Delivery Car Dealer
            _driver.FindElement(_deliveryDealerBy).Clear();
            _driver.FindElement(_deliveryDealerBy).SendKeys(deliveryDealer);

            // Payment method (select)
            SelectElement selectElement = new SelectElement(_driver.FindElement(_paymentMethodBy));
            selectElement.SelectByText(paymentMethod);
        }

        public void FillInRentalDescription(string rentalDescription, int movieId)
        {
            _driver.FindElement(By.Id("description_" + movieId)).SendKeys(rentalDescription);
        }


        public void PressRentYourCars()
        {
            _driver.FindElement(By.Id("Submit")).Click();
        }



        public void PressModifyMovies()
        {
            _driver.FindElement(By.Id("ModifyMovies")).Click();
        }

        public bool CheckListOfRentalItems(List<string[]> expectedRentalItems)
        {
            return CheckBodyTable(expectedRentalItems, By.Id("TableOfRentalItems"));
        }

        public void PressModifyCars()
        {
            By modifyCarsBy = By.Id("ModifyCars");

            WaitForBeingVisible(modifyCarsBy);
            WaitForBeingClickable(modifyCarsBy);

            _driver.FindElement(modifyCarsBy).Click();
        }


        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }
    }
}
