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
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Base.MemoryCache;
using UtilZ.Dotnet.Ex.Log;

namespace CoreWpfApp
{
    /// <summary>
    /// TestMemoryCacheEx.xaml 的交互逻辑
    /// </summary>
    public partial class TestMemoryCacheEx : Window
    {
        public TestMemoryCacheEx()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var redirectAppenderToUI = (RedirectAppender)Loger.GetAppenderByName(null, "RedirectToUI");
            if (redirectAppenderToUI != null)
            {
                redirectAppenderToUI.RedirectOuput += RedirectLogOutput;
            }
        }

        private void RedirectLogOutput(object sender, RedirectOuputArgs e)
        {
            string str;
            try
            {
                str = string.Format("{0} {1}", DateTime.Now, e.Item.Content);
            }
            catch (Exception ex)
            {
                str = ex.Message;
            }

            logControl.AddLog(str, e.Item.Level);
        }


        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            Loger.Info($"------------------------------------------------------------");
            List<int> keyList = new List<int>() { 1, 2, 3, 4, 5, 6 };
            foreach (var key in keyList)
            {
                object value = MemoryCacheEx.Get(key);
                if (value != null)
                {
                    Loger.Info($"缓存Key:{key}对应值为:{value}");
                }
                else
                {
                    Loger.Warn($"缓存Key:{key}不存在");
                }
            }
            Loger.Info($"------------------------------------------------------------");
        }


        private void CacheItemRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            Loger.Warn($"缓存项{arguments.Key}移除");
            arguments.CacheItem.RemovedCallback = null;
        }


        private int _expiration = 5000;
        private bool _firstAdd = true;
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._firstAdd)
                {
                    MemoryCacheEx.Add(1, TimeEx.GetTimestamp());
                    this._firstAdd = false;
                }

                MemoryCacheEx.Add(2, TimeEx.GetTimestamp(), this._expiration, this.CacheItemRemovedCallback);
                MemoryCacheEx.Add(3, TimeEx.GetTimestamp(), TimeSpan.FromMilliseconds(this._expiration), this.CacheItemRemovedCallback);
                MemoryCacheEx.Add(4, TimeEx.GetTimestamp(), (t) => { return (DateTimeOffset.Now - t.AddTime).TotalMilliseconds > this._expiration * 2; }, this.CacheItemRemovedCallback);

                CacheItem cacheItem = new CacheItem(5, TimeEx.GetTimestamp());
                cacheItem.AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(this._expiration);
                //cacheItem.SlidingExpiration = slidingExpiration;
                //cacheItem.CustomerExpiration = customerExpiration;
                cacheItem.RemovedCallback = this.CacheItemRemovedCallback;
                //cacheItem.InnerRemovedCallback = true;
                MemoryCacheEx.Add(cacheItem);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            MemoryCacheEx.Set(1, TimeEx.GetTimestamp());
            MemoryCacheEx.Set(2, TimeEx.GetTimestamp(), this._expiration, this.CacheItemRemovedCallback);
            MemoryCacheEx.Set(3, TimeEx.GetTimestamp(), TimeSpan.FromMilliseconds(this._expiration), this.CacheItemRemovedCallback);
            MemoryCacheEx.Set(4, TimeEx.GetTimestamp(), (t) => { return (DateTimeOffset.Now - t.AddTime).TotalMilliseconds > this._expiration * 2; }, this.CacheItemRemovedCallback);

            CacheItem cacheItem = new CacheItem(5, TimeEx.GetTimestamp());
            cacheItem.AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(this._expiration);
            //cacheItem.SlidingExpiration = slidingExpiration;
            //cacheItem.CustomerExpiration = customerExpiration;
            cacheItem.RemovedCallback = this.CacheItemRemovedCallback;
            //cacheItem.InnerRemovedCallback = true;
            MemoryCacheEx.Set(cacheItem);
        }


        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            MemoryCacheEx.Remove(3);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            MemoryCacheEx.Clear();
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            logControl.Clear();
        }
    }
}
