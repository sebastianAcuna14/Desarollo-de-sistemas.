using ClosedXML.Excel;
using System.IO;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using prototipo2.Models;
using System.Net.Mail;
using System.Net;

namespace prototipo2.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IConfiguration _configuration;

        public PedidoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var lista = context.Query<Pedido>("ObtenerPedido", commandType: CommandType.StoredProcedure).ToList();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pedido pedido)
        {
            if (!ModelState.IsValid)
                return View(pedido);

            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = context.Execute("Crear_Pedido", new
            {
                pedido.Nombre_Producto,
                pedido.Numero_Pedido,
                pedido.Cantidad,
                pedido.FechaPedido,
                pedido.Precio,
                pedido.Estado,
                pedido.CorreoCliente
            }, commandType: CommandType.StoredProcedure);

            if (resultado > 0)
            {
                // Enviar correo si el estado es "En camino" o "Enviado"
                if (!string.IsNullOrEmpty(pedido.CorreoCliente) &&
                    (pedido.Estado == "En camino" || pedido.Estado == "Enviado"))
                {
                    Console.WriteLine("Intentando enviar correo tras creación...");
                    EnviarCorreoEstado(pedido);
                }

                return RedirectToAction("Index");
            }

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoEnCamino(int id)
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));
            context.Execute("MarcarPedidoComoEnCamino", new { Id = id }, commandType: CommandType.StoredProcedure);

            var pedido = context.QueryFirstOrDefault<Pedido>("SELECT * FROM Pedido WHERE Id = @Id", new { Id = id });

            if (pedido != null && pedido.Estado == "En camino" && !string.IsNullOrEmpty(pedido.CorreoCliente))
            {
                Console.WriteLine("Intentando enviar correo al marcar como En camino...");
                EnviarCorreoEstado(pedido);
            }

            return RedirectToAction("Index");
        }

        public IActionResult ExportarExcel()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var pedidos = context.Query<Pedido>("ObtenerPedido", commandType: CommandType.StoredProcedure).ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Pedidos");

                    // Encabezados
                    worksheet.Cell(1, 1).Value = "Número de Pedido";
                    worksheet.Cell(1, 2).Value = "Nombre del Producto";
                    worksheet.Cell(1, 3).Value = "Cantidad";
                    worksheet.Cell(1, 4).Value = "Precio";
                    worksheet.Cell(1, 5).Value = "Fecha del Pedido";
                    worksheet.Cell(1, 6).Value = "Estado";

                    int fila = 2;

                    foreach (var pedido in pedidos)
                    {
                        worksheet.Cell(fila, 1).Value = pedido.Numero_Pedido;
                        worksheet.Cell(fila, 2).Value = pedido.Nombre_Producto;
                        worksheet.Cell(fila, 3).Value = pedido.Cantidad;
                        worksheet.Cell(fila, 4).Value = pedido.Precio;
                        worksheet.Cell(fila, 5).Value = pedido.FechaPedido.ToShortDateString();
                        worksheet.Cell(fila, 6).Value = pedido.Estado;
                        fila++;
                    }

                    // Opcional: ajustar tamaño de las columnas
                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ListadoPedidos.xlsx");
                    }


                }
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoEnviado(int id)
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));
            context.Execute("MarcarPedidoComoEnviado", new { Id = id }, commandType: CommandType.StoredProcedure);

            var pedido = context.QueryFirstOrDefault<Pedido>("SELECT * FROM Pedido WHERE Id = @Id", new { Id = id });

            if (pedido != null && pedido.Estado == "Enviado" && !string.IsNullOrEmpty(pedido.CorreoCliente))
            {
                Console.WriteLine("Intentando enviar correo al marcar como Enviado...");
                EnviarCorreoEstado(pedido);
            }

            return RedirectToAction("Index");
        }





        private void EnviarCorreoEstado(Pedido pedido)
        {
            try
            {
                var smtpCorreo = _configuration["SMTP:CorreoSalida"];
                var smtpClave = _configuration["SMTP:ClaveCorreoSalida"];

                Console.WriteLine($"Preparando correo desde {smtpCorreo} a {pedido.CorreoCliente}");

                var fromAddress = new MailAddress(smtpCorreo, "Sistema de Pedidos");
                var toAddress = new MailAddress(pedido.CorreoCliente);
                string subject = "Estado de tu pedido";
                string body = $"Tu pedido número {pedido.Numero_Pedido} ahora está: {pedido.Estado}.";

                using var smtp = new SmtpClient
                {
                    Host = "smtp.office365.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpCorreo, smtpClave)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };

                smtp.Send(message);

                Console.WriteLine("Correo enviado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo: " + ex.ToString());
            }
        }

        // Resto del controlador (Editar, Eliminar, Consultar) igual que antes, sin cambio necesario aquí.
    }
}