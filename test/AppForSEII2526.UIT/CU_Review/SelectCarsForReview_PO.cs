using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_Review
{
    public class SelectCarsForReview_PO : PageObject
    {
        // Selectores basados en tu HTML
        private By inputManufacturer = By.Id("inputManufacturer");
        private By inputFuelType = By.Id("inputFuelType"); // Es un Input, no un Select
        private By buttonSearchCars = By.Id("searchCars");
        private By tableOfCarsBy = By.Id("TableOfCars");
        private By errorShownBy = By.Id("ErrorsShown");

        private By buttonReviewCars = By.Id("reviewCarButton");
        public SelectCarsForReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {

        }

        public void SearchCars(string manufacturer, string fueltype)
        {
            // 1. Llenar Fabricante
            WaitForBeingClickable(inputManufacturer);
            var manuElement = _driver.FindElement(inputManufacturer);
            manuElement.Clear();
            manuElement.SendKeys(manufacturer);

            // 2. Llenar Tipo de Combustible (Corregido: es un Input, no un Select)
            var fuelElement = _driver.FindElement(inputFuelType);
            fuelElement.Clear();

            // Si el test pasa un valor vacío (""), asumimos que no quiere filtrar
            if (!string.IsNullOrEmpty(fueltype))
            {
                fuelElement.SendKeys(fueltype);
            }

            // 3. Click en buscar
            _driver.FindElement(buttonSearchCars).Click();
        }

        public bool CheckListOfCars(List<string[]> expectedCars)
        {
            return CheckBodyTable(expectedCars, tableOfCarsBy);
        }

        public bool CheckMessageError(string errorMessage)
        {
            WaitForBeingVisible(errorShownBy);
            IWebElement actualErrorShown = _driver.FindElement(errorShownBy);
            _output.WriteLine($"actual Message shown:{actualErrorShown.Text}");
            return actualErrorShown.Text.Contains(errorMessage);
        }

        public void AddCarToReviewCart(string carModel)
        {
            // CORRECCIÓN: Usamos XPath para soportar espacios en el ID (ej: "Audi A4")
            // By.Id a veces falla con espacios, XPath es más seguro aquí.
            By buttonAddBy = By.Id("carToReview_" + carModel);

            WaitForBeingClickable(buttonAddBy);
            _driver.FindElement(buttonAddBy).Click();
        }

        public void RemoveCarFromReviewCart(string carModel)
        {
            // Misma corrección para el botón de borrar para soportar espacios
            By buttonRemoveBy = By.Id("removeCar_" + carModel);

            WaitForBeingClickable(buttonRemoveBy);
            _driver.FindElement(buttonRemoveBy).Click();
        }

        public void ReviewCars()
        {
            WaitForBeingClickable(buttonReviewCars);
            _driver.FindElement(buttonReviewCars).Click();
        }

        public bool ReviewNotAvailable()
        {
            try
            {
                var button = _driver.FindElement(By.Id("reviewCarButton"));
                return !button.Displayed || !button.Enabled;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        }
    }
}
