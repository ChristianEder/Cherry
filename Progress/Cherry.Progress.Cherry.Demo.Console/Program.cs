using Cherry.Progress.Cherry.Portable;
using Cherry.Progress.Contracts.Portable;

namespace Cherry.Progress.Cherry.Demo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Startup code
            // Instead of using a func here, using Cherry.IoC to retrieve a IProgressService 
            // and registering the ConsoleProgressDisplay in the IoC container would work as well
            IProgressService progressService = new CherryProgressService(() => new ConsoleProgressDisplay());

         

            // Create progress and do something
            using (var progress = progressService.CreateProgress("SomeArbitraryKey"))
            {
                progress.Max = 3;

                progress.Title = "Loading some data...(Step one of three)";
                progress.Increment();

                progress.Title = "Loading additional data...(Step two of three)";
                progress.Increment();

                progress.Title = "Loading even more data...(Step three of three)";
                progress.Increment();
            }
            System.Console.ReadKey();
        }

        internal class ConsoleProgressDisplay : IProgressDisplay
        {
            public void OnProgressStarted(IProgress progress)
            {
                System.Console.WriteLine("Progress started: {0}", progress.Key);
            }

            public void OnProgressChanged(IProgress progress)
            {
                System.Console.WriteLine("Progress changed: {0} - \"{1}\" {2}/{3}", progress.Key, progress.Title, progress.Current, progress.Max);
            }

            public void OnProgressCompleted(IProgress progress)
            {
                System.Console.WriteLine("Progress completed: {0}", progress.Key);
            }
        }
    }
}
