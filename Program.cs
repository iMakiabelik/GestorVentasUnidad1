using System;
using System.Collections.Generic;
using System.Globalization;

namespace GestorVentasUnidad1
{
    internal class Program
    {
        // Listas en memoria (Sin POO según la guía de la Unidad 1)
        static List<string> nombresProductos = new List<string>();
        static List<decimal> preciosProductos = new List<decimal>();
        static List<int> stocksProductos = new List<int>();
        static List<int> ventasPorProducto = new List<int>();

        // Variables de caja
        static int totalVentasRealizadas = 0;
        static decimal totalDineroCaja = 0m;

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("es-CO");

            int opcion;
            do
            {
                Console.Clear();
                ImprimirEncabezado("SISTEMA GESTOR DE VENTAS E INVENTARIO (MINI-POS)");
                Console.WriteLine(" 1. Registrar nuevo producto en inventario");
                Console.WriteLine(" 2. Consultar inventario completo");
                Console.WriteLine(" 3. Registrar una venta");
                Console.WriteLine(" 4. Ver reporte de caja y estadísticas diarias");
                Console.WriteLine(" 5. Salir");
                Console.WriteLine("====================================================");

                opcion = LeerEntero("Seleccione una opción (1-5): ", 1, 5);

                switch (opcion)
                {
                    case 1:
                        RegistrarNuevoProducto();
                        break;
                    case 2:
                        ConsultarInventario();
                        break;
                    case 3:
                        RegistrarVenta();
                        break;
                    case 4:
                        VerReporteCaja();
                        break;
                    case 5:
                        Console.Clear();
                        ImprimirEncabezado("¡GRACIAS POR USAR EL SISTEMA MINI-POS!");
                        Console.WriteLine("Saliendo del programa...");
                        break;
                }

                if (opcion != 5)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }

            } while (opcion != 5);
        }

        #region Métodos Obligatorios

        static void ImprimirEncabezado(string titulo)
        {
            Console.WriteLine("====================================================");
            int espacios = Math.Max(0, (50 - titulo.Length) / 2);
            Console.WriteLine($"{new string(' ', espacios)}{titulo}");
            Console.WriteLine("====================================================");
        }

        static int LeerEntero(string mensaje, int min, int max)
        {
            int valor;
            while (true)
            {
                Console.Write(mensaje);
                string entrada = Console.ReadLine();

                if (!int.TryParse(entrada, out valor))
                {
                    Console.WriteLine("[ERROR] Entrada no válida. Debe ingresar un número entero.");
                }
                else if (valor < min || valor > max)
                {
                    Console.WriteLine($"[ERROR] Opción fuera de rango. Ingrese un valor entre {min} y {max}.");
                }
                else
                {
                    return valor;
                }
            }
        }

        static decimal LeerDecimal(string mensaje, decimal min)
        {
            decimal valor;
            while (true)
            {
                Console.Write(mensaje);
                string entrada = Console.ReadLine();

                if (!decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.CurrentCulture, out valor) &&
                    !decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                {
                    Console.WriteLine("[ERROR] Entrada no válida. Debe ingresar un número decimal válido.");
                }
                else if (valor < min)
                {
                    Console.WriteLine($"[ERROR] El valor debe ser mayor o igual a {min:C}.");
                }
                else
                {
                    return valor;
                }
            }
        }

        static decimal CalcularFactura(decimal precio, int cantidad, bool tieneDescuento, out decimal montoIva, out decimal montoDescuento)
        {
            decimal subtotal = precio * cantidad;
            montoDescuento = tieneDescuento ? subtotal * 0.10m : 0m;
            decimal baseImponible = subtotal - montoDescuento;
            montoIva = baseImponible * 0.19m;
            decimal totalPagar = baseImponible + montoIva;

            return totalPagar;
        }

        #endregion

        #region Funciones del Sistema

        static void RegistrarNuevoProducto()
        {
            Console.Clear();
            ImprimirEncabezado("REGISTRAR NUEVO PRODUCTO");

            string nombre;
            while (true)
            {
                Console.Write("Ingrese el nombre del producto: ");
                nombre = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("[ERROR] El nombre del producto no puede estar vacío.");
                    continue;
                }

                bool yaExiste = false;
                foreach (string p in nombresProductos)
                {
                    if (p.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        yaExiste = true;
                        break;
                    }
                }

                if (yaExiste)
                {
                    Console.WriteLine($"[ERROR] Ya existe un producto registrado con el nombre '{nombre}'. Ingrese uno diferente.");
                }
                else
                {
                    break;
                }
            }

            decimal precio = LeerDecimal("Ingrese el precio unitario ($): ", 0.01m);
            int stock = LeerEntero("Ingrese el stock inicial: ", 0, int.MaxValue);

            nombresProductos.Add(nombre);
            preciosProductos.Add(precio);
            stocksProductos.Add(stock);
            ventasPorProducto.Add(0);

            Console.WriteLine($"\n[OK] Producto '{nombre}' registrado con éxito.");
        }

        static void ConsultarInventario()
        {
            Console.Clear();
            ImprimirEncabezado("INVENTARIO COMPLETO");

            if (nombresProductos.Count == 0)
            {
                Console.WriteLine("No hay productos registrados en el inventario actualmente.");
                return;
            }

            Console.WriteLine("{0,-5} | {1,-30} | {2,-15} | {3,-10} | {4}", "ID", "Nombre", "Precio", "Stock", "Alerta");
            Console.WriteLine(new string('-', 75));

            for (int i = 0; i < nombresProductos.Count; i++)
            {
                string alerta = stocksProductos[i] < 5 ? "[ALERTA: BAJO STOCK]" : "";
                Console.WriteLine("{0,-5} | {1,-30} | {2,-15:C} | {3,-10} | {4}",
                    (i + 1),
                    nombresProductos[i],
                    preciosProductos[i],
                    stocksProductos[i],
                    alerta);
            }
        }

        static void RegistrarVenta()
        {
            Console.Clear();
            ImprimirEncabezado("REGISTRAR VENTA");

            if (nombresProductos.Count == 0)
            {
                Console.WriteLine("[ERROR] No hay productos registrados para realizar ventas.");
                return;
            }

            for (int i = 0; i < nombresProductos.Count; i++)
            {
                string alerta = stocksProductos[i] < 5 ? "[ALERTA: BAJO STOCK]" : "";
                Console.WriteLine($"{i + 1}. {nombresProductos[i],-25} | Precio: {preciosProductos[i],12:C} | Stock: {stocksProductos[i]} {alerta}");
            }
            Console.WriteLine();

            int seleccion = LeerEntero($"Seleccione el número del producto a vender (1-{nombresProductos.Count}): ", 1, nombresProductos.Count);
            int indice = seleccion - 1;

            if (stocksProductos[indice] <= 0)
            {
                Console.WriteLine($"\n[ERROR] El producto '{nombresProductos[indice]}' no tiene unidades disponibles en stock.");
                return;
            }

            int cantidad;
            while (true)
            {
                cantidad = LeerEntero("Ingrese la cantidad a comprar: ", 1, int.MaxValue);

                if (cantidad > stocksProductos[indice])
                {
                    Console.WriteLine($"[ERROR] Stock insuficiente. Solo quedan {stocksProductos[indice]} unidades en inventario.");
                }
                else
                {
                    break;
                }
            }

            bool tieneDescuento = false;
            while (true)
            {
                Console.Write("¿Aplica descuento de cliente frecuente (10%)? (S/N): ");
                string respuesta = Console.ReadLine()?.Trim().ToUpper();

                if (respuesta == "S")
                {
                    tieneDescuento = true;
                    break;
                }
                else if (respuesta == "N")
                {
                    tieneDescuento = false;
                    break;
                }
                else
                {
                    Console.WriteLine("[ERROR] Ingrese 'S' para Sí o 'N' para No.");
                }
            }

            decimal montoIva, montoDescuento;
            decimal subtotal = preciosProductos[indice] * cantidad;
            decimal totalPagar = CalcularFactura(preciosProductos[indice], cantidad, tieneDescuento, out montoIva, out montoDescuento);

            stocksProductos[indice] -= cantidad;
            ventasPorProducto[indice] += cantidad;
            totalVentasRealizadas++;
            totalDineroCaja += totalPagar;

            Console.WriteLine();
            ImprimirEncabezado("TICKET DE VENTA");
            Console.WriteLine($" Producto:             {nombresProductos[indice]} (x{cantidad})");
            Console.WriteLine($" Subtotal:             {subtotal,14:C}");
            Console.WriteLine($" Descuento (10%):     -{montoDescuento,14:C}");
            Console.WriteLine($" IVA (19%):            +{montoIva,14:C}");
            Console.WriteLine(" ---------------------------------------------------");
            Console.WriteLine($" TOTAL A PAGAR:        {totalPagar,14:C}");
            Console.WriteLine("====================================================");
            Console.WriteLine($"[OK] Venta efectuada con éxito. Stock actualizado: {stocksProductos[indice]} unidades.");
        }

        static void VerReporteCaja()
        {
            Console.Clear();
            ImprimirEncabezado("REPORTE DE CAJA Y ESTADÍSTICAS DIARIAS");

            Console.WriteLine($"Total de ventas realizadas:      {totalVentasRealizadas}");
            Console.WriteLine($"Total dinero en caja acumulado:  {totalDineroCaja:C}");

            decimal promedio = totalVentasRealizadas > 0 ? totalDineroCaja / totalVentasRealizadas : 0m;
            Console.WriteLine($"Promedio de dinero por venta:    {promedio:C}");

            if (totalVentasRealizadas == 0)
            {
                Console.WriteLine("Producto más vendido:            N/A (Sin ventas registradas)");
            }
            else
            {
                int maxVendidas = -1;
                string productoMasVendido = "";

                for (int i = 0; i < nombresProductos.Count; i++)
                {
                    if (ventasPorProducto[i] > maxVendidas)
                    {
                        maxVendidas = ventasPorProducto[i];
                        productoMasVendido = nombresProductos[i];
                    }
                }

                if (maxVendidas > 0)
                {
                    Console.WriteLine($"Producto más vendido:            {productoMasVendido} ({maxVendidas} unidades)");
                }
                else
                {
                    Console.WriteLine("Producto más vendido:            Sin registros de unidades vendidas.");
                }
            }

            Console.WriteLine("====================================================");
        }

        #endregion
    }
}
