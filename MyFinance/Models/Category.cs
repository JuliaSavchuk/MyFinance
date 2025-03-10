using System.Collections.Generic;

namespace MyFinance.Models
{
    public class Category
    {
        public int Id { get; set; } // Унікальний ідентифікатор
        public string Name { get; set; } // Назва категорії (наприклад, "Продукти")
        public string Color { get; set; } // Колір для відображення (наприклад, "#FF5733")
        public ICollection<Transaction> Transactions { get; set; } // Колекція транзакцій у категорії
    }
}