using System;

namespace MyFinance.Models
{
    public enum TransactionType { Income, Expense }
    public class Transaction
    {
        public int Id { get; set; } // Унікальний ідентифікатор
        public int BudgetId { get; set; } // Зовнішній ключ до бюджету
        public int CategoryId { get; set; } // Зовнішній ключ до категорії
        public string Description { get; set; } // Опис транзакції
        public decimal Amount { get; set; } // Сума транзакції
        public TransactionType Type { get; set; } // Тип транзакції (Income – дохід, Expense – витрата)
        public DateTime Date { get; set; } // Дата транзакції
        public Category Category { get; set; } // Категорія транзакції
    }
}




