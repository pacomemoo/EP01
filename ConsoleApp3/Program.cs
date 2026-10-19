using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MesaDePartesSacoOliveros
{
    // Clase para representar la estructura de un Expediente
    public class Expediente
    {
        public string Codigo { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string TipoDoc { get; set; }
        public string Asunto { get; set; }
        public int Folios { get; set; }
        public string Fecha { get; set; }

        public Expediente(string codigo, string dni, string nombre, string tipoDoc, string asunto, int folios, string fecha)
        {
            Codigo = codigo;
            Dni = dni;
            Nombre = nombre;
            TipoDoc = tipoDoc;
            Asunto = asunto;
            Folios = folios;
            Fecha = fecha;
        }

        public string ALineaArchivo()
        {
            return $"{Codigo}|{Dni}|{Nombre}|{TipoDoc}|{Asunto}|{Folios}|{Fecha}";
        }

        public void MostrarDetalle()
        {
            Console.WriteLine($"Código   : {Codigo}");
            Console.WriteLine($"DNI      : {Dni}");
            Console.WriteLine($"Remitente: {Nombre}");
            Console.WriteLine($"Tipo     : {TipoDoc} | Folios: {Folios}");
            Console.WriteLine($"Asunto   : {Asunto}");
            Console.WriteLine($"Fecha    : {Fecha}");
            Console.WriteLine(new string('-', 80));
        }
    }

    class Program
    {
        private const string ARCHIVO_DATOS = "expedientes_saco_oliveros.txt";
        private static List<Expediente> listaExpedientes = new List<Expediente>();

        static void Main(string[] args)
        {
            CargarDatosDesdeArchivo();
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("        MESA DE PARTES DIGITAL");
                Console.WriteLine("   COLEGIO SACO OLIVEROS - SEDE PRO");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Registrar documento");
                Console.WriteLine("2. Buscar documento");
                Console.WriteLine("3. Mostrar documentos registrados");
                Console.WriteLine("4. Ordenar documentos");
                Console.WriteLine("5. Salir");
                Console.Write("\nSeleccione una opción (1-5): ");

                string opcion = Console.ReadLine()?.Trim();

                switch (opcion)
                {
                    case "1":
                        RegistrarExpediente();
                        break;
                    case "2":
                        BuscarExpediente();
                        break;
                    case "3":
                        MostrarExpedientes();
                        break;
                    case "4":
                        OrdenarExpedientes();
                        break;
                    case "5":
                        salir = true;
                        Console.WriteLine("\nCerrando el sistema de Mesa de Partes Digital. ¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("\n[ERROR] Opción inválida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void CargarDatosDesdeArchivo()
        {
            if (!File.Exists(ARCHIVO_DATOS)) return;

            string[] lineas = File.ReadAllLines(ARCHIVO_DATOS);
            foreach (string linea in lineas)
            {
                string[] datos = linea.Split('|');
                if (datos.Length == 7 && int.TryParse(datos[5], out int folios))
                {
                    Expediente exp = new Expediente(datos[0], datos[1], datos[2], datos[3], datos[4], folios, datos[6]);
                    listaExpedientes.Add(exp);
                }
            }
        }

        private static void GuardarExpedienteEnArchivo(Expediente exp)
        {
            using (StreamWriter sw = new StreamWriter(ARCHIVO_DATOS, true))
            {
                sw.WriteLine(exp.ALineaArchivo());
            }
        }

        private static void RegistrarExpediente()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRO DE NUEVO EXPEDIENTE ---\n");

            string dni;
            do
            {
                Console.Write("Ingrese DNI del remitente (8 dígitos): ");
                dni = Console.ReadLine()?.Trim();
            } while (string.IsNullOrEmpty(dni) || dni.Length != 8 || !dni.All(char.IsDigit));

            Console.Write("Ingrese Nombres y Apellidos completos: ");
            string nombre = Console.ReadLine()?.Trim();

            Console.Write("Tipo de documento (Solicitud / Carta / Reclamo / Informe): ");
            string tipo = Console.ReadLine()?.Trim();

            Console.Write("Asunto o descripción breve: ");
            string asunto = Console.ReadLine()?.Trim();

            int numFolios;
            while (true)
            {
                Console.Write("Número de folios (hojas): ");
                if (int.TryParse(Console.ReadLine(), out numFolios) && numFolios > 0)
                    break;
                Console.WriteLine("El número de folios debe ser un número entero mayor a 0.");
            }

            string codigo = $"EXP-{(listaExpedientes.Count + 1):D4}";
            string fechaActual = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            Expediente nuevoExp = new Expediente(codigo, dni, nombre, tipo, asunto, numFolios, fechaActual);

            listaExpedientes.Add(nuevoExp);
            GuardarExpedienteEnArchivo(nuevoExp);

            Console.WriteLine($"\n[ÉXITO] Documento registrado correctamente con el código: {codigo}");
            Pausar();
        }

        
        private static void BuscarExpediente()
        {
            Console.Clear();
            Console.WriteLine("--- BÚSQUEDA DE EXPEDIENTES ---\n");

            if (listaExpedientes.Count == 0)
            {
                Console.WriteLine("No hay expedientes registrados en el sistema.");
                Pausar();
                return;
            }

            Console.Write("Ingrese Código del expediente (ej. EXP-0001) o DNI del remitente: ");
            string criterio = Console.ReadLine()?.Trim().ToUpper();

            bool encontrado = false;
            Console.WriteLine("\nResultados encontrados:");
            Console.WriteLine(new string('-', 80));

            // Búsqueda Secuencial
            for (int i = 0; i < listaExpedientes.Count; i++)
            {
                if (listaExpedientes[i].Codigo.ToUpper() == criterio ||
                    listaExpedientes[i].Dni == criterio)
                {
                    listaExpedientes[i].MostrarDetalle();
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("No se encontró ningún expediente con el código o DNI ingresado.");
            }

            Pausar();
        }


        

        private static void MostrarExpedientes()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO DE DOCUMENTOS REGISTRADOS ---\n");

            if (listaExpedientes.Count == 0)
            {
                Console.WriteLine("El sistema no contiene expedientes registrados.");
                Pausar();
                return;
            }

            Console.WriteLine($"{"CÓDIGO",-10} | {"DNI",-10} | {"REMITENTE",-25} | {"TIPO",-12} | {"FOLIOS",-6} | {"FECHA"}");
            Console.WriteLine(new string('=', 80));

            foreach (var exp in listaExpedientes)
            {
                string nombreCorto = exp.Nombre.Length > 24 ? exp.Nombre.Substring(0, 24) : exp.Nombre;
                Console.WriteLine($"{exp.Codigo,-10} | {exp.Dni,-10} | {nombreCorto,-25} | {exp.TipoDoc,-12} | {exp.Folios,-6} | {exp.Fecha}");
            }

            Pausar();
        }

        private static void OrdenarExpedientes()
        {
            Console.Clear();
            Console.WriteLine("--- ORDENAMIENTO DE EXPEDIENTES ---\n");

            int n = listaExpedientes.Count;
            if (n <= 1)
            {
                Console.WriteLine("No hay suficientes registros para ordenar.");
                Pausar();
                return;
            }

            Console.WriteLine("1. Ordenar por Código (Ascendente)");
            Console.WriteLine("2. Ordenar por Nombre de Remitente (A-Z)");
            Console.Write("Seleccione el criterio de ordenamiento: ");
            string opc = Console.ReadLine()?.Trim();

            // Ordenamiento Burbuja (Bubble Sort)
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    bool intercambiar = false;

                    if (opc == "1" && string.Compare(listaExpedientes[j].Codigo, listaExpedientes[j + 1].Codigo, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        intercambiar = true;
                    }
                    else if (opc == "2" && string.Compare(listaExpedientes[j].Nombre, listaExpedientes[j + 1].Nombre, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        intercambiar = true;
                    }

                    if (intercambiar)
                    {
                        Expediente temp = listaExpedientes[j];
                        listaExpedientes[j] = listaExpedientes[j + 1];
                        listaExpedientes[j + 1] = temp;
                    }
                }
            }

            Console.WriteLine("\n[ÉXITO] Los registros han sido ordenados correctamente.");
            Pausar();
            MostrarExpedientes();
        }

        private static void Pausar()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
