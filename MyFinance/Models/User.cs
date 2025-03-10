using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFinance.Models
{
    public class User
    {
        public int Id { get; set; }//Унікальний ідентифікатор.
        public string Username { get; set; }//Ім’я користувача.
        public string PasswordHash { get; set; }//Хеш пароля для безпечного зберігання.
        public string Currency { get; set; }//Вибрана валюта (наприклад, "UAH").
        public ICollection<Budget> Budgets { get; set; }//Колекція бюджетів, пов’язаних із користувачем.

    }
}