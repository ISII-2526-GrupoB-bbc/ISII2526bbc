using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;   

namespace AppForSEII2526.UIT.UC_Rental { 
    public class SelectCarsForRental_PO : PageObject
    {
        private By inputRentingPrice = By.Id("rentingprice");    // title = rentingprice
        private By inputModel = By.Id("modelname");              // genre = model
        private By inputStartDate = By.Id("fromDate");
        private By inputEndDate = By.Id("toDate");

        private By showRentingCart = By.Id("showRentingCart");
        private By buttonSearchCars = By.Id("searchCars");
        private By buttonRentCars = By.Id("rentCarButton");

        private By tableOfCarsBy = By.Id("TableOfCars");
        private By _modalBy = By.Id("DialogOKSaveDelete");

        private IWebElement carRentingPrice() => _driver.FindElement(inputRentingPrice);

        private By errorShownBy = By.Id("ErrorsShown");

        private IWebElement _carModel() => _driver.FindElement(inputModel);
        private IWebElement _showRentingCartButton() => _driver.FindElement(showRentingCart);
        private IWebElement _searchCarsButton() => _driver.FindElement(buttonSearchCars);
        private IWebElement _rentButton() => _driver.FindElement(buttonRentCars);


        public SelectCarsForRental_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {

        }

        // =========== METODO NUEVO PARA EL EXAMEN =================================================

        public void CambiarFiltroPrecioModelo(string rentingPriceFilter, string modelSelected)
        {
            WaitForBeingVisible(inputRentingPrice);

            // Renting price
            carRentingPrice().Clear();
            if (!string.IsNullOrEmpty(rentingPriceFilter))
                carRentingPrice().SendKeys(rentingPriceFilter);

            // Model (ES INPUT, NO SELECT)
            var modelInput = _carModel();
            modelInput.Clear();
            if (!string.IsNullOrEmpty(modelSelected))
                modelInput.SendKeys(modelSelected);

            _searchCarsButton().Click();
            _searchCarsButton().Click();
            //we wait for 2 seconds (2000 milliseconds) till the table is reloaded as we have to wait for the API service to be called
            System.Threading.Thread.Sleep(2000);

        }

        // ===================================================================================



        public void FilterCars(string rentingPriceFilter, string modelSelected, DateTime from, DateTime to)
        {
            WaitForBeingVisible(inputRentingPrice);

            // Renting price
            carRentingPrice().Clear();
            if (!string.IsNullOrEmpty(rentingPriceFilter))
                carRentingPrice().SendKeys(rentingPriceFilter);

            // Model (ES INPUT, NO SELECT)
            var modelInput = _carModel();
            modelInput.Clear();
            if (!string.IsNullOrEmpty(modelSelected))
                modelInput.SendKeys(modelSelected);

            InputDateInDatePicker(inputStartDate, from);
            InputDateInDatePicker(inputEndDate, to);

            _searchCarsButton().Click();
            _searchCarsButton().Click();
            //we wait for 2 seconds (2000 milliseconds) till the table is reloaded as we have to wait for the API service to be called
            System.Threading.Thread.Sleep(2000);

        }



        public void SelectCars(List<string> modelNames)
        {
            //we wait for till the movies are available to be selected 
            foreach (var modelName in modelNames)
            {
                WaitForBeingVisible(By.Id($"carToRent_{modelName}"));
                _driver.FindElement(By.Id($"carToRent_{modelName}")).Click();
            }
        }

        public void RentCars()
        {
            WaitForBeingClickable(buttonRentCars);
            _rentButton().Click();
        }

        public void ModifyRentingCart(string modelName)
        {
            By removeButton = By.Id($"removeCar_{modelName}");

            WaitForBeingVisible(removeButton);
            WaitForBeingClickable(removeButton);

            _driver.FindElement(removeButton).Click();
        }




        public bool CheckListOfCars(List<string[]> expectedCars)
        {
            return CheckBodyTable(expectedCars, tableOfCarsBy);
        }

        public bool CheckRentCarsDisabled()
        {
            //comprueba que el boton de alquilar esta deshabilitado
            return !(_rentButton().Enabled);
        }

        public bool CheckShoppingCart(string modelname)
        {
            //string texto = _showRentingCartButton().Text;
            //WaitForTextToBePresentInElement(_rentButtonBy, $"Renting Cart: {price} €" );
            return _driver.FindElements(By.Id($"removeCar_{modelname}")).Any(); 
        }

        public bool CheckMessageErrorNotAvaibleCars(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }

        public bool CheckMessageError(string expectedError)
        {
            return CheckModalBodyText(expectedError, _modalBy);
        }

        public bool IsShoppingCartEmpty()
        {
            return !_driver.PageSource.Contains("removeCar_");
        }




        //===================== A PARTIR DE AQUI SON COSAS QUE HICE SIGUIENDO EL AppForMoviesForClasses QUE NO ESTÁN EN AppForMovies ==================================

        /*
        public void SearchCars(string? rentingPrice, string modelName, string startDate, string endDate)
        {
            //wait for the webelement to be clickable
            WaitForBeingVisible(inputRentingPrice);
            _driver.FindElement(inputRentingPrice).SendKeys(rentingPrice);   //Tengo que pasar RentingPrice a string porque SendKeys solo acepta strings
            if (modelName == "") modelName = "All";
            SelectElement selectElement = new SelectElement(_driver.FindElement(inputModel));
            selectElement.SelectByText(modelName);

            if (startDate != "")
                _driver.FindElement(inputStartDate).SendKeys(startDate);

            if (endDate != "")
                _driver.FindElement(inputEndDate).SendKeys(endDate);    

            _driver.FindElement(buttonSearchCars).Click();
        }


        public bool CheckListOfCars(List<string[]> expectedCars)
        {

            return CheckBodyTable(expectedCars, tableOfCarsBy);
        }

        public bool CheckMessageError(string errorMessage)
        {
            IWebElement actualErrorShown = _driver.FindElement(errorShownBy);
            _output.WriteLine($"actual Message shown:{actualErrorShown.Text}");
            return actualErrorShown.Text.Contains(errorMessage);
        }

        public void AddCarToRentingCart(string modelname)
        {
            WaitForBeingClickable(By.Id("movieToRent_" + modelname));

            _driver.FindElement(By.Id("movieToRent_" + modelname)).Click();
        }

        public void RemoveCarFromRentingCart(string modelname)
        {
            WaitForBeingClickable(By.Id("removeMovie_" + modelname));
            _driver.FindElement(By.Id("removeMovie_" + modelname)).Click();
        }

        // METODO PARA OCULTAR EL BOTON DE ALQUILAR SI NO HAY COCHES DISPONIBLES
        public bool RentingNotAvailable()
        {
            //the button is not Displayed=hidden

            return _driver.FindElement(buttonRentCars).Displayed == false;
        }
        */
    }
}
