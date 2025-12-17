﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_Review
{
    public class CreateReview_PO : PageObject
    {
        // Selectores estáticos
        private By _nameBy = By.Id("Name");
        private By _surnameBy = By.Id("Surname");
        private By _countryBy = By.Id("Country");
        private By _driverTypeBy = By.Id("DriverType");

        // Selectores de botones
        private By _reviewCarsBtnBy = By.Id("Submit");
        private By _modifyCarsBtnBy = By.Id("ModifyCar");
        private By _tableReviewItemsBy = By.Id("TableOfReviewItems");

        // Elementos web (helpers)
        private IWebElement _name() => _driver.FindElement(_nameBy);
        private IWebElement _surname() => _driver.FindElement(_surnameBy);
        private IWebElement _country() => _driver.FindElement(_countryBy);
        private IWebElement _driverType() => _driver.FindElement(_driverTypeBy);

        public CreateReview_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        // Rellena la información del conductor
        public void FillInReviewInfo(string name, string surname, string country, string driverType)
        {
            // Esperamos a que el primer campo sea visible para asegurar que la página cargó
            WaitForBeingVisible(_nameBy);

            _name().SendKeys(name);
            _surname().SendKeys(surname);
            _country().SendKeys(country);
            _driverType().SendKeys(driverType);
        }

        // Rellena la descripción y el rating del coche en la tabla
        public void FillInCarDetails(string description, int rating, string model)
        {
            var safeModel = model.Replace(" ", "_");
            By descriptionBy = By.Id("description_" + safeModel);
            By ratingBy = By.Id("rating_" + safeModel);

            WaitForBeingVisible(descriptionBy);
            var desc = _driver.FindElement(descriptionBy);
            desc.Clear();
            desc.SendKeys(description);
            desc.SendKeys(Keys.Tab);

            WaitForBeingVisible(ratingBy);
            var rate = _driver.FindElement(ratingBy);
            rate.Clear();
            rate.SendKeys(rating.ToString());
            rate.SendKeys(Keys.Tab); // <-- dispara validación
        }
        


        // Botón "Review your cars" (Confirmar)
        public void PressReviewYourCars()
        {
            _driver.FindElement(_reviewCarsBtnBy).Click();
        }

        // Botón "Modify cars" (Volver atrás)
        public void PressModifyCars()
        {
            _driver.FindElement(_modifyCarsBtnBy).Click();
        }

        // Chequeo de la tabla resumen
        public bool CheckListOfReviewItems(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, _tableReviewItemsBy);
        }

        // Verificación de errores de validación
        public bool CheckValidationError(string expectedError)
        {
            Thread.Sleep(2000); // Pequeña espera para asegurar que el error se renderizó
            return _driver.PageSource.Contains(expectedError);
        }
    }
}