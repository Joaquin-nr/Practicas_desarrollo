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
            NotaSeleccionada = nota; // Pasamos la nota seleccionada
            DataContext = this;     // Enlazamos el DataContext
        }

        // Botón Guardar
        private void GuardarNota_Click(object sender, RoutedEventArgs e)
        {
            // Aquí puedes implementar la lógica para actualizar la base de datos si es necesario.
            MessageBox.Show("Nota guardada correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close(); // Cierra la ventana de edición
        }

        // Botón Cancelar
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("¿Salir sin guardar cambios?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Close(); // Cierra la ventana sin guardar                
            }
            
        }
    }
}
