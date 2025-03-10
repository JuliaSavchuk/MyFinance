using System.Collections.Generic;
namespace MyFinance.Models
{
    public class Budget
    {
        public int Id { get; set; } // Унікальний ідентифікатор
        public int UserId { get; set; } // Зовнішній ключ до користувача
        public string Name { get; set; } // Назва бюджету (наприклад, "Щомісячний")
        public decimal Limit { get; set; } // Ліміт витрат
        public decimal TotalIncome { get; set; } // Загальний дохід
        public decimal TotalExpense { get; set; } // Загальні витрати
        public ICollection<Transaction> Transactions { get; set; } // Колекція транзакцій у бюджеті
        public decimal Balance { get; set; } // Баланс користувача
    }
}