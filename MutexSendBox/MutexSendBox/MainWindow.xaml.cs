using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Task3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //   private static AsyncMutex mutex = new AsyncMutex();
        private static AsyncMutex mutex = new AsyncMutex();
        private const int numThreads = 5;

        public MainWindow()
        {
            InitializeComponent();
        }

        // This method represents a resource that must be synchronized 
        private async void UseResource()
        {
            //Start the Mutex
            await mutex.Lock();
            textBlockConsole.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate ()
                    {
                        textBlockConsole.Text += '\n' + String.Format("Thread has entered the protected area");
                    }
            ));

            // Simulate some work.
            Thread.Sleep(1000);

            textBlockConsole.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    delegate ()
                    {
                         textBlockConsole.Text += '\n' + String.Format("Thread is leaving the protected area");
                    }
            ));
            //Release mutex
            mutex.Release();
        }

        private async void btnStartThreads_Click(object sender, RoutedEventArgs e)
        {
            textBlockConsole.Text = "";
            // Create the threads that will use the protected resource. 
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < numThreads; i++)
                {
                    Thread newThread = new Thread(new ThreadStart(UseResource));
                    newThread.Name = String.Format("Thread{0}", i + 1);
                    newThread.Start();
                }
            });
        }
    }
}
