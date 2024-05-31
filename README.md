# TicketCine

## 1. Agregando dependencia

Dentro del Studio code, vayan a:
Herramientas > Administrador de paquetes Nuget > Consola del administrador de paquetes

en el terminal que aparecerá agreguen: 

`Install-Package Npgsql`

Asegurense de usar la dependencias del SQL con agregando al principio:

`using Npgsql;`

## 2. Accediendo al SQL

primero deben de agregar un string que pueda ser accesible facilmente con la cadena de conexión:

`string cadenaConexion = "Server=hansken.db.elephantsql.com;Port=5432;Database=ikegunyj;User Id=ikegunyj;Password=PjDjGMbve9rwF5eP4fMGm5M59yzCpExq;";}`

recomiendo agregarlo de la siguiente manera:

```
public partial class MainWindow : Window
{
    string cadenaConexion = "Server=hansken.db.elephantsql.com;Port=5432;Database=ikegunyj;User Id=ikegunyj;Password=PjDjGMbve9rwF5eP4fMGm5M59yzCpExq;";
    public MainWindow()
    {
        InitializeComponent();
    }
    ///Resto del código
}
```

después ya pueden acceder usando Try y Catch, como este ejemplo para iniciar sesión

```
private bool AutenticarUsuario(string usuario, string contrasena)
{
    bool autenticado = false;
    string consulta = "SELECT contrasena = crypt(@contrasena, contrasena) FROM userbase WHERE usuario = @usuario";

    try
    {
        using (var conexion = new NpgsqlConnection(cadenaConexion))
        {
            conexion.Open();
            using (var cmd = new NpgsqlCommand(consulta, conexion))
            {
                cmd.Parameters.Add(new NpgsqlParameter("@usuario", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = usuario });
                cmd.Parameters.Add(new NpgsqlParameter("@contrasena", NpgsqlTypes.NpgsqlDbType.Text) { Value = contrasena });

                autenticado = Convert.ToBoolean(cmd.ExecuteScalar());

                if (autenticado)
                {
                    MessageBox.Show("Inició sesión.");
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrecta.");
                }
            }
        }
    }
    catch (NpgsqlException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        MessageBox.Show($"error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"error: {ex.Message}");
        MessageBox.Show($"error: {ex.Message}");
    }
    return autenticado;
}
```
