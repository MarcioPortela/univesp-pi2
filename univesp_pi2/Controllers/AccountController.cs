using Microsoft.AspNetCore.Mvc;
using univesp_pi2.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using univesp_pi2.Helpers;
using System.Linq;

namespace univesp_pi2.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName)) { return RedirectToAction("Index", "Login"); }
            var viewModel = GetMockAccountData(userName);
            return View(viewModel);
        }

        public IActionResult Register()
{
    // Retorna a view de cadastro (Register.cshtml)
    return View();
}

[HttpPost]
public IActionResult Register(RegisterViewModel model)
{
    if (ModelState.IsValid)
    {
     
        Console.WriteLine($"Novo usuário registrado: {model.Nome} - {model.Email}");
      
        return RedirectToAction("Index", "Login");
    }

    
    return View(model);
}

        [HttpPost]
        public IActionResult EditProfile([FromBody] ProfileViewModel model)
        {
            if (ModelState.IsValid) { Console.WriteLine($"Salvando perfil para: {model.FullName}"); return Json(new { success = true }); }
            return Json(new { success = false });
        }
        
        [HttpPost]
        public IActionResult EditAddress([FromBody] AddressViewModel model)
        {
            if (model != null) { Console.WriteLine($"Salvando endereço: {model.Title}"); return Json(new { success = true }); }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult DeleteAddress([FromBody] DeleteAddressRequest request)
        {
             if (request != null) { Console.WriteLine($"Excluindo endereço: {request.Title}"); return Json(new { success = true }); }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult DeleteProfile()
        {
            var userName = HttpContext.Session.GetString("UserName");
            Console.WriteLine($"Excluindo perfil do usuário: {userName}");
            HttpContext.Session.Clear(); 
            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
        }

        public class DeleteAddressRequest { public string Title { get; set; } = string.Empty; }
        
       private AccountViewModel GetMockAccountData(string userName)
        {
            bool isAdmin = (userName.ToLower() == "admin");

            var userOrders = HttpContext.Session.Get<List<OrderViewModel>>("UserOrders");

           
            if (userOrders == null || !userOrders.Any())
            {
                userOrders = new List<OrderViewModel> 
                { 
                    new OrderViewModel { OrderId = "PED-001", Date = new DateTime(2025, 10, 1), ItemCount = 2, TotalPrice = 494.70m, Status = "Entregue", Products = new List<string> { "Bateria Automotiva 60Ah", "Pastilha de Freio" } } 
                };
            }

            var model = new AccountViewModel
            {
                Profile = new ProfileViewModel { FullName = isAdmin ? "Administrador" : "João Silva", Email = isAdmin ? "admin@ml3.com" : "joao@exemplo.com", Phone = "(11) 98765-4321", Cpf = "123.456.789-00", BirthDate = "10/05/1990", IsAdmin = isAdmin },
                
               
                Orders = isAdmin ? new List<OrderViewModel>() : userOrders.OrderByDescending(o => o.Date).ToList(),

                Addresses = new List<AddressViewModel> { new AddressViewModel { Title = "Principal", Street = "Rua das Flores", Number = "123", Complement = "Apto 45", Neighborhood = "Centro", City = "São Paulo", State = "SP", Cep = "01234-567", IsPrimary = true }, new AddressViewModel { Title = "Trabalho", Street = "Av. Paulista", Number = "1000", Complement = "Sala 102", Neighborhood = "Bela Vista", City = "São Paulo", State = "SP", Cep = "01310-100", IsPrimary = false } }
            };
            if (isAdmin) { model.SupplierOrders = new List<SupplierOrderViewModel> { new SupplierOrderViewModel { OrderId = "FORN-01", OrderDate = new DateTime(2025, 10, 1), SupplierName = "Bosch Brasil", ItemCount = 50, TotalCost = 15000.00m, Status = "Recebido" } }; }
            return model;
        }
    }
}