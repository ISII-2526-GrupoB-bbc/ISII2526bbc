using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppForSEII2526.UIT.UC_Rental {
    public class UC_RentalCars_UIT : UC_UIT {

        public UC_RentalCars_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();
            listcars = new SelectCarsForRental_PO(_driver, _output);
        }

        // Mi UC2_4: FILTRO POR MODEL (R8)
        private const int carId1 = 2;
        private const string carRentingPrice1 = "35000";
        private const string carModel1 = "R8";
        private const string carManufacturer1 = "Audi";
        private const string carColor1 = "Rojo";
        private const string carFuelType1 = "Gasolina";
        private const string carDescription1 = "Superdeportivo";

        // Mi UC2_5: FILTRO POR RENTINGPRICE (35000)
        private const int carId2 = 4;
        private const string carRentingPrice2 = "28000";
        private const string carModel2 = "Mustang";
        private const string carManufacturer2 = "Ford";
        private const string carColor2 = "Negro";
        private const string carFuelType2 = "Diésel";
        private const string carDescription2 = "Motor potente";

        private SelectCarsForRental_PO listcars;

        

        private void Precondition_perform_login()
        {
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void InitialStepsForRentalCars()
        {
            //COMENTAMOS EL LOGIN: Precondition_perform_login();
            listcars.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("CreateRental"));
            _driver.FindElement(By.Id("CreateRental")).Click();
        }

        [Theory]
        [InlineData("R8", "35000", carFuelType1, carManufacturer1, carColor1)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_4_FilteringOnlyByModel(
            string model,
            string rentingPrice,
            string fuelType,
            string manufacturer,
            string color)
        {
            // Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            var expectedCars = new List<string[]>
            {
                new string[] { model, fuelType, manufacturer, rentingPrice, color, "Add" }
            };

            // Act
            InitialStepsForRentalCars();
            listcars.FilterCars("", model, from, to);   // rentingPrice vacío -> filtro solo por el modelo

            // Assert
            Assert.True(listcars.CheckListOfCars(expectedCars),
                "Filtering by model did not return expected cars");
        }

        [Theory]
        [InlineData("Mustang", "30000", carFuelType2, carManufacturer2, carColor2)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_4_FilteringOnlyByRentingPrice(
            string model,
            string rentingPrice,
            string fuelType,
            string manufacturer,
            string color)
        {
            // Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            var expectedCars = new List<string[]>
            {
                new string[] { model, fuelType, manufacturer, carRentingPrice2, color, "Add" }
            };

            // Act
            InitialStepsForRentalCars();
            listcars.FilterCars(rentingPrice, "", from, to);   // rentingPrice vacío -> filtro solo por el modelo

            // Assert
            Assert.True(listcars.CheckListOfCars(expectedCars),
                "Filtering by model did not return expected cars");
        }

        

        //COMPROBAR ERRORES EN FEHCAS: UC2_6, UC2_7, UC2_8
        public static IEnumerable<object[]> TestCasesFor_UC2_4_5_AF2_errorindates()
        {
            var allTests = new List<object[]> {
                //Todos los dias estan igual que en mi Word de Casos de Prueba
                new object[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(2), "Your rental period must be later",  },         //EL ALQUILER EMPIEZA AYER
                new object[] { DateTime.Today.AddDays(-2), DateTime.Today.AddDays(-1), "Your rental period must be later", },         //EL ALQUILER EMPIEZA Y ACABA AYER
                new object[] { DateTime.Today.AddDays(5), DateTime.Today.AddDays(3), "Your rental must end after than its starts", }, //EL ALQUILER TERMINA ANTES DE EMPEZAR
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_UC2_4_5_AF2_errorindates))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_6_7_8_AF2_errorindates(DateTime from, DateTime to, string error)
        {
            //Arrange


            //Act
            InitialStepsForRentalCars();
            listcars.FilterCars("", "", from, to);

            //Assert

            //this message will be shown if assert fails
            Assert.True(listcars.CheckMessageErrorNotAvaibleCars(error), $"Error in the error area for test {from} - {to}");


        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_10_AF3_ModifySelectedCars()
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalCars();

            listcars.FilterCars("", "", from, to);
            listcars.SelectCars(new List<string> { carModel1, carModel2 });
            listcars.ModifyRentingCart(carModel2);


            //Assert            
            Assert.True(listcars.CheckShoppingCart(carModel1));
        }

        /*
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_11_AF4_RentButtonNotAvailable()
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalCars();

            listcars.FilterCars("", "", from, to);
            listcars.SelectCars(new List<string> { carModel1 });
            listcars.ModifyRentingCart(carModel1);


            //Assert            
            Assert.True(listcars.IsShoppingCartEmpty(), "Rent button should be disabled");
        }
        */
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_11_AF4_RentButtonNotAvailable()
        {
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            InitialStepsForRentalCars();

            listcars.FilterCars("", "", from, to);
            listcars.SelectCars(new List<string> { carModel1 });
            listcars.ModifyRentingCart(carModel1);

            Assert.True(listcars.IsShoppingCartEmpty(), "Rent button should be disabled");
        }


        // PRUEBAS DE RELLENAR CAMPOS DEL POST

        [Theory]
        [InlineData("gaspar123", "", "Martinez", "Calle de la Universidad 1, Albacete, 02006, España", "The CustomerName field is required")]
        [InlineData("gaspar123", "G", "Martinez", "Calle de la Universidad 1, Albacete, 02006, España", "The field CustomerName must be a string with a minimum length of 2 and a maximum length of 50.")]
        [InlineData("gaspar123", "Gaspar", "Martrinez", "", "The DeliveryCarDealer field is required.")]
        [InlineData("gaspar123", "Gaspar", "Martinez", "Calle", "The field DeliveryCarDealer must be a string with a minimum length of 10 and a maximum length of 150.")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_12_13_14_15_AF5_testingErrorsMandatorydata(string username, string name, string surname, string deliveryAddress,
            string expectedMessageError)
        {
            //Arrange

            var createrental = new CreateRental_PO(_driver, _output);

            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);


            //Act
            InitialStepsForRentalCars();

            listcars.FilterCars("", "", from, to);
            listcars.SelectCars(new List<string> { carModel1 });
            listcars.RentCars();
            createrental.FillInRentalInfo(username, name, surname, deliveryAddress, "CreditCard");
            createrental.PressRentYourCars();

            //Assert
            //the expected error is shown in the view
            Assert.True(createrental.CheckValidationError(expectedMessageError), $"Expected error: {expectedMessageError}");
        }


        //MODIFICAR COCHES SELECCIONADOS

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_16_AF6_ModifyRentalItems()
        {
            //Arrange

            var createrental = new CreateRental_PO(_driver, _output);

            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalCars();

            listcars.FilterCars("", "", from, to);
            listcars.SelectCars(new List<string> { carModel1, carModel2 });
            listcars.RentCars();
            createrental.PressModifyCars();
            //we remove movietitle2 from the rentingcart
            listcars.ModifyRentingCart(carModel2);
            listcars.RentCars();

            //Assert
            //the list of movies must change
            var expectedRentalItems = new List<string[]> { new string[] { carModel1, carManufacturer1, carRentingPrice1, "1"}, };
            Assert.True(createrental.CheckListOfRentalItems(expectedRentalItems));
        }

        [Theory]
        [InlineData("elena@uclm.es", "Elena", "Navarro Martínez", "Calle de la Universidad 1, Albacete, 02006, España", "CreditCard")]
        [InlineData("elena@uclm.es", "Elena", "Navarro Martrínez", "Calle de la Universidad 1, Albacete, 02006, España", "PayPal")]
        [InlineData("elena@uclm.es", "Elena", "Navarro Martínez", "Calle de la Universidad 1, Albacete, 02006, España", "Cash")]


        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_1_2_3_BasicFlow(string username, string name, string surname, string deliveryAddress, string paymentMethod)
        {
            //Arrange

            var createrental = new CreateRental_PO(_driver, _output);
            var detailRental = new DetailRental_PO(_driver, _output);

            var from = DateTime.Today.AddDays(1);
            var to = DateTime.Today.AddDays(2);



            //Act
            InitialStepsForRentalCars();

            listcars.FilterCars("", "", from, to);
            listcars.SelectCars(new List<string> { carModel1 });
            listcars.RentCars();

            createrental.FillInRentalInfo(username, name, surname, deliveryAddress, paymentMethod);
            //createrental.FillInRentalDescription(carDescription1, carId1);
            createrental.PressRentYourCars();
            createrental.PressOkModalDialog();


            //Assert
            //the expected error is shown in the view
            Assert.True(
            detailRental.CheckRentalDetail(
                paymentMethod,
                from,
                to,
                carRentingPrice1 + " €"),
            "Error: detail rental is not as expected"
            );





            var expectedRentalItems = new List<string[]>
            {
                new string[]
                {
                    carModel1,          // Model
                    carManufacturer1,   // Manufacturer (ej: "Audi")
                    carRentingPrice1 + " €",
                    "1"                 // Quantity
                }
            };


            Assert.True(detailRental.CheckListOfCars(expectedRentalItems),
                "Error: rental items are not as expected");

        }



        // ===================== EXAMEN 3ER SPRINT GASPAR =================

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void FlujoExamen_Gaspar()
        {

            //Arrange

            var createrental = new CreateRental_PO(_driver, _output);
            var detailRental = new DetailRental_PO(_driver, _output);

            var from = DateTime.Today.AddDays(1);
            var to = DateTime.Today.AddDays(2);


            //Act

            InitialStepsForRentalCars();

                // 1. Filtrar por modelo y añadir uno de los coches
            listcars.FilterCars("", carModel1, from, to);                                       //Filtro por R8
            listcars.SelectCars(new List<string> { carModel1 });                                //Cojo el R8

                // 2. Filtrar por precio y añadir otro coche distinto al anterior
            listcars.CambiarFiltroPrecioModelo("30000", "");                                    //Filtro por precio (Mustang)      ESTE METODO ES NUEVO
            listcars.SelectCars(new List<string> { carModel2 });                                //Cojo el Mustang

            Thread.Sleep(1000);

            
                // 3. Eliminar el primer coche añadido
            listcars.ModifyRentingCart(carModel1);                                              //Borro el R8

            Thread.Sleep(1000);

                // 4. Continuar con el flujo básico
            listcars.RentCars();

            createrental.FillInRentalInfo("elena@uclm.es", "Elena", "Navarro Martínez", "Calle de la Universidad 1, Albacete, 02006, España", "CreditCard");
            Thread.Sleep(1000);

            createrental.PressRentYourCars();
            createrental.PressOkModalDialog();

            Thread.Sleep(2000);

            //Assert

            Assert.True(
            detailRental.CheckRentalDetail(
                "CreditCard",
                from,
                to,
                carRentingPrice2 + " €"),
            "Error: detail rental is not as expected"
            );

            var expectedRentalItems = new List<string[]>
            {
                new string[]
                {
                    carModel2,          
                    carManufacturer2,   
                    carRentingPrice2 + " €",
                    "1"                
                }
            };

            Assert.True(detailRental.CheckListOfCars(expectedRentalItems),
                "Error: rental items are not as expected");
        }


    }
}

