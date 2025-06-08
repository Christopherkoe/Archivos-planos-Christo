using System;
using System.Collections.Generic;
using System.IO;

namespace ProyectoUsuarios
{
    class Program
    {
        static string archivoUsers = "Users.txt";
        static string archivoLog = "log.txt";
        static string archivoPersonas = "Personas.txt";

        static void Main(string[] args)
        {
            // Login
            if (!Login())
            {
                Console.WriteLine("Acceso denegado. Saliendo...");
                return;
            }
        }

        static bool Login()
        {
            var usuarios = CargarUsuarios();
            int intentos = 0;

            while (intentos < 3)
            {
                Console.Write("Usuario: ");
                string user = Console.ReadLine();
                Console.Write("Contraseña: ");
                string pass = Console.ReadLine();

                Usuario usuario = usuarios.Find(u => u.Nombre == user);

                if (usuario == null)
                {
                    Console.WriteLine("Usuario no encontrado.");
                    intentos++;
                    continue;
                }

                if (!usuario.Activo)
                {
                    Console.WriteLine("Usuario bloqueado.");
                    return false;
                }

                if (usuario.Contrasena == pass)
                {
                    Console.WriteLine("Bienvenido " + user);
                    MostrarMenu(user);
                    return true;
                }
                else
                {
                    intentos++;
                    Console.WriteLine($"Contraseña incorrecta. Intentos: {intentos}/3");
                }
            }

            // Bloquear usuario si falla 3 veces
            Console.WriteLine("Has superado el número de intentos. Usuario bloqueado.");
            BloquearUsuario(usuarios, usuarios.Find(u => u.Activo), archivoUsers);
            return false;
        }

        static List<Usuario> CargarUsuarios()
        {
            List<Usuario> lista = new List<Usuario>();
            if (!File.Exists(archivoUsers)) return lista;

            var lineas = File.ReadAllLines(archivoUsers);
            foreach (var linea in lineas)
            {
                var partes = linea.Split(',');
                if (partes.Length == 3)
                {
                    lista.Add(new Usuario
                    {
                        Nombre = partes[0],
                        Contrasena = partes[1],
                        Activo = bool.Parse(partes[2])
                    });
                }
            }
            return lista;
        }

        static void BloquearUsuario(List<Usuario> usuarios, Usuario usuario, string archivo)
        {
            if (usuario == null) return;

            usuario.Activo = false;

            List<string> lineas = new List<string>();
            foreach (var u in usuarios)
            {
                lineas.Add($"{u.Nombre},{u.Contrasena},{u.Activo}");
            }
            File.WriteAllLines(archivo, lineas);
        }

