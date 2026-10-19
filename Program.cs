using System;
using System.Collections.Generic;
using System.Globalization;

namespace ConsoleApp1
{
    public class Producto
    {
        public string Nombre { get; private set; }
        public decimal Precio { get; private set; }
        public int StockActual { get; private set; }
        public int StockMinimo { get; private set; }
        public DateTime FechaVencimiento { get; private set; }

        public Producto(
            string nombre,
            decimal precio,
            int stockActual,
            int stockMinimo,
            DateTime fechaVencimiento)
        {
            Nombre = nombre;
            Precio = precio;
            StockActual = stockActual;
            StockMinimo = stockMinimo;
            FechaVencimiento = fechaVencimiento;
        }

        public void ReducirStock(int cantidad)
        {
            StockActual -= cantidad;
        }
    }

    internal class Program
    {
        private static readonly Dictionary<string, Producto> Productos =
            new Dictionary<string, Producto>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "Agua San Mateo 600ml",
                    new Producto(
                        "Agua San Mateo 600ml",
                        2.50m,
                        50,
                        10,
                        new DateTime(2027, 12, 31))
                },
                {
                    "Galleta Field Soda",
                    new Producto(
                        "Galleta Field Soda",
                        1.20m,
                        20,
                        5,
                        new DateTime(2027, 1, 1))
                },
                {
                    "Gaseosa Inca Kola 500ml",
                    new Producto(
                        "Gaseosa Inca Kola 500ml",
                        3.50m,
                        15,
                        5,
                        new DateTime(2027, 6, 30))
                }
            };

        private static void Main(string[] args)
        {
            EjecutarSistema();
        }

        private static void EjecutarSistema()
        {
            bool continuar = true;

            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("==============================================");
                Console.WriteLine("       SISTEMA POS - REGISTRO DE VENTAS");
                Console.WriteLine("==============================================");
                Console.WriteLine("1. Registrar venta");
                Console.WriteLine("2. Mostrar inventario");
                Console.WriteLine("3. Salir");
                Console.WriteLine();

                Console.Write("Seleccione una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        RegistrarVenta();
                        break;

                    case "2":
                        MostrarInventario();
                        break;

                    case "3":
                        continuar = false;
                        Console.WriteLine("Programa finalizado.");
                        break;

                    default:
                        Console.WriteLine("Opción no válida.");
                        Pausar();
                        break;
                }
            }
        }

        private static void RegistrarVenta()
        {
            decimal total = 0m;
            bool agregarProductos = true;

            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("              REGISTRAR VENTA");
            Console.WriteLine("==============================================");

            while (agregarProductos)
            {
                Console.WriteLine();
                Console.Write("Ingrese el nombre del producto: ");

                string nombreProducto =
                    (Console.ReadLine() ?? string.Empty).Trim();

                Producto producto;

                if (!Productos.TryGetValue(nombreProducto, out producto))
                {
                    Console.WriteLine("ERROR: El producto no está registrado.");
                    Console.WriteLine("Consulte la opción 'Mostrar inventario'.");
                    continue;
                }

                Console.WriteLine("Producto: " + producto.Nombre);
                Console.WriteLine(
                    "Precio: S/ " +
                    producto.Precio.ToString(
                        "F2",
                        CultureInfo.InvariantCulture));
                Console.WriteLine("Stock disponible: " + producto.StockActual);

                Console.Write("Ingrese la cantidad: ");
                int cantidad;

                if (!int.TryParse(Console.ReadLine(), out cantidad) ||
                    cantidad <= 0)
                {
                    Console.WriteLine(
                        "ERROR: Ingrese una cantidad válida mayor que cero.");
                    continue;
                }

                if (!ProductoDisponible(producto, cantidad))
                {
                    continue;
                }

                decimal subtotal = producto.Precio * cantidad;
                total += subtotal;
                producto.ReducirStock(cantidad);

                Console.WriteLine(
                    "Producto agregado. Subtotal: S/ {0}",
                    subtotal.ToString(
                        "F2",
                        CultureInfo.InvariantCulture));

                if (producto.StockActual <= producto.StockMinimo)
                {
                    Console.WriteLine(
                        "AVISO: El stock está en el nivel mínimo o por debajo.");
                }

                Console.WriteLine();
                Console.Write("¿Desea agregar otro producto? (S/N): ");

                string respuesta =
                    (Console.ReadLine() ?? string.Empty).Trim();

                agregarProductos = respuesta.Equals(
                    "S",
                    StringComparison.OrdinalIgnoreCase);
            }

            EmitirComprobante(total);
            Pausar();
        }

        private static bool ProductoDisponible(
            Producto producto,
            int cantidad)
        {
            if (producto.StockActual < cantidad)
            {
                Console.WriteLine("ERROR: Stock insuficiente.");
                return false;
            }

            if (producto.FechaVencimiento.Date < DateTime.Today)
            {
                Console.WriteLine("ERROR: El producto está vencido.");
                return false;
            }

            return true;
        }

        private static void MostrarInventario()
        {
            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("                 INVENTARIO");
            Console.WriteLine("==============================================");

            foreach (Producto producto in Productos.Values)
            {
                Console.WriteLine("Nombre: " + producto.Nombre);
                Console.WriteLine(
                    "Precio: S/ " +
                    producto.Precio.ToString(
                        "F2",
                        CultureInfo.InvariantCulture));
                Console.WriteLine("Stock: " + producto.StockActual);
                Console.WriteLine("Stock mínimo: " + producto.StockMinimo);
                Console.WriteLine(
                    "Vencimiento: " +
                    producto.FechaVencimiento.ToString("dd/MM/yyyy"));

                if (producto.StockActual <= producto.StockMinimo)
                {
                    Console.WriteLine(
                        "Estado: REQUIERE REABASTECIMIENTO");
                }
                else
                {
                    Console.WriteLine("Estado: Disponible");
                }

                Console.WriteLine("----------------------------------------------");
            }

            Pausar();
        }

        private static void EmitirComprobante(decimal total)
        {
            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine("              TICKET DE VENTA");
            Console.WriteLine("==============================================");
            Console.WriteLine(
                "Total a pagar: S/ " +
                total.ToString(
                    "F2",
                    CultureInfo.InvariantCulture));
            Console.WriteLine("==============================================");
            Console.WriteLine("Gracias por su compra.");
        }

        private static void Pausar()
        {
            Console.WriteLine();
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
        }
    }
}
