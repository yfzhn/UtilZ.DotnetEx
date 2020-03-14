using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using UtilZ.Dotnet.Ex.Base;

namespace CoreWpfApp
{
    /// <summary>
    /// TestAsynQueue.xaml 的交互逻辑
    /// </summary>
    public partial class TestAsynQueue : Window
    {
        public TestAsynQueue()
        {
            InitializeComponent();
        }

        private AsynQueue<int> _asynQueue;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._asynQueue = new AsynQueue<int>(this.QueueCallback, "异步队列线程", true, false);
        }

        private void QueueCallback(int value)
        {
            this.Dispatcher.Invoke(new Func<int, object>(v =>
            {
                logControl.AppendText($"{value}\r");
                return null;
            }), System.Windows.Threading.DispatcherPriority.Normal, new object[] { value });
        }



        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (this._asynQueue.Status)
            {
                this._asynQueue.Stop();
                btnStart.Content = "Start";
            }
            else
            {
                this._asynQueue.Start();
                btnStart.Content = "Stop";
            }
        }

        private bool _flag = false;
        private readonly Random _rnd = new Random();
        private void btnStartProduce_Click(object sender, RoutedEventArgs e)
        {
            if (_flag)
            {
                _flag = false;
                btnStartProduce.Content = "StartProduce";
            }
            else
            {
                _flag = true;
                btnStartProduce.Content = "StopProduce";

                var produceInterval = int.Parse(txtProduceInterval.Text);
                var addTimeout = int.Parse(txtAddTimeout.Text);

                Task.Factory.StartNew((obj) =>
                {
                    var tp = (Tuple<int, int>)obj;
                    int produceInterval2 = tp.Item1;
                    int addTimeout2 = tp.Item2;
                    while (_flag)
                    {
                        this._asynQueue.Enqueue(_rnd.Next(0, 100));
                        Thread.Sleep(produceInterval2);
                    }
                }, new Tuple<int, int>(produceInterval, addTimeout));
            }
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            logControl.Document.Blocks.Clear();
        }

        private void btnCount_Click(object sender, RoutedEventArgs e)
        {
            logControl.AppendText($"队列.Count:{this._asynQueue.Count}\r");
        }
    }
}
