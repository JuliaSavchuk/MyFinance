namespace MyFinance.Models
{
    public class Currency
    {
        public int Id { get; set; } // Унікальний ідентифікатор
        public string Code { get; set; } // Код валюти (наприклад, "USD")
        public decimal Rate { get; set; } // Курс відносно базової валюти (наприклад, до USD)
    }
}