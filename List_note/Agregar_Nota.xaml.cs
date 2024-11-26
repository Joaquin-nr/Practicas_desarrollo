using System;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace List_note
{
    public partial class Agregar_Nota : Window
    {
        public Agregar_Nota()
        {
            InitializeComponent();
        }

        
        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            
            string titulo = TituloTextBox.Text;
            string descripcion = DescripcionTextBox.Text;
            string color = ColorComboBox.Text;

            
            if (string.IsNullOrWhiteSpace(titulo))
            {
                MessageBox.Show("El título es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ruta de la base de datos
            string databasePath = "NotasDB.sqlite";
            string connectionString = $"Data Source={databasePath}";

            // Insertar los datos en la base de datos
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO Notas (Titulo, Descripcion, Color)
                    VALUES (@Titulo, @Descripcion, @Color);";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Titulo", titulo);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);
                    command.Parameters.AddWithValue("@Color", color);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            // Confirmar al usuario
            MessageBox.Show("Nota agregada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            
            TituloTextBox.Text = string.Empty;
            DescripcionTextBox.Text = string.Empty;
            ColorComboBox.SelectedIndex = -1;

            var listadoNotas = new Listado_Notas();
            listadoNotas.Show();  
            this.Close();  
        }
        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            var listadoNotas = new Listado_Notas();
            listadoNotas.Show();  
            this.Close();  
        }
    }
}
