using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.UC_Rental {
    public class UC_RentalCars_UIT : UC_UIT {

        public UC_RentalCars_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();
            listcars = new SelectCarsForRental_PO(_driver, _output);
        }

        // Mi UC2_4: FILTRO POR MODEL (R8)                    
        private const string carRentingPrice1 = "35000";
        private const string carModel1 = "R8";
        private const string carReleaseDate1 = "1/1/2015";

        // Mi UC2_5: FILTRO POR RENTINGPRICE (35000)
        private const string carRentingPrice2 = "28000";
        private const string carModel2 = "Mustang";
        private const string carReleaseDate2 = "3/1/2015";

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
        [InlineData(carRentingPrice1, carModel1, "", "R8" )]        //UC2_4: FILTRO POR MODEL (R8)
        [InlineData(carRentingPrice2, carModel2, "30000", "")]      //UC2_5: FILTRO POR RENTINGPRICE (30000)
        [Trait("LevelTesting", "Funcional Testing")]    
        public void UC2_AF1_UC2_4_5_filteringByRentingPriceAndModel(string rentingPrice, string modelname, string filterRentingPrice, string filterModel)
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);
            var expectedCars = new List<string[]> { new string[] { rentingPrice, modelname,  }, };

            //Act
            InitialStepsForRentalCars();
            listcars.FilterCars(filterRentingPrice, filterModel, from, to);

            //Assert
            Assert.True(listcars.CheckListOfCars(expectedCars));
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
    }
}
