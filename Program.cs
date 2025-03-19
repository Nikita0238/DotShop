using System;
using System.Collections.Generic;

namespace OnlineStore
{
    // Перечисление для статусов заказа
    public enum OrderStatus
    {
        Pending,
        Completed,
        Returned
    }

    // Класс для категорий товаров с иерархической структурой
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category ParentCategory { get; set; }
        public List<Category> SubCategories { get; set; } = new List<Category>();

        public Category(int id, string name, Category parent = null)
        {
            Id = id;
            Name = name;
            ParentCategory = parent;
            parent?.SubCategories.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    // Класс для товаров
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }

        public Product(int id, string name, decimal price, Category category)
        {
            Id = id;
            Name = name;
            Price = price;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Name} - ${Price}";
        }
    }

    // Базовый класс пользователя с функционалом авторизации
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Для реального проекта используйте шифрование
        public string Role { get; set; } // Например: "Customer", "Administrator", "Executor"

        public User(int id, string username, string password, string role)
        {
            Id = id;
            Username = username;
            Password = password;
            Role = role;
        }

        // Простейшая проверка пароля
        public bool Authenticate(string password)
        {
            return Password == password;
        }

        public override string ToString()
        {
            return Username;
        }
    }

    // Класс для элемента корзины
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Product.Name} x {Quantity}";
        }
    }

    // Класс корзины покупателя
    public class ShoppingCart
    {
        public User Customer { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public ShoppingCart(User customer)
        {
            Customer = customer;
        }

        public void AddProduct(Product product, int quantity)
        {
            Items.Add(new CartItem(product, quantity));
        }

        public decimal GetTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Product.Price * item.Quantity;
            }
            return total;
        }

        public override string ToString()
        {
            string details = "";
            foreach (var item in Items)
            {
                details += item.ToString() + "\n";
            }
            details += $"Total: ${GetTotal()}";
            return details;
        }
    }

    // Класс для заказа
    public class Order
    {
        public int Id { get; set; }
        public User Customer { get; set; }
        public List<CartItem> OrderItems { get; set; }
        public OrderStatus Status { get; set; }
        public User Executor { get; set; } // Ответственный исполнитель

        public Order(int id, User customer, List<CartItem> orderItems)
        {
            Id = id;
            Customer = customer;
            OrderItems = new List<CartItem>(orderItems);
            Status = OrderStatus.Pending;
        }

        public override string ToString()
        {
            string details = $"Order #{Id} for {Customer.Username}:\n";
            foreach (var item in OrderItems)
            {
                details += item.ToString() + "\n";
            }
            details += $"Status: {Status}";
            if (Executor != null)
                details += $", Executor: {Executor.Username}";
            return details;
        }
    }

    // Класс администратора с дополнительным функционалом
    public class Administrator : User
    {
        public Administrator(int id, string username, string password)
            : base(id, username, password, "Administrator")
        {
        }

        // Назначение исполнителя для заказа
        public void AssignOrder(Order order, User executor)
        {
            if (executor.Role == "Executor")
            {
                order.Executor = executor;
                Console.WriteLine($"Order #{order.Id} assigned to executor {executor.Username}.");
            }
            else
            {
                Console.WriteLine($"{executor.Username} не имеет роль Executor.");
            }
        }

        // Обновление статуса заказа
        public void UpdateOrderStatus(Order order, OrderStatus newStatus)
        {
            order.Status = newStatus;
            Console.WriteLine($"Order #{order.Id} status updated to {newStatus}.");
        }
    }

    // Пример использования системы
    class Program
    {
        static void Main(string[] args)
        {
            // Создание категорий
            Category electronics = new Category(1, "Electronics");
            Category computers = new Category(2, "Computers", electronics);
            Category smartphones = new Category(3, "Smartphones", electronics);

            // Создание товаров
            Product laptop = new Product(1, "Gaming Laptop", 1500.00m, computers);
            Product phone = new Product(2, "Smartphone", 800.00m, smartphones);

            // Создание пользователей
            User customer = new User(1, "john_doe", "password123", "Customer");
            Administrator admin = new Administrator(2, "admin", "adminpass");
            User executor = new User(3, "executor1", "execpass", "Executor");

            // Пример авторизации клиента
            if (customer.Authenticate("password123"))
            {
                Console.WriteLine($"{customer.Username} успешно авторизован.");
            }
            else
            {
                Console.WriteLine("Ошибка авторизации.");
                return;
            }

            // Работа с корзиной: добавление товаров
            ShoppingCart cart = new ShoppingCart(customer);
            cart.AddProduct(laptop, 1);
            cart.AddProduct(phone, 2);
            Console.WriteLine("\nShopping Cart:");
            Console.WriteLine(cart.ToString());

            // Создание заказа из содержимого корзины
            Order order = new Order(1, customer, cart.Items);
            Console.WriteLine("\nСоздан заказ:");
            Console.WriteLine(order.ToString());

            // Действия администратора: назначение исполнителя и обновление статуса заказа
            admin.AssignOrder(order, executor);
            admin.UpdateOrderStatus(order, OrderStatus.Completed);

            Console.WriteLine("\nИтоговые данные заказа:");
            Console.WriteLine(order.ToString());

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
