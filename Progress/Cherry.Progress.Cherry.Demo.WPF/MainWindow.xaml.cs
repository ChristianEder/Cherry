using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cherry.Progress.Cherry.Portable;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Pro.Demo.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IProgressDisplay
    {
        private IProgressService _progressService;

        public MainWindow()
        {
            InitializeComponent();

            // Startup code
            // Instead of using a func here, using Cherry.IoC to retrieve a IProgressService 
            // and registering this as IProgressDisplay in the IoC container would work as well
            _progressService = new CherryProgressService(() => this);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create progress and do something
            using (var progress = _progressService.CreateProgress("SomeArbitraryKey"))
            {
                progress.Max = 3;

                progress.Title = "Loading some data...(Step one of three)";
                await SomeAsyncTask(1200);
                progress.Increment();

                progress.Title = "Loading additional data...(Step two of three)";
                await SomeAsyncTask(500);
                progress.Increment();

                progress.Title = "Loading even more data...(Step three of three)";
                await SomeAsyncTask(800);
                progress.Increment();
            }
        }

        private static Task SomeAsyncTask(int millisecondsDelay)
        {
            return Task.Factory.StartNew(() => Thread.Sleep(millisecondsDelay));
        }

        public void OnProgressStarted(IProgress progress)
        {
            ProgressLabel.Content = progress.Title;
            ProgressBar.Maximum = progress.Max.GetValueOrDefault();
            ProgressBar.Value = progress.Current.GetValueOrDefault();
        }

        public void OnProgressChanged(IProgress progress)
        {
            ProgressLabel.Content = progress.Title;
            ProgressBar.Maximum = progress.Max.GetValueOrDefault();
            ProgressBar.Value = progress.Current.GetValueOrDefault();
        }

        public void OnProgressCompleted(IProgress progress)
        {
            ProgressLabel.Content = "Done";
            ProgressBar.Value = 0;
        }
    }
}
