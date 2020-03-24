using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DatabaseTool
{
    internal class DBTableInfoEx : DBTableInfo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnRaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static Action<DBTableInfoEx> ShowFieldChanged;

        private bool _showField = false;
        [DisplayName("显示表字段")]
        public bool ShowField
        {
            get { return _showField; }
            set
            {
                _showField = value;
                this.OnRaisePropertyChanged(nameof(ShowField));
                var handler = ShowFieldChanged;
                if (handler != null)
                {
                    handler(this);
                }
            }
        }

        public DBTableInfoEx()
            : base()
        {

        }

        public DBTableInfoEx(DBTableInfo tableInfo)
            : base(tableInfo)
        {

        }

        public static bool StringContains(string str1, string str2, bool ignorcase)
        {
            if (str1 == null && str2 == null)
            {
                return true;
            }

            if (str1 == null || str2 == null)
            {
                return false;
            }

            if (str1.Contains(str2))
            {
                return true;
            }

            if (!ignorcase)
            {
                return false;
            }

            str1 = str1.ToLower();
            str2 = str2.ToLower();
            return str1.Contains(str2);
        }
    }
}
