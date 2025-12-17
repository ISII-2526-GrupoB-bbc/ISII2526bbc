using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase
{
    /// <summary>
    /// Page Object de la pantalla:
    /// /purchase/detailpurchase
    /// 
    /// Encapsula TODAS las verificaciones del detalle de una compra.
    /// NO navega, NO realiza acciones.
    /// </summary>
    public class DetailPurchase_PO : PageObject
    {
        public DetailPurchase_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        /// <summary>
        /// Comprueba los datos generales de la compra
        /// </summary>
        public bool CheckPurchaseDetail(
            string fullName,
            string address,
            string paymentMethod,
            DateTime purchaseDate,
            string totalPrice)
        {
            // Esperamos a que el precio total esté visible (página cargada)
            WaitForBeingVisible(By.Id("TotalPrice"));

            bool result = true;

            result &= _driver.FindElement(By.Id("NameSurname")).Text.Contains(fullName);
            result &= _driver.FindElement(By.Id("Address")).Text.Contains(address);
            result &= _driver.FindElement(By.Id("PaymentMethod")).Text.Contains(paymentMethod);
            result &= _driver.FindElement(By.Id("TotalPrice")).Text.Contains(totalPrice);

            // Fecha de compra (permitimos 1 minuto de diferencia)
            var actualPurchaseDate =
                DateTime.Parse(_driver.FindElement(By.Id("PurchasingDate")).Text);

            result &= ((actualPurchaseDate - purchaseDate) < new TimeSpan(0, 1, 0));

            return result;
        }

        /// <summary>
        /// Comprueba la tabla de coches comprados
        /// </summary>
        public bool CheckListOfPurchasedCars(List<string[]> expectedCars)
        {
            return CheckBodyTable(expectedCars, By.Id("PurchasedCars"));
        }


    }
}