        static void MostrarMenu(string usuarioActual)
        {
            while (true)
            {
                Console.WriteLine("\n=== MENÚ ===");
                Console.WriteLine("1. Crear persona");
                Console.WriteLine("2. Editar persona");
                Console.WriteLine("3. Eliminar persona");
                Console.WriteLine("4. Mostrar informe");
                Console.WriteLine("5. Salir");
                Console.Write("Selecciona una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        RegistrarLog(usuarioActual, "Entró en opción: Crear persona");
                        CrearPersona(usuarioActual);
                        break;
                    case "2":
                        RegistrarLog(usuarioActual, "Entró en opción: Editar persona");
                        EditarPersona(usuarioActual);
                        break;
                    case "3":
                        RegistrarLog(usuarioActual, "Entró en opción: Eliminar persona");
                        EliminarPersona(usuarioActual);
                        break;
                    case "4":
                        RegistrarLog(usuarioActual, "Entró en opción: Mostrar informe");
                        MostrarInforme(usuarioActual);
                        break;
                    case "5":
                        RegistrarLog(usuarioActual, "Salió del sistema");
                        return;
                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
        }

        static void RegistrarLog(string usuario, string mensaje)
        {
            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | Usuario: {usuario} | {mensaje}";
            File.AppendAllText(archivoLog, log + Environment.NewLine);
        }

        // --- Clase Usuario ---
        class Usuario
        {
            public string Nombre { get; set; }
            public string Contrasena { get; set; }
            public bool Activo { get; set; }
        }

        // --- Clase Persona ---
        class Persona
        {
            public int ID { get; set; }
            public string Nombres { get; set; }
            public string Apellidos { get; set; }
            public string Telefono { get; set; }
            public string Ciudad { get; set; }
            public decimal Saldo { get; set; }
        }

        // --- Personas ---
        static List<Persona> CargarPersonas()
        {
            List<Persona> lista = new List<Persona>();
            if (!File.Exists(archivoPersonas)) return lista;

            var lineas = File.ReadAllLines(archivoPersonas);
            foreach (var linea in lineas)
            {
                var partes = linea.Split('|');
                if (partes.Length == 6)
                {
                    lista.Add(new Persona
                    {
                        ID = int.Parse(partes[0]),
                        Nombres = partes[1],
                        Apellidos = partes[2],
                        Telefono = partes[3],
                        Ciudad = partes[4],
                        Saldo = decimal.Parse(partes[5])
                    });
                }
            }
            return lista;
        }

        static void GuardarPersonas(List<Persona> personas)
        {
            List<string> lineas = new List<string>();
            foreach (var p in personas)
            {
                lineas.Add($"{p.ID}|{p.Nombres}|{p.Apellidos}|{p.Telefono}|{p.Ciudad}|{p.Saldo}");
            }
            File.WriteAllLines(archivoPersonas, lineas);
        }

        static void CrearPersona(string usuarioActual)
        {
            Console.Clear();
            Console.WriteLine("=== Crear Nueva Persona ===");

            List<Persona> personas = CargarPersonas();

            int id;
            while (true)
            {
                Console.Write("ID (número): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out id) && id > 0)
                {
                    if (personas.Exists(p => p.ID == id))
                    {
                        Console.WriteLine("El ID ya existe. Intenta con otro.");
                    }
                    else break;
                }
                else
                {
                    Console.WriteLine("Debe ser un número positivo.");
                }
            }

            Console.Write("Nombres: ");
            string nombres = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nombres))
            {
                Console.WriteLine("Los nombres no pueden estar vacíos.");
                return;
            }

            Console.Write("Apellidos: ");
            string apellidos = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(apellidos))
            {
                Console.WriteLine("Los apellidos no pueden estar vacíos.");
                return;
            }

            Console.Write("Teléfono: ");
            string telefono = Console.ReadLine();
            if (!long.TryParse(telefono, out _) || telefono.Length < 7)
            {
                Console.WriteLine("Teléfono inválido.");
                return;
            }

            Console.Write("Ciudad: ");
            string ciudad = Console.ReadLine();

            Console.Write("Saldo: ");
            string saldoInput = Console.ReadLine();
            if (!decimal.TryParse(saldoInput, out decimal saldo) || saldo < 0)
            {
                Console.WriteLine("El saldo debe ser un número positivo.");
                return;
            }

            Persona nueva = new Persona
            {
                ID = id,
                Nombres = nombres,
                Apellidos = apellidos,
                Telefono = telefono,
                Ciudad = ciudad,
                Saldo = saldo
            };

            personas.Add(nueva);
            GuardarPersonas(personas);

            Console.WriteLine("Persona creada con éxito.");
            RegistrarLog(usuarioActual, $"Creó persona ID {id} - {nombres} {apellidos}");
        }

        static void EditarPersona(string usuarioActual)
        {
            Console.Clear();
            Console.WriteLine("=== Editar Persona ===");

            List<Persona> personas = CargarPersonas();

            Console.Write("Ingrese el ID de la persona a editar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            Persona persona = personas.Find(p => p.ID == id);
            if (persona == null)
            {
                Console.WriteLine("Persona no encontrada.");
                return;
            }

            Console.WriteLine("\nPresiona ENTER para mantener el valor actual.");
            Console.WriteLine($"Nombres actuales: {persona.Nombres}");
            Console.Write("Nuevo nombre: ");
            string nuevoNombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoNombre))
                persona.Nombres = nuevoNombre;

