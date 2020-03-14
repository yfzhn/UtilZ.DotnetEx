using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls;
using CoreWpfApp.Model;

namespace CoreWpfApp
{
    /// <summary>
    /// TestChartCollection.xaml 的交互逻辑
    /// </summary>
    public partial class TestChartCollection : Window
    {
        public TestChartCollection()
        {
            InitializeComponent();
        }

        private ChartCollection<Stu> _stus;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this._stus = new ChartCollection<Stu>();
            this._stus.CollectionChanged += _stus_CollectionChanged;
            this._stus.ChartCollectionChanged += _stus_ChartCollectionChanged;
        }

        private void _stus_ChartCollectionChanged(object sender, ChartCollectionChangedEventArgs<Stu> e)
        {
            logControl.AddLog($"ChartCollectionChanged_Action:{e.Action.ToString()},    NewItemsCount:{e.NewItems?.Count},  OldItemsCount:{e.OldItems?.Count}", UtilZ.Dotnet.Ex.Log.LogLevel.Info);
        }

        

        private void _stus_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            logControl.AddLog($"CollectionChanged_Action:{e.Action.ToString()},         NewItemsCount:{e.NewItems?.Count},   OldItemsCount:{e.OldItems?.Count}", UtilZ.Dotnet.Ex.Log.LogLevel.Info);
        }

        private const string _SP = "-----------------------------------------------------------------";
        private Stu _stu = new Stu() { ID = 1, Name = "zhn" };
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("Add", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.Add(_stu);
        }

        private void btnAddRaange_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("AddRange", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.AddRange(new Stu[] { new Stu() { ID = 2, Name = "zzx" }, new Stu() { ID = 3, Name = "zvb" } });
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("Insert", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.Insert(0, new Stu() { ID = 4, Name = "yy" });
        }

        private void btnInsertAddRaange_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("InsertRange", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.InsertRange(0, new Stu[] { new Stu() { ID = 5, Name = "yc" }, new Stu() { ID = 6, Name = "aa" } });
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("Remove", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.Remove(this._stu);
        }

        private void btnRemoveAt_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("RemoveAt", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.RemoveAt(0);
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("RemoveAll", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.RemoveAll(t => t.ID < 3);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog(_SP, UtilZ.Dotnet.Ex.Log.LogLevel.Info);
            logControl.AddLog("Clear", UtilZ.Dotnet.Ex.Log.LogLevel.Warn);
            this._stus.Clear();
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            logControl.Clear();
        }
    }
}
