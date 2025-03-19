using System.Windows;
using Windowshop.WPF;

namespace Windowshop
{
    public partial class LoadingScreen : Window
    {
        WPFGlobalsBridge viewModel;

        public LoadingScreen()
        {
            InitializeComponent();
            viewModel = new WPFGlobalsBridge();
            DataContext = viewModel;
        }

        public WPFGlobalsBridge GetViewModel() => viewModel;
    }
}
