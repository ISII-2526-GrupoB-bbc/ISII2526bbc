using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.UC_Rental
{
    internal class DetailRental_PO : PageObject
    {
        public DetailRental_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckRentalDetail(
            string paymentMethod,
            DateTime from,
            DateTime to,
            string totalPrice)
        {
            WaitForBeingVisible(By.Id("TotalPrice"));

            bool result = true;

            result = result &&
                _driver.FindElement(By.Id("PaymentMethod"))
                       .Text.Contains(paymentMethod);

            result = result &&
                _driver.FindElement(By.Id("TotalPrice"))
                       .Text.Contains(totalPrice);

            result = result &&
                _driver.FindElement(By.Id("RentalPeriod"))
                       .Text.Contains(
                           $"{from:dd/MM/yyyy} - {to:dd/MM/yyyy}");

            return result;
        }






        public bool CheckListOfCars(List<string[]> expectedRentalItems)
        {
            return CheckBodyTable(expectedRentalItems, By.Id("RentedCars"));
        }

    }
}
