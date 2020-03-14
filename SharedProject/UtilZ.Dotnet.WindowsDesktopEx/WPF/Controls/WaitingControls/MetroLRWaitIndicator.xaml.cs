using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.WaitingControls
{
    /// <summary>
    /// MetroLRWaitIndicator.xaml 的交互逻辑
    /// </summary>
    public partial class MetroLRWaitIndicator : UserControl
    {
        /// <summary>
        /// 动画执行的故事板对象
        /// </summary>
        private Storyboard _transStorybord = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MetroLRWaitIndicator()
        {
            InitializeComponent();

            this.DataContext = new AnimalSegmentSize();
            _transStorybord = Resources["Trans"] as Storyboard;

            this.Loaded += ((sender, e) =>
            {
                Active();
            });
        }

        /// <summary>
        /// 激活动画
        /// </summary>
        public void Active()
        {
            int waittime = ((AnimalSegmentSize)this.DataContext).MilliSecondsDelay;
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Thread.Sleep(waittime);
                this.Excute(el1);
                Thread.Sleep(waittime);
                this.Excute(el2);
                Thread.Sleep(waittime);
                this.Excute(el3);
                Thread.Sleep(waittime);
                this.Excute(el4);
                Thread.Sleep(waittime);
                this.Excute(el5);
                Thread.Sleep(waittime);
            });
        }

        /// <summary>
        /// 启动一个动画
        /// </summary>
        /// <param name="el"></param>
        private void Excute(Ellipse el)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                el.BeginStoryboard(_transStorybord);
            }));
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void Stop()
        {
            _transStorybord.Stop(el1);
            _transStorybord.Stop(el2);
            _transStorybord.Stop(el3);
            _transStorybord.Stop(el4);
            _transStorybord.Stop(el5);
        }
    }

    /// <summary>
    /// 动画段对象
    /// </summary>
    public class AnimalSegmentSize : NotifyPropertyChangedAbs
    {
        private double _segment1 = 200;

        /// <summary>
        /// 段1
        /// </summary>
        public double Segment1
        {
            get { return _segment1; }
            set
            {
                _segment1 = value;
                this.OnRaisePropertyChanged("Segment1");
            }
        }
        private double _segment2 = 300;

        /// <summary>
        /// 段2
        /// </summary>
        public double Segment2
        {
            get { return _segment2; }
            set
            {
                _segment2 = value;
                this.OnRaisePropertyChanged("Segment2");
            }
        }
        private double _segment3 = 500;

        /// <summary>
        /// 段3
        /// </summary>
        public double Segment3
        {
            get { return _segment3; }
            set
            {
                _segment3 = value;
                this.OnRaisePropertyChanged("Segment3");
            }
        }

        private int _milliSecondsDelay = 100;

        /// <summary>
        /// 延迟时间
        /// </summary>
        public int MilliSecondsDelay
        {
            get { return _milliSecondsDelay; }
            set
            {
                _milliSecondsDelay = value;
                this.OnRaisePropertyChanged("MilliSecondsDelay");
            }
        }
    }
}
