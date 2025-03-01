using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core = MainSystemEngine;

namespace VAOEngine
{
    public partial class MainWindow : Window
    {

        Core _Engine = new Core();

        public MainWindow()
        {
            InitializeComponent();
            _Control.Start();
            _Engine.Load();
        }

        private void _Control_Render(TimeSpan obj)
        {
            _Engine.Render();
        }
    }
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
