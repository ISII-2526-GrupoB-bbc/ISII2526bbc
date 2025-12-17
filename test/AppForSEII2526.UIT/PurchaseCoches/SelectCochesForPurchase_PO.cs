using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase
{
    /// <summary>
    /// Page Object de la pantalla:
    /// /purchase/selectcarforpurchase
    /// Encapsula las acciones y comprobaciones de selección de coches
    /// </summary>
    public class SelectCochesForPurchase_PO : PageObject
    {
        // ========= LOCATORS =========
        private By addFirstCarButton =  By.CssSelector("button[id^='AddCarButton_']"); //Coge el primer botón Añadir que exista, da igual el ID exacto, me daba problemas si ponía un ID concreto

        private By finishPurchaseButton = By.Id("FinishPurchaseButton");
        private By PurchaseCart = By.Id("PurchaseCart");

        private By colorInput = By.Id("ColorFilter");
        private By modelInput = By.Id("ModelFilter");
        private By searchButton = By.Id("SearchCarsButton");

        // Zona donde SIEMPRE hay algo (loading / mensaje / tabla)
        private By carsResultZone = By.Id("CarsResultZone");

        // Tabla SOLO cuando hay resultados
        private By carsTable = By.Id("CarsTable");

        public SelectCochesForPurchase_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        // ========= ACCIONES =========

        /// <summary>
        /// UC1_3 – Filtrar coches por color y modelo
        /// </summary>
        public void FilterCars(string color, string model)
        {
            // Espera base: inputs cargados
            WaitForBeingVisible(colorInput);

            _driver.FindElement(colorInput).Clear();
            _driver.FindElement(colorInput).SendKeys(color);

            _driver.FindElement(modelInput).Clear();
            _driver.FindElement(modelInput).SendKeys(model);

            _driver.FindElement(searchButton).Click();

            // ESPERA CLAVE (como en Rental)
            WaitForBeingVisible(carsResultZone);
        }

        // ========= VERIFICACIONES =========

        public bool CarsTableIsDisplayed()
        {
            return _driver.FindElements(carsTable).Any()
                   && _driver.FindElement(carsTable).Displayed;
        }

        public bool TableContains(string text)
        {
            return _driver.FindElement(carsTable).Text.Contains(text);
        }
        public void SearchCars(string color, string model)
        {
            WaitForBeingVisible(colorInput);

            if (!string.IsNullOrEmpty(color))
            {
                _driver.FindElement(colorInput).Clear();
                _driver.FindElement(colorInput).SendKeys(color);
            }

            if (!string.IsNullOrEmpty(model))
            {
                _driver.FindElement(modelInput).Clear();
                _driver.FindElement(modelInput).SendKeys(model);
            }

            _driver.FindElement(searchButton).Click();
        }

        public void AddFirstCarToCart()
        {
            WaitForBeingVisible(addFirstCarButton);
            _driver.FindElement(addFirstCarButton).Click();
        }

        public void FinishPurchase()
        {
            WaitForBeingVisible(finishPurchaseButton);
            _driver.FindElement(finishPurchaseButton).Click();
        }

        /// Comprueba que el carrito NO es visible
        public bool ShoppingCartIsHidden()
        {
            var cart = _driver.FindElement(By.Id("PurchaseCart"));
            return cart.GetAttribute("hidden") != null;
        }

        //Esto se usa en el UC 5 para esperar a que el carrito desaparezca
        public void WaitForCartToDisappear()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(driver =>
            {
                var cart = driver.FindElement(By.Id("PurchaseCart"));
                return cart.GetAttribute("hidden") != null;
            });
        }




        /// Comprueba que el botón de finalizar compra NO es visible
        public bool FinishPurchaseButtonIsHidden()
        {
            var elements = _driver.FindElements(finishPurchaseButton);

            if (!elements.Any())
                return true; // no existe = OK

            return !elements.First().Displayed;
        }

        /// Elimina el primer coche del carrito
       
        public void RemoveFirstCarFromCart()
        {
            // buscamos cualquier botón de eliminar existente
            var removeButtons = _driver.FindElements(
                By.XPath("//button[starts-with(@id,'RemoveCarButton_')]")
            );

            if (removeButtons.Any())
            {
                removeButtons.First().Click();
            }
        }







    }
}
