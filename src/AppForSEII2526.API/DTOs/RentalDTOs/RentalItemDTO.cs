
// DTO que representa un coche dentro de un alquiler
// Cada RentalItem describe un modelo concreto, con su precio y cuantas unidades se alquilan

public class RentalItemDTO
{
    public int Id { get; set; }
    public int ModelId { get; set; }
    public string ModelName { get; set; }
    public string Manufacturer { get; set; }
    public decimal RentingPrice { get; set; }
    public int Quantity { get; set; }

    //Constructor vacío
    public RentalItemDTO() { }  

    //Constructor principal usado al crear DTOs
    public RentalItemDTO(int id, int modelId, string modelName, string manufacturer, decimal rentingPrice, int quantity)
    {
        Id = id;
        ModelId = modelId;
        ModelName = modelName;
        Manufacturer = manufacturer;
        RentingPrice = rentingPrice;
        Quantity = quantity;
    }

    //Equals y GetHashCode -> Permite comparar objetos DTO directamente
    public override bool Equals(object? obj)
    {
        return obj is RentalItemDTO dto &&
               Id == dto.Id &&
               ModelId == dto.ModelId &&
               ModelName == dto.ModelName &&
               Manufacturer == dto.Manufacturer &&
               RentingPrice == dto.RentingPrice &&
               Quantity == dto.Quantity;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ModelId, ModelName, Manufacturer, RentingPrice, Quantity);
    }
}