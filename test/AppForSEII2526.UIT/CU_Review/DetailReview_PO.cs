using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_Review
{
    public class DetailReview_PO : PageObject
    {
        public DetailReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {

        }

        public bool CheckReviewDetail(string nameSurname, string country, string driverType, DateTime created)
        {
            Thread.Sleep(500);
            bool result = true;
            result = result && _driver.FindElement(By.Id("NameSurname")).Text.Contains(nameSurname);
            result = result && _driver.FindElement(By.Id("Country")).Text.Contains(country);
            result = result && _driver.FindElement(By.Id("DriverType")).Text.Contains(driverType);

            var actualReviewDate = DateTime.Parse(_driver.FindElement(By.Id("Created")).Text);
            result = result && ((actualReviewDate - actualReviewDate) < new TimeSpan(0, 1, 0));


            return result;
        }

        public bool CheckListOfReview(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("ReviewedCars"));
        }
    }
}