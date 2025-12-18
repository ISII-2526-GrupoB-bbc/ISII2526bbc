using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.Purchase;
using OpenQA.Selenium;
using System.Drawing;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.PurchaseCoches
{
    /// <summary>
    /// UC1 – Comprar coches
    /// UC1_3 – Filtrar coches
    /// </summary>
    public class UCPurchaseCoches_UIT : UC_UIT
    {
        private const string carModel1 = "R8";
        private const string carColor1 = "Rojo";

        private const string carModel2 = "Model S";
        private const string carColor2 = "Blanco";

        private SelectCochesForPurchase_PO selectCars;

        public UCPurchaseCoches_UIT(ITestOutputHelper output)
            : base(output)
        {
            Initial_step_opening_the_web_page();
            selectCars = new SelectCochesForPurchase_PO(_driver, _output);
        }

        private void Precondition_perform_login()
        {
            Perform_login("elena@uclm.es", "Password1234%");
        }

        private void InitialStepsForPurchaseCoches_UIT()
        {
          // Precondition_perform_login();

            // MISMA IDEA QUE RENTAL, pero por URL directamente ya que me daba problemas al encontrar el id de CreatePurchase
            _driver.Navigate()
                   .GoToUrl(_URI + "purchase/selectcarforpurchase");

            // Espera estable (no la tabla)
            selectCars.WaitForBeingVisible(By.Id("CarsResultZone"));
        }

        [Theory]
        [InlineData(carColor1, carModel1)]
        [InlineData(carColor2, carModel2)]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_3_AF1_FilterCars_ByColorAndModel(string color, string model)
        {
            // Act
            InitialStepsForPurchaseCoches_UIT();
            selectCars.FilterCars(color, model);

            // Assert
            Assert.True(selectCars.CarsTableIsDisplayed(),
                "La tabla de coches debería mostrarse");

            Assert.True(selectCars.TableContains(model),
                $"La tabla debería contener el modelo {model}");

            Assert.True(selectCars.TableContains(color),
                $"La tabla debería contener el color {color}");
        }
        [Theory]
        [InlineData("Rojo")]
        [InlineData("Blanco")]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_3_AF2_FilterCars_ByColorOnly(string color)
        {
            InitialStepsForPurchaseCoches_UIT();

            selectCars.SearchCars(color, "");

            Assert.True(selectCars.CarsTableIsDisplayed());
            Assert.True(selectCars.TableContains(color));
        }


        [Theory]
        [InlineData("R8")]
        [InlineData("Model S")]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_3_AF3_FilterCars_ByModelOnly(string model)
        {
            InitialStepsForPurchaseCoches_UIT();

            selectCars.SearchCars("", model);

            Assert.True(selectCars.CarsTableIsDisplayed());
            Assert.True(selectCars.TableContains(model));
        }



        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_4_AF1_NoCarSelected_CannotFinishPurchase()
        {
            // Arrange
            InitialStepsForPurchaseCoches_UIT();

            // Act
            // No se selecciona ningún coche



            // Assert
            Assert.True(
                selectCars.ShoppingCartIsHidden(),
                "El carrito no debería mostrarse si no hay coches seleccionados"
            );

            Assert.True(
                selectCars.FinishPurchaseButtonIsHidden(),
                "El botón de finalizar compra no debería ser visible sin coches"
            );
        }



        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_5_AF1_RemoveAllCars_CannotFinishPurchase()
        {
            // ========== Arrange ==========
            InitialStepsForPurchaseCoches_UIT();

            // ========== Act ==========
            // Filtrar y añadir coche
            selectCars.SearchCars("Rojo", "R8");
            selectCars.AddFirstCarToCart();

            // Eliminar el coche del carrito
            selectCars.RemoveFirstCarFromCart();

            selectCars.WaitForCartToDisappear();

            // ========== Assert ==========
            Assert.True(
                selectCars.ShoppingCartIsHidden(),
                "El carrito debería desaparecer al eliminar todos los coches"
            );

            Assert.True(
                selectCars.FinishPurchaseButtonIsHidden(),
                "No debería ser posible finalizar la compra sin coches"
            );


        }


        [Theory]
        [InlineData("","Model S")]
        [InlineData("Rojo","")]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_ExamenAF1(string color, string model)
        {
            // ========== Arrange ==========
            InitialStepsForPurchaseCoches_UIT();

            // ========== Act ==========
            // Filtrar y añadir coche
            selectCars.SearchCars(color, "");
            selectCars.AddFirstCarToCart();

            //Filtra por modelo
            selectCars.SearchCars("", model); 
            selectCars.AddFirstCarToCart();



            // ========== Assert ==========
            
            // Assert
            Assert.True(selectCars.CarsTableIsDisplayed(),
                "La tabla de coches debería mostrarse");

            Assert.True(selectCars.TableContains(model),
                $"La tabla debería contener el modelo {model}");

            Assert.True(selectCars.TableContains(color),
                $"La tabla debería contener el color {color}");




        }






        [Theory]
        [InlineData("", "Navarro", "Calle de la Universidad 1, Albacete, 02006, España", "CreditCard", "Name")]
        [InlineData("E", "Navarro", "Calle de la Universidad 1, Albacete, 02006, España", "CreditCard", "Name")]
        [InlineData("Elena", "", "Calle de la Universidad 1, Albacete, 02006, España", "CreditCard", "Surname")]
        [InlineData("Elena", "Navarro", "", "CreditCard", "Address")]
        [InlineData("Elena", "Navarro", "Calle", "CreditCard", "Address")]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC1_6_AF1_InvalidMandatoryData_CannotFinishPurchase(
     string name,
     string surname,
     string address,
     string paymentMethod,
     string expectedErrorFragment)
        {
            var createPurchase = new CreatePurchase_PO(_driver, _output);

            InitialStepsForPurchaseCoches_UIT();
            selectCars.SearchCars("Rojo", "R8");
            selectCars.AddFirstCarToCart();
            selectCars.FinishPurchase();

            createPurchase.FillInPurchaseInfo(name, surname, address, paymentMethod);
            createPurchase.PressSubmit();

            Assert.True(
                createPurchase.CheckAnyValidationError(expectedErrorFragment),
                $"Expected validation error containing: {expectedErrorFragment}"
            );
        }









        /*
            [Fact]
[Trait("LevelTesting", "Functional Testing")]
public void UC1_1_2_3_BasicFlow_PurchaseCars()
{
    // ========== Arrange ==========
    var createPurchase = new CreatePurchase_PO(_driver, _output);
    var detailPurchase = new DetailPurchase_PO(_driver, _output);

    string name = "Elena";
    string surname = "Navarro";
    string fullName = "Elena Navarro";
    string address = "Calle de la Universidad 1, Albacete, 02006, España";
    string paymentMethod = "CreditCard";

    var expectedCars = new List<string[]>
    {
        new string[] { "R8", "Rojo", "string", "70000 €", "1" }
    };

    // ========== Act ==========
    InitialStepsForPurchaseCoches_UIT();

    // UC1_3 – Filtrar coches
    selectCars.SearchCars("Rojo", "R8");

    // Añadir coche al carrito
    selectCars.AddFirstCarToCart();

    // Finalizar → CreatePurchase
    selectCars.FinishPurchase();

    // Rellenar datos de la compra
    createPurchase.FillInPurchaseInfo(name, surname, address, paymentMethod);

    // Confirmar compra
    createPurchase.PressBuyCars();
    createPurchase.PressOkModalDialog();

    // ========== Assert ==========
    Assert.True(
        detailPurchase.CheckPurchaseDetail(
            fullName,
            address,
            paymentMethod,
            DateTime.Now,
            "70000 €"
        ),
        "Error: los datos generales de la compra no son correctos"
    );

    Assert.True(
        detailPurchase.CheckListOfPurchasedCars(expectedCars),
        "Error: los coches comprados no son correctos"
    );
}

         */

    }
}
