using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using OfficeOpenXml;
using System.Text;
using System.IO;

namespace List_note
{
    public partial class Listado_Notas : Window
    {
        public ObservableCollection<Nota> Notas { get; set; }
        public Nota NotaSeleccionada { get; set; }

        public ICommand EliminarNotaCommand { get; set; }

        public Listado_Notas()
        {
            InitializeComponent();
            Notas = new ObservableCollection<Nota>();
            DataContext = this;  

            EliminarNotaCommand = new RelayCommand<Nota>(EliminarNota);

            CargarNotas(); 
        }

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

            var result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta nota?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Notas.Remove(nota);

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
            var agregarNota = new Agregar_Nota(); 
            agregarNota.Show(); 
            this.Close(); 
        }

        // Método para abrir la ventana de edición
        private void EditarNota_Click(object sender, RoutedEventArgs e)
        {
            if (NotaSeleccionada == null)
            {
                MessageBox.Show("Por favor, selecciona una nota para editar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var ventanaEdicion = new EditarNota(NotaSeleccionada);
            ventanaEdicion.ShowDialog(); 

            
            var notaEditada = Notas.FirstOrDefault(n => n.Titulo == NotaSeleccionada.Titulo && n.Descripcion == NotaSeleccionada.Descripcion);
            if (notaEditada != null)
            {
                
                notaEditada.Color = NotaSeleccionada.Color;
                notaEditada.Titulo = NotaSeleccionada.Titulo;
                notaEditada.Descripcion = NotaSeleccionada.Descripcion;
            }
        }

        public void ExportarNotas_Click(object sender, RoutedEventArgs e)
        {
            // Abrir el explorador de archivos para elegir la ubicación y nombre del archivo
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivo de notas (*.notasunison)|*.notasunison",
                Title = "Exportar notas"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Guardar el archivo .notasunison
                string filePath = saveFileDialog.FileName;
                StringBuilder sb = new StringBuilder();

                foreach (var nota in Notas)
                {
                    sb.AppendLine($"{nota.Titulo}|{nota.Descripcion}|{nota.Color}"); // Guardamos en formato Titulo|Descripcion|Color
                }

                // Guardar el archivo .notasunison
                File.WriteAllText(filePath, sb.ToString());

                // Guardar el archivo .xlsx (con la misma información)
                string excelFilePath = Path.ChangeExtension(filePath, ".xlsx");
                GuardarExcel(excelFilePath);
            }
        }

        // Método para guardar las notas en un archivo Excel (.xlsx)
        private void GuardarExcel(string filePath)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Notas");
                worksheet.Cells[1, 1].Value = "Título";
                worksheet.Cells[1, 2].Value = "Descripción";
                worksheet.Cells[1, 3].Value = "Color";

                int row = 2; // Empezamos en la segunda fila
                foreach (var nota in Notas)
                {
                    worksheet.Cells[row, 1].Value = nota.Titulo;
                    worksheet.Cells[row, 2].Value = nota.Descripcion;
                    worksheet.Cells[row, 3].Value = nota.Color;
                    row++;
                }

                package.Save();
            }
        }

        public void ImportarNotas_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de notas (*.notasunison)|*.notasunison",
                Title = "Seleccionar archivo de notas"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                
                string filePath = openFileDialog.FileName;

               
                var lineas = File.ReadAllLines(filePath);

                foreach (var linea in lineas)
                {
                    var partes = linea.Split('|'); 
                    if (partes.Length == 3)  
                    {
                        var nota = new Nota
                        {
                            Titulo = partes[0],
                            Descripcion = partes[1],
                            Color = partes[2]
                        };

                        
                        AgregarNotaBD(nota);

                        
                        Notas.Add(nota);
                    }
                }
            }
        }

        // Método para agregar una nota a la base de datos
        private void AgregarNotaBD(Nota nota)
        {
            string connectionString = "Data Source=NotasDB.sqlite";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Notas (Titulo, Descripcion, Color) VALUES (@Titulo, @Descripcion, @Color)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Titulo", nota.Titulo);
                    command.Parameters.AddWithValue("@Descripcion", nota.Descripcion);
                    command.Parameters.AddWithValue("@Color", nota.Color);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
