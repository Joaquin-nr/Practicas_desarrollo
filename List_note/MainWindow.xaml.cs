using System;
using System.Windows;
using Microsoft.Data.Sqlite; // Asegúrate de tener Microsoft.Data.Sqlite instalado desde NuGet
using OfficeOpenXml;

namespace List_note
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            InitializeComponent();
            CrearBaseDeDatosYTabla(); 
        }

        private void CrearBaseDeDatosYTabla()
        {
            // Ruta de la base de datos (se creará en el directorio del proyecto)
            string databasePath = "NotasDB.sqlite";

            // Verificar si ya existe la base de datos
            if (!System.IO.File.Exists(databasePath))
            {
                // Crear la base de datos
                using (var connection = new SqliteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Notas (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Titulo TEXT NOT NULL,
                            Descripcion TEXT,
                            Color TEXT
                        );";

                    using (var command = new SqliteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Base de datos y tabla 'Notas' creadas exitosamente.");
                    }

                    connection.Close();
                }
            }
            else
            {
                
            }
        }

        public void GoToListado_click(object sender, RoutedEventArgs e)
        {
            // Navega a la ventana Listado de Notas
            var listadoNotas = new Listado_Notas();
            listadoNotas.Show();
            this.Close(); // Cierra la ventana actual
        }
    }
}
