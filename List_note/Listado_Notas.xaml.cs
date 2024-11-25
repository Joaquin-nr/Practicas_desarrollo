using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace List_note
{
    public partial class Listado_Notas : Window
    {
        public ObservableCollection<Nota> Notas { get; set; }
        public Nota NotaSeleccionada { get; set; }

        // Definimos el comando aquí en el código detrás
        public ICommand EliminarNotaCommand { get; set; }

        public Listado_Notas()
        {
            InitializeComponent();
            Notas = new ObservableCollection<Nota>();
            DataContext = this;  // Establece el DataContext de la ventana

            EliminarNotaCommand = new RelayCommand<Nota>(EliminarNota);

            CargarNotas(); // Cargar las notas al iniciar
        }

        // Clase para representar una nota
        public class Nota
        {
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string Color { get; set; }
        }

        // Método para cargar las notas desde la base de datos
        private void CargarNotas()
        {
            string connectionString = "Data Source=NotasDB.sqlite";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Titulo, Descripcion, Color FROM Notas";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Notas.Add(new Nota
                            {
                                Titulo = reader.GetString(0),
                                Descripcion = reader.GetString(1),
                                Color = reader.GetString(2)
                            });
                        }
                    }
                }
                connection.Close();
            }
        }

        // Método para eliminar una nota
        private void EliminarNota(Nota nota)
        {
            if (nota == null) return;

            // Mostrar el cuadro de confirmación
            var result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta nota?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Eliminar de la colección observable
                Notas.Remove(nota);

                // Eliminar de la base de datos
                EliminarNotaDeBD(nota);
            }
        }

        // Método para eliminar la nota de la base de datos
        private void EliminarNotaDeBD(Nota nota)
        {
            string connectionString = "Data Source=NotasDB.sqlite";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Notas WHERE Titulo = @Titulo AND Descripcion = @Descripcion";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Titulo", nota.Titulo);
                    command.Parameters.AddWithValue("@Descripcion", nota.Descripcion);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        // Método para ir a la ventana de agregar nota
        public void GoToAgregarNota_click(object sender, RoutedEventArgs e)
        {
            var agregarNota = new Agregar_Nota(); // Crear instancia de la ventana Agregar Nota
            agregarNota.Show(); // Mostrar la ventana Agregar Nota
            this.Close(); // Cerrar la ventana actual
        }

        // Método para abrir la ventana de edición
        private void EditarNota_Click(object sender, RoutedEventArgs e)
        {
            if (NotaSeleccionada == null)
            {
                MessageBox.Show("Por favor, selecciona una nota para editar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Crear y mostrar la ventana de edición, pasando la nota seleccionada
            var ventanaEdicion = new EditarNota(NotaSeleccionada);
            ventanaEdicion.ShowDialog(); // Mostrar como cuadro de diálogo

            // Refrescar la lista de notas en caso de que se hayan hecho cambios
            //CargarNotas();
        }


    }
}