            Console.WriteLine($"Apellidos actuales: {persona.Apellidos}");
            Console.Write("Nuevo apellido: ");
            string nuevoApellido = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoApellido))
                persona.Apellidos = nuevoApellido;

            Console.WriteLine($"Teléfono actual: {persona.Telefono}");
            Console.Write("Nuevo teléfono: ");
            string nuevoTelefono = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoTelefono))
            {
                if (!long.TryParse(nuevoTelefono, out _) || nuevoTelefono.Length < 7)
                {
                    Console.WriteLine("Teléfono inválido.");
                    return;
                }
                persona.Telefono = nuevoTelefono;
            }

            Console.WriteLine($"Ciudad actual: {persona.Ciudad}");
            Console.Write("Nueva ciudad: ");
            string nuevaCiudad = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaCiudad))
                persona.Ciudad = nuevaCiudad;

            Console.WriteLine($"Saldo actual: {persona.Saldo}");
            Console.Write("Nuevo saldo: ");
            string nuevoSaldoInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoSaldoInput))
            {
                if (!decimal.TryParse(nuevoSaldoInput, out decimal nuevoSaldo) || nuevoSaldo < 0)
                {
                    Console.WriteLine("El saldo debe ser un número positivo.");
                    return;
                }
                persona.Saldo = nuevoSaldo;
            }

            GuardarPersonas(personas);
            Console.WriteLine("Persona actualizada con éxito.");
            RegistrarLog(usuarioActual, $"Editó persona ID {id}");
        }

        static void EliminarPersona(string usuarioActual)
        {
            Console.Clear();
            Console.WriteLine("=== Eliminar Persona ===");

            List<Persona> personas = CargarPersonas();

            Console.Write("Ingrese el ID de la persona a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            Persona persona = personas.Find(p => p.ID == id);
            if (persona == null)
            {
                Console.WriteLine("Persona no encontrada.");
                return;
            }

            Console.WriteLine($"\nID: {persona.ID}");
            Console.WriteLine($"Nombre: {persona.Nombres} {persona.Apellidos}");
            Console.WriteLine($"Teléfono: {persona.Telefono}");
            Console.WriteLine($"Ciudad: {persona.Ciudad}");
            Console.WriteLine($"Saldo: {persona.Saldo:C}");

            Console.Write("\n¿Estás seguro que deseas eliminar esta persona? (S/N): ");
            string confirmacion = Console.ReadLine().Trim().ToUpper();

            if (confirmacion == "S")
            {
                personas.Remove(persona);
                GuardarPersonas(personas);
                Console.WriteLine("Persona eliminada.");
                RegistrarLog(usuarioActual, $"Eliminó persona ID {id} - {persona.Nombres} {persona.Apellidos}");
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
                RegistrarLog(usuarioActual, $"Canceló eliminación de persona ID {id}");
            }
        }

        static void MostrarInforme(string usuarioActual)
        {
            Console.Clear();
            Console.WriteLine("=== INFORME POR CIUDAD ===\n");

            List<Persona> personas = CargarPersonas();

            if (personas.Count == 0)
            {
                Console.WriteLine("No hay personas registradas.");
                return;
            }

            var personasPorCiudad = new Dictionary<string, List<Persona>>();

            foreach (var persona in personas)
            {
                if (!personasPorCiudad.ContainsKey(persona.Ciudad))
                    personasPorCiudad[persona.Ciudad] = new List<Persona>();

                personasPorCiudad[persona.Ciudad].Add(persona);
            }

            decimal totalGeneral = 0;

            foreach (var ciudad in personasPorCiudad.Keys)
            {
                Console.WriteLine($"Ciudad: {ciudad}\n");
                Console.WriteLine($"{"ID",-5} {"Nombres",-15} {"Apellidos",-15} {"Saldo",10}");
                Console.WriteLine(new string('-', 50));

                decimal subtotal = 0;
                foreach (var p in personasPorCiudad[ciudad])
                {
                    Console.WriteLine($"{p.ID,-5} {p.Nombres,-15} {p.Apellidos,-15} {p.Saldo,10:C}");
                    subtotal += p.Saldo;
                }

                Console.WriteLine($"{"",-37}{"======="}");
                Console.WriteLine($"{"Total " + ciudad,-37}{subtotal,10:C}\n");

                totalGeneral += subtotal;
            }

            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"{"TOTAL GENERAL:",-37}{totalGeneral,10:C}");

            RegistrarLog(usuarioActual, "Generó informe de balances por ciudad");
        }
    }
}
