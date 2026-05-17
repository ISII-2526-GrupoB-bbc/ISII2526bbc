using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.CU_Review;
using AppForSEII2526.UIT.Shared;
using AppForSEII2526.UIT.UC_Rental;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace AppForSEII2526.UIT.CU_Review
{
    public class CU_ReviewCars_UIT : UC_UIT
    {
        private const int carId1 = 1;
        private const string carModel1 = "R8";
        private const string carClass1 = "Deportivo";
        private const string carManufacturer1 = "Audi";
        private const string carFuelType1 = "Gasolina";
        private const string carColor1 = "Rojo";

        private const int carId2 = 2;
        private const string carModel2 = "Model S";
        private const string carClass2 = "Lujo";
        private const string carManufacturer2 = "Tesla";
        private const string carFuelType2 = "Eléctrico";
        private const string carColor2 = "Blanco";

        private const string name = "Juan Pérez";
        private const string surname = "García";
        private const string country = "España";
        private const string driverType = "Experto";

        //Datos de la Reseña
        private const string description = "Reseña para.";
        private const int rating = 5;

        //Page Object
        private SelectCarsForReview_PO selectCarsForReview_PO;
        private CreateReview_PO createReview_PO;
        private DetailReview_PO detailReview_PO;


        public CU_ReviewCars_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();
            selectCarsForReview_PO = new SelectCarsForReview_PO(_driver, _output);
            createReview_PO = new CreateReview_PO(_driver, _output);
            detailReview_PO = new DetailReview_PO(_driver, _output);

        }

        private void Precondition_perform_login()
        {
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void InitialStepsForReviewCars()
        {
            //Precondition_perform_login();

            // Espera de seguridad tras login
            Thread.Sleep(1000);

            var byCreate = By.Id("CreateReview");
            selectCarsForReview_PO.WaitForBeingClickable(byCreate);
            var element = _driver.FindElement(byCreate);

            IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
            executor.ExecuteScript("arguments[0].click();", element);

            try
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.Url.Contains("review"));
            }
            catch (WebDriverTimeoutException)
            {
                _driver.Navigate().GoToUrl(new Uri(_driver.Url).GetLeftPart(UriPartial.Authority) + "/review/select");
            }
        }
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_1_ReviewCorrecta()
        {
            // Arrange
            InitialStepsForReviewCars();

            // Act
            selectCarsForReview_PO.AddCarToReviewCart(carModel2);
            selectCarsForReview_PO.ReviewCars();

            createReview_PO.FillInReviewInfo(name, surname, country, driverType);
            createReview_PO.FillInCarDetails(description, rating, carModel2);
            createReview_PO.PressReviewYourCars();

            var expectedReviewItems = new List<string[]>
    {
        new string[]
        {
            carModel2,
            carFuelType2,
            carManufacturer2,
            carColor2,
            rating.ToString(),
            description
        }
    };

            // Assert
            Assert.True(detailReview_PO.CheckReviewDetail($"{name} {surname}", country, driverType, DateTime.Now));
            Assert.True(detailReview_PO.CheckListOfReview(expectedReviewItems));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_2_FiltroPorFabricante()
        {
            // Arrange
            InitialStepsForReviewCars();

            var expectedCars = new List<string[]>
    {
        new string[]
        {
            carModel2,
            carClass2,
            carManufacturer2,
            carColor2,
            carFuelType2,
            "Add"
        }
    };

            // Act
            selectCarsForReview_PO.SearchCars("Tesla", "");

            // Assert
            Assert.True(selectCarsForReview_PO.CheckListOfCars(expectedCars));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_3_FiltroPorCombustible()
        {
            // Arrange
            InitialStepsForReviewCars();

            var expectedCars = new List<string[]>
    {
        new string[]
        {
            carModel1,
            carClass1,
            carManufacturer1,
            carColor1,
            carFuelType1,
            "Add"
        }
    };

            // Act
            selectCarsForReview_PO.SearchCars("", "Gasolina");

            // Assert
            Assert.True(selectCarsForReview_PO.CheckListOfCars(expectedCars));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_4_BotonDeshabilitadoSinCocheSeleccionado()
        {
            // Arrange
            InitialStepsForReviewCars();

            // Act
            // No seleccionamos ningún coche

            // Assert
            Assert.True(selectCarsForReview_PO.ReviewNotAvailable());
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_5_BorrarCochesSeleccionados()
        {
            //Arrange
            InitialStepsForReviewCars();

            //Act
            selectCarsForReview_PO.AddCarToReviewCart(carModel1);
            selectCarsForReview_PO.AddCarToReviewCart(carModel2);
            selectCarsForReview_PO.ReviewCars();

            createReview_PO.FillInReviewInfo(name, surname, country, driverType);

            createReview_PO.PressModifyCars();

            selectCarsForReview_PO.RemoveCarFromReviewCart(carModel2);

            selectCarsForReview_PO.ReviewCars();

            var expectedReviewItems = new List<string[]> {
                new string[] { carModel1, carFuelType1, carManufacturer1, carColor1 }
            };

            Assert.True(createReview_PO.CheckListOfReviewItems(expectedReviewItems));
        }


        [Theory]
        [InlineData("", surname, country, driverType, description, rating, "The field Name must be a string with a minimum length of 2 and a maximum length of 20.")]
        [InlineData(name, surname, " ", driverType, description, rating, "The field Country must be a string with a minimum length of 3 and a maximum length of 30.")]
        [InlineData(name, surname, country, "", description, rating, "The DriverType field is required.")]
        [InlineData(name, surname, country, driverType, description, 0, "The field Rating must be between 1 and 5.")]

        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_AF3_UC4_6_7_8_9_Testing_Errors_Mandatory_Data(
        string name,
        string surname,
        string country,
        string driverType,
        string description,
        int rating,
        string expectedMessageError)
        {
            // Arrange
            InitialStepsForReviewCars();

            // Act
            selectCarsForReview_PO.AddCarToReviewCart(carModel1);
            selectCarsForReview_PO.ReviewCars();

            createReview_PO.FillInReviewInfo(name, surname, country, driverType);
            createReview_PO.FillInCarDetails(description, rating, carModel1);
            createReview_PO.PressReviewYourCars();

            // Assert
            Assert.True(
                createReview_PO.CheckValidationError(expectedMessageError),
                $"Expected error: {expectedMessageError}"
            );
        }
    }

}
