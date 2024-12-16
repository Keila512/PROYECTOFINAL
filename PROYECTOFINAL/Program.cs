using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTOFINAL
{
    internal class Program
    {
        private static string connectionString = "Server=KEILA\\SQLEXPRESS01;Database=ProyectoFinal;Integrated Security=True";
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. login");
                Console.WriteLine("2. Exit");
                switch (Console.ReadLine())
                {
                    case "1":
                        {
                            Console.WriteLine("Enter your Username:");
                            string usuario = Console.ReadLine();
                            Console.WriteLine("Enter your password:");
                            string contrasena = Console.ReadLine();
                            if (AutenticarUsuario(usuario, contrasena))
                            {
                                Console.WriteLine("Login successful.");
                                Console.WriteLine("1. Users Menu");
                                Console.WriteLine("2. Product Menu");
                                string opcion = Console.ReadLine();
                                if (opcion=="1")
                                {
                                    MostrarMenuCrud(usuario);
                                }
                                else if (opcion=="2") { MostrarMenuProducto(); }
                            }
                            else
                            {
                                Console.WriteLine("Incorrect credentials. Access denied.");
                            }
                            break;
                        }
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                    break;
                }
            }
        }
        private static bool AutenticarUsuario(string nombreUsuario, string contrasena)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id FROM Usuarios WHERE usuario = @usuario AND contrasena = @contrasena";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@usuario", nombreUsuario);
                        command.Parameters.AddWithValue("@contrasena", EncriptarContrasena(contrasena));

                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return true; 
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false; 
                }
            }
        }
        private static string EncriptarContrasena(string contrasena)
        {
            using (SHA256 sha256Hash = SHA256.Create())

            { 
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
            StringBuilder builder = new StringBuilder();
            byte[] array = bytes;
            foreach (byte b in array)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
            }
        }
        private static void MostrarMenuCrud(string usuario)
        {
            bool salir = true;
            while (salir)
            {
                Console.WriteLine("\nCRUD Menu for Users:");
                Console.WriteLine("1. View Users");
                Console.WriteLine("2. Create Users");
                Console.WriteLine("3. Update Users");
                Console.WriteLine("4. See Audit");
                Console.WriteLine("5. Exit");
                switch (Console.ReadLine())
                { 
                case "1":
                        ObtenerUsuarios();
                    break;
                case "2":
                        CrearUsuario();
                        break;
                    case "3":
                        {
                            Console.WriteLine("Enter the ID of the employee to update:");
                            int UsuarioIdActualizar = int.Parse(Console.ReadLine());
                            ActualizarUsuario(UsuarioIdActualizar, usuario);
                            break;
                        }
                    case "4":
                        VerAuditoria();
                        break;
                    case "5":
                        Console.WriteLine("Exiting the CRUD menu.");
                        salir = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                    break;
                    
                }
            }
        }
        private static void CrearUsuario()
        {
            Console.WriteLine("Enter the user's full name:");
            string nombrecompleto = Console.ReadLine();
            Console.WriteLine("Enter the username:");
            string usuario = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string contrasena = Console.ReadLine();
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Usuarios (nombrecompleto, estado, usuario, contrasena) VALUES (@nombrecompleto, 'activo', @usuario, @contrasena)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    { 
                    command.Parameters.AddWithValue("@nombrecompleto", nombrecompleto);
                    command.Parameters.AddWithValue("@usuario", usuario);
                    command.Parameters.AddWithValue("@contrasena", EncriptarContrasena(contrasena));
                    command.ExecuteNonQuery();
                    Console.WriteLine("Employee created successfully.");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
        }
        private static void VerAuditoria()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {     
            string query = "SELECT * FROM Auditoria ORDER BY fecha DESC";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                           SqlDataReader reader = command.ExecuteReader();
                           while (reader.Read())
                           {
                           Console.WriteLine(string.Format("ID: {0}, Usuario: {1}, Tabla: {2}, ", reader["id"], 
                           reader["usuario"], reader["tabla_modificada"]) + string.Format("Operación: {0}, Fecha: {1}", reader["operacion"], reader["fecha"]));
                           }
                }
            }
        }
        static void ObtenerUsuarios()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id, nombrecompleto, usuario, estado FROM Usuarios";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Console.WriteLine("ID: {0}, Nombre Completo: {1}, Usuario: {2}, Estado: {3}",
                                reader["id"], reader["nombrecompleto"], reader["usuario"], reader["estado"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        private static void MostrarMenuProducto()
        {
            bool salir = true;
            while (salir)
            {
                Console.WriteLine("\nCRUD Menu for Products:");
                Console.WriteLine("1. See Products");
                Console.WriteLine("2. Create Products");
                Console.WriteLine("3. Update Products");
                Console.WriteLine("4. See Audit");
                Console.WriteLine("5. Exit");
                switch (Console.ReadLine())
                {
                    case "1":
                        ObtenerProducto();
                        break;
                    case "2":
                        CrearProducto();
                        break;
                    case "3":
                        
                            Console.WriteLine("Enter the ID of the product to update:");
                             int idProducto = int.Parse(Console.ReadLine());
                            ActualizarProducto( idProducto); 
                            break;
                    case "4":
                        VerAuditoria();
                        break;
                    case "5":
                        Console.WriteLine("Exiting the CRUD menu.");
                        salir = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                    break;

                }
            }
        }
        static void ActualizarUsuario(int UsuarioId, string usuario)
        {
            Console.WriteLine("\nUpdate User Data:");

            Console.Write("Enter the new status (active/inactive): ");
            string nuevoEstado = Console.ReadLine();
            if (nuevoEstado == "inactive")
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string selectQuery = "SELECT estado FROM Usuarios WHERE id = @UsuarioId";
                    string estadoAnterior = "";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                        SqlDataReader reader = selectCmd.ExecuteReader();

                        if (reader.Read())
                        {
                            estadoAnterior = reader["estado"].ToString();
                        }
                        reader.Close();
                    }

                    string updateQuery = "UPDATE Usuarios SET estado = @nuevoEstado WHERE id = @UsuarioId";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@nuevoEstado", nuevoEstado);
                        updateCmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                        updateCmd.ExecuteNonQuery();
                    }

                    string insertAuditQuery = "INSERT INTO Auditoria (usuario, tabla_modificada, operacion, dato_anterior, dato_nuevo, fecha) " +
                                              "VALUES (@usuario, 'Usuarios', 'UPDATE', @datoAnterior, @datoNuevo, GETDATE())";

                    using (SqlCommand auditCmd = new SqlCommand(insertAuditQuery, conn))
                    {
                        auditCmd.Parameters.AddWithValue("@usuario", usuario);
                        auditCmd.Parameters.AddWithValue("@datoAnterior", estadoAnterior);
                        auditCmd.Parameters.AddWithValue("@datoNuevo", nuevoEstado);
                        auditCmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Status updated to 'inactive'.");
                }
            }
            else if (nuevoEstado == "activo")
            {
                Console.Write("Enter the new password: ");
                string nuevaContrasena = Console.ReadLine();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string selectQuery = "SELECT estado, contrasena FROM Usuarios WHERE id = @UsuarioId";
                    string estadoAnterior = "", contrasenaAnterior = "";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                        SqlDataReader reader = selectCmd.ExecuteReader();

                        if (reader.Read())
                        {
                            estadoAnterior = reader["estado"].ToString();
                            contrasenaAnterior = reader["contrasena"].ToString();
                        }
                        reader.Close();
                    }

                    string contrasenaEncriptada = EncriptarContrasena(nuevaContrasena);

                    string updateQuery = "UPDATE Usuarios SET estado = @nuevoEstado, contrasena = @nuevaContrasena WHERE id = @UsuarioId";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@nuevoEstado", nuevoEstado);
                        updateCmd.Parameters.AddWithValue("@nuevaContrasena", contrasenaEncriptada);
                        updateCmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                        updateCmd.ExecuteNonQuery();
                    }

                    string insertAuditQuery = "INSERT INTO Auditoria (usuario, tabla_modificada, operacion, dato_anterior, dato_nuevo, fecha) " +
                                              "VALUES (@usuario, 'Usuarios', 'UPDATE', @datoAnterior, @datoNuevo, GETDATE())";

                    using (SqlCommand auditCmd = new SqlCommand(insertAuditQuery, conn))
                    {
                        auditCmd.Parameters.AddWithValue("@usuario", usuario);
                        auditCmd.Parameters.AddWithValue("@datoAnterior", estadoAnterior + " / " + contrasenaAnterior);
                        auditCmd.Parameters.AddWithValue("@datoNuevo", nuevoEstado + " / " + contrasenaEncriptada);
                        auditCmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Status and password updated.");
                }
            }
            else
            {
                Console.WriteLine("Invalid status. The operation has not been performed.");
            }
        }
        static void CrearProducto()
        {
            Console.WriteLine("Enter the new product data:");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string selectQuery = "SELECT id, nombre FROM Categoria";
                using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                {
                    SqlDataReader reader = selectCmd.ExecuteReader();

                    Console.WriteLine("Available categories:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["id"]}. {reader["nombre"]}");
                    }
                    reader.Close();

                    Console.Write("Select the category ID: ");
                    int idCategoria = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Product name: ");
                    string nombre = Console.ReadLine();
                    Console.Write("Product Description: ");
                    string descripcion = Console.ReadLine();
                    Console.Write("Product price: ");
                    decimal precio = Convert.ToDecimal(Console.ReadLine());
                    Console.Write("Current stock: ");
                    int stockActual = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Minimum stock: ");
                    int stockMinimo = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Expiration date (yyyy-mm-dd): ");
                    DateTime fechaVencimiento = DateTime.Parse(Console.ReadLine());

                    string estado = "activo"; 

                    string insertQuery = @"
                INSERT INTO Producto (nombre, descripcion, precio, id_categoria, stock_actual, stock_minimo, fecha_vencimiento, estado)
                VALUES (@nombre, @descripcion, @precio, @id_categoria, @stock_actual, @stock_minimo, @fecha_vencimiento, @estado)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@nombre", nombre);
                        insertCmd.Parameters.AddWithValue("@descripcion", descripcion);
                        insertCmd.Parameters.AddWithValue("@precio", precio);
                        insertCmd.Parameters.AddWithValue("@id_categoria", idCategoria);
                        insertCmd.Parameters.AddWithValue("@stock_actual", stockActual);
                        insertCmd.Parameters.AddWithValue("@stock_minimo", stockMinimo);
                        insertCmd.Parameters.AddWithValue("@fecha_vencimiento", fechaVencimiento);
                        insertCmd.Parameters.AddWithValue("@estado", estado); 

                        insertCmd.ExecuteNonQuery();
                        Console.WriteLine("Product created successfully.");
                    }
                }
            }
        }
        static void ObtenerProducto()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string selectCategoriesQuery = "SELECT id, nombre FROM Categoria";
                using (SqlCommand selectCategoriesCmd = new SqlCommand(selectCategoriesQuery, conn))
                {
                    SqlDataReader reader = selectCategoriesCmd.ExecuteReader();

                    Console.WriteLine("Available categories:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["id"]}. {reader["nombre"]}");
                    }
                    reader.Close();

                    Console.Write("Select the ID of the category you want to view products from: ");
                    int idCategoria = Convert.ToInt32(Console.ReadLine());

                    string query = @"
                SELECT p.id, p.nombre, p.descripcion, p.precio, p.stock_actual, p.stock_minimo, p.fecha_vencimiento, c.nombre AS categoria, p.estado
                FROM Producto p
                INNER JOIN Categoria c ON p.id_categoria = c.id
                WHERE p.id_categoria = @idCategoria";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@idCategoria", idCategoria);

                        SqlDataReader productReader = command.ExecuteReader();

                        Console.WriteLine("\nID | Name | Description | Price | Current Stock | Minimum Stock | Expiration Date | Category | Status");
                        Console.WriteLine("----------------------------------------------------------------------------------------------------");
                        while (productReader.Read())
                        {
                            Console.WriteLine($"{productReader["ID"]} | {productReader["Name"]} | {productReader["Description"]} | {productReader["Price"]} " +
                                $"| {productReader["Current Stock"]} | {productReader["Minimum Stock"]} | {productReader["Expiration Date"]} | {productReader["Category"]} | {productReader["Status"]}");
                        }
                        productReader.Close();
                    }
                }
            }
        }
        static void ActualizarProducto(int idProducto)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string selectQuery = "SELECT nombre, descripcion, precio, stock_actual, stock_minimo, fecha_vencimiento, id_categoria, estado FROM Producto WHERE id = @idProducto";
                using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                {
                    selectCmd.Parameters.AddWithValue("@idProducto", idProducto);
                    SqlDataReader reader = selectCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Console.WriteLine("\nCurrent product data:");
                        Console.WriteLine($"Name: {reader["nombre"]}");
                        Console.WriteLine($"Description: {reader["descripcion"]}");
                        Console.WriteLine($"Price: {reader["precio"]}");
                        Console.WriteLine($"Current Stock: {reader["stock_actual"]}");
                        Console.WriteLine($"Minimum Stock: {reader["stock_minimo"]}");
                        Console.WriteLine($"Expiration Date: {reader["fecha_vencimiento"]}");
                        Console.WriteLine($"State: {reader["estado"]}");
                        reader.Close();

                        Console.WriteLine();

                        Console.Write("New name (): ");
                        string nombre = Console.ReadLine();
                        if (string.IsNullOrEmpty(nombre)) nombre = reader["nombre"].ToString();

                        Console.Write("New description (press Enter to keep the current one): ");
                        string descripcion = Console.ReadLine();
                        if (string.IsNullOrEmpty(descripcion)) descripcion = reader["descripcion"].ToString();

                        Console.Write("New price (press Enter to keep the current one): ");
                        string precioInput = Console.ReadLine();
                        decimal precio = string.IsNullOrEmpty(precioInput) ? Convert.ToDecimal(reader["precio"]) : Convert.ToDecimal(precioInput);

                        Console.Write("New current stock (press Enter to keep current): ");
                        string stockActualInput = Console.ReadLine();
                        int stockActual = string.IsNullOrEmpty(stockActualInput) ? Convert.ToInt32(reader["stock_actual"]) : Convert.ToInt32(stockActualInput);

                        Console.Write("New minimum stock (press Enter to keep the current one): ");
                        string stockMinimoInput = Console.ReadLine();
                        int stockMinimo = string.IsNullOrEmpty(stockMinimoInput) ? Convert.ToInt32(reader["stock_minimo"]) : Convert.ToInt32(stockMinimoInput);

                        Console.Write("New due date (yyyy-mm-dd) (press Enter to keep the current one): ");
                        string fechaVencimientoInput = Console.ReadLine();
                        DateTime fechaVencimiento = string.IsNullOrEmpty(fechaVencimientoInput) ? Convert.ToDateTime(reader["fecha_vencimiento"]) : Convert.ToDateTime(fechaVencimientoInput);

                        string selectCategoriesQuery = "SELECT id, nombre FROM Categoria";
                        using (SqlCommand selectCategoriesCmd = new SqlCommand(selectCategoriesQuery, conn))
                        {
                            SqlDataReader categoryReader = selectCategoriesCmd.ExecuteReader();
                            Console.WriteLine("\nAvailable categories:");
                            while (categoryReader.Read())
                            {
                                Console.WriteLine($"{categoryReader["id"]}. {categoryReader["nombre"]}");
                            }
                            categoryReader.Close();

                            Console.Write("Select the ID of the new category (press Enter to keep the current one): ");
                            string categoriaInput = Console.ReadLine();
                            int idCategoria = string.IsNullOrEmpty(categoriaInput) ? Convert.ToInt32(reader["id_categoria"]) : Convert.ToInt32(categoriaInput);

                            Console.Write("New status (active/inactive) (press Enter to keep the current one): ");
                            string estado = Console.ReadLine().ToLower();
                            if (string.IsNullOrEmpty(estado)) estado = reader["estado"].ToString();

                            string updateQuery = @"
                        UPDATE Producto 
                        SET nombre = @nombre, 
                            descripcion = @descripcion, 
                            precio = @precio, 
                            stock_actual = @stockActual, 
                            stock_minimo = @stockMinimo, 
                            fecha_vencimiento = @fechaVencimiento, 
                            id_categoria = @idCategoria, 
                            estado = @estado 
                        WHERE id = @idProducto";

                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@nombre", nombre);
                                updateCmd.Parameters.AddWithValue("@descripcion", descripcion);
                                updateCmd.Parameters.AddWithValue("@precio", precio);
                                updateCmd.Parameters.AddWithValue("@stockActual", stockActual);
                                updateCmd.Parameters.AddWithValue("@stockMinimo", stockMinimo);
                                updateCmd.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                                updateCmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                                updateCmd.Parameters.AddWithValue("@estado", estado);
                                updateCmd.Parameters.AddWithValue("@idProducto", idProducto);

                                int rowsAffected = updateCmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("\nProduct updated successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("\nNo update was performed.");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nProduct not found.");
                    }
                }
            }
        }
    }
}
