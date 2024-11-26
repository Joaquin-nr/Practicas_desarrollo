using System.Windows;
using static List_note.Listado_Notas;

namespace List_note
{
    public partial class EditarNota : Window
    {
        public Nota NotaSeleccionada { get; set; }

        public EditarNota(Nota nota)
        {
            InitializeComponent();

            NotaSeleccionada = nota ?? new Nota
            {
                Titulo = string.Empty,
                Descripcion = string.Empty,
                Color = "Rojo" 
            };

            DataContext = this;
        }

        // Botón Guardar
        private void GuardarNota_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show($"Nota guardada correctamente","Información", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        // Botón Cancelar
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("¿Salir sin guardar cambios?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();                
            }
        }
    }
}
