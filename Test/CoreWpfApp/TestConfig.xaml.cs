using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Base.Config;
using UtilZ.Dotnet.Ex.Log;

namespace CoreWpfApp
{
    /// <summary>
    /// TestConfig.xaml 的交互逻辑
    /// </summary>
    public partial class TestConfig : Window
    {
        public TestConfig()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private readonly string _configFilePath = @"config.xml";
        private readonly string _configFilePath2 = @"config2.xml";
        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = new ConfigDemo(true);
                var xdoc = ConfigHelper.WriteConfigToXDocument(config);
                xdoc.Save(this._configFilePath);
                MessageBox.Show("OK");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = ConfigHelper.ReadConfigFromFile<ConfigDemo>(this._configFilePath);
                ConfigHelper.WriteConfigToXmlFile(config, _configFilePath2);
                MessageBox.Show("OK");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStruct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var configStructRoot = new ConfigStructRoot();
                configStructRoot.Age = 23;
                configStructRoot.Interval = 123d;
                //this.Interval2 = null;
                configStructRoot.Interval2 = 465d;
                configStructRoot.Name = "yy";
                configStructRoot.Name2 = "cc";
                configStructRoot.Bir = DateTime.Parse("1234-12-12 12:34:56");

                configStructRoot.Complex = new ConfigChildItem(true);

                configStructRoot.Numbers = new List<int>() { 1, 2, 3 };
                configStructRoot.CodeList = new List<string>() { "aaa", "bbb", "ccc" };

                configStructRoot.Dic = new Dictionary<int, string>(new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(11, "京东"), new KeyValuePair<int, string>(22, null) });
                configStructRoot.Dic2 = new Dictionary<string, int?>(new KeyValuePair<string, int?>[] { new KeyValuePair<string, int?>("tls", 123), new KeyValuePair<string, int?>("chery", null) });

                configStructRoot.Dic3 = new Dictionary<string, List<ConfigChildItem>>();
                configStructRoot.Dic3.Add("List1", new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) });
                configStructRoot.Dic3.Add("List2", new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) });

                configStructRoot.Dic4 = new Dictionary<string, Dictionary<string, ConfigChildItem2>>();
                configStructRoot.Dic4.Add("DicDic1", new Dictionary<string, ConfigChildItem2>(new KeyValuePair<string, ConfigChildItem2>[]
                {
                new KeyValuePair<string, ConfigChildItem2>("tt", new ConfigChildItem2(true)),
                new KeyValuePair<string, ConfigChildItem2>("uu", new ConfigChildItem2(true)),
                }));
                configStructRoot.Dic4.Add("DicDic2", new Dictionary<string, ConfigChildItem2>(new KeyValuePair<string, ConfigChildItem2>[]
                {
                new KeyValuePair<string, ConfigChildItem2>("dd", new ConfigChildItem2(true)),
                new KeyValuePair<string, ConfigChildItem2>("ff", new ConfigChildItem2(true)),
                }));

                configStructRoot.ChildList1 = new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) };
                configStructRoot.ChildList2 = new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) };
                configStructRoot.ChildArr = new ConfigChildItem[] { new ConfigChildItem(true), new ConfigChildItem(true) };
                configStructRoot.ChildArr2 = new ConfigChildItem[] { new ConfigChildItem(true), new ConfigChildItem(true) };
                configStructRoot.Cus = "zj";


                string configFilePath = @"struct_config.xml";
                string configFilePath2 = @"struct_config2.xml";

                ConfigHelper.WriteConfigToXmlFile(configStructRoot, configFilePath);
                var config = ConfigHelper.ReadConfigFromFile<ConfigStructRoot>(configFilePath);
                ConfigHelper.WriteConfigToXmlFile(config, configFilePath2);
                MessageBox.Show("OK");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }


    [ConfigRoot("ConfigStruct", "ConfigStructRoot描述")]
    public struct ConfigStructRoot
    {
        public string Name { get; set; }

        [ConfigItem("年龄", "1-200")]
        public int Age { get; set; }

        public double Interval { get; set; }

        public double? Interval2 { get; set; }


        public string Name2 { get; set; }

        [ConfigItemAttribute("Bir", "生日", AllowNullValueElement = true, ConverterType = typeof(DateTimeConfigValueConverter))]
        public DateTime Bir { get; set; }


        [ConfigObject("Object", "对象")]
        public ConfigChildItem Complex { get; set; }

        [ConfigCollection("Numbers", "Item", "基础类型列表")]
        public List<int> Numbers { get; set; }

        public List<string> CodeList { get; set; }


        public Dictionary<int, string> Dic { get; set; }

        [ConfigCollection("Dictionary", "DictionaryItem", "value可null的字典")]
        public Dictionary<string, int?> Dic2 { get; set; }

        [ConfigCollection("Dic3Dictionary", "Dic3Item", "value为列表")]
        public Dictionary<string, List<ConfigChildItem>> Dic3 { get; set; }

        public Dictionary<string, Dictionary<string, ConfigChildItem2>> Dic4 { get; set; }




        public List<ConfigChildItem> ChildList1 { get; set; }

        [ConfigCollection("ChildList", "Child", "对象列表")]
        public List<ConfigChildItem> ChildList2 { get; set; }

        public ConfigChildItem[] ChildArr { get; set; }

        [ConfigCollection("Array", "Item", "对象数组")]
        public ConfigChildItem[] ChildArr2 { get; set; }


        [ConfigCustomerAttribute(typeof(CusConfigRW))]
        public string Cus { get; set; }
    }



    [ConfigRoot("配置", "根描述")]
    public class ConfigDemo
    {
        [ConfigCommentAttribute]
        public string Comment1 { get; set; } = "=======================================================================================";
        [ConfigCommentAttribute]
        public string Comment2 { get; set; } = "++++++++++++++++++++++++++++++++++++这是第一个注释开始+++++++++++++++++++++++++++++++++";

        [ConfigItem("年龄", "1-200")]
        public int Age { get; set; }


        public double Interval { get; set; }

        public double? Interval2 { get; set; }


        [ConfigCommentAttribute]
        public string Comment3 { get; set; } = "=======================================================================================";

        [ConfigIgnore]
        public string Name { get; set; }

        public string Name2 { get; set; }

        [ConfigItemAttribute("Bir", "生日", AllowNullValueElement = true, ConverterType = typeof(DateTimeConfigValueConverter))]
        public DateTime Bir { get; set; }


        [ConfigObject("Object", "对象")]
        public ConfigChildItem Complex { get; set; }

        [ConfigObject("StructObject", "struct对象")]
        public ConfigStruct Struct { get; set; }

        public ConfigStruct Struct2 { get; set; }


        [ConfigCollection("Numbers", "Item", "基础类型列表")]
        public List<int> Numbers { get; set; }

        public List<string> CodeList { get; set; }


        public Dictionary<int, string> Dic { get; set; }

        [ConfigCollection("Dictionary", "DictionaryItem", "value可null的字典")]
        public Dictionary<string, int?> Dic2 { get; set; }

        [ConfigCollection("Dic3Dictionary", "Dic3Item", "value为列表")]
        public Dictionary<string, List<ConfigChildItem>> Dic3 { get; set; }

        public Dictionary<string, Dictionary<string, ConfigChildItem2>> Dic4 { get; set; }




        public List<ConfigChildItem> ChildList1 { get; set; }

        [ConfigCollection("ChildList", "Child", "对象列表")]
        public List<ConfigChildItem> ChildList2 { get; set; }

        public ConfigChildItem[] ChildArr { get; set; }

        [ConfigCollection("Array", "Item", "对象数组")]
        public ConfigChildItem[] ChildArr2 { get; set; }




        [ConfigCustomerAttribute(typeof(CusConfigRW))]
        public string Cus { get; set; }






        public ConfigDemo()
        {

        }

        public ConfigDemo(bool flag)
        {
            this.Age = 23;
            this.Interval = 123d;
            //this.Interval2 = null;
            this.Interval2 = 465d;
            this.Name = "yy";
            this.Name2 = "cc";
            this.Bir = DateTime.Parse("1234-12-12 12:34:56");

            this.Complex = new ConfigChildItem(true);
            this.Struct = new ConfigStruct() { Name = "wzz", Age = 28 };
            this.Struct2 = new ConfigStruct() { Name = "zzx", Age = 18 };

            this.Numbers = new List<int>() { 1, 2, 3 };
            this.CodeList = new List<string>() { "aaa", "bbb", "ccc" };

            this.Dic = new Dictionary<int, string>(new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(11, "京东"), new KeyValuePair<int, string>(22, null) });
            this.Dic2 = new Dictionary<string, int?>(new KeyValuePair<string, int?>[] { new KeyValuePair<string, int?>("tls", 123), new KeyValuePair<string, int?>("chery", null) });

            this.Dic3 = new Dictionary<string, List<ConfigChildItem>>();
            this.Dic3.Add("List1", new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) });
            this.Dic3.Add("List2", new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) });

            this.Dic4 = new Dictionary<string, Dictionary<string, ConfigChildItem2>>();
            this.Dic4.Add("DicDic1", new Dictionary<string, ConfigChildItem2>(new KeyValuePair<string, ConfigChildItem2>[]
            {
                new KeyValuePair<string, ConfigChildItem2>("tt", new ConfigChildItem2(true)),
                new KeyValuePair<string, ConfigChildItem2>("uu", new ConfigChildItem2(true)),
            }));
            this.Dic4.Add("DicDic2", new Dictionary<string, ConfigChildItem2>(new KeyValuePair<string, ConfigChildItem2>[]
            {
                new KeyValuePair<string, ConfigChildItem2>("dd", new ConfigChildItem2(true)),
                new KeyValuePair<string, ConfigChildItem2>("ff", new ConfigChildItem2(true)),
            }));

            this.ChildList1 = new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) };
            this.ChildList2 = new List<ConfigChildItem>() { new ConfigChildItem(true), new ConfigChildItem(true) };
            this.ChildArr = new ConfigChildItem[] { new ConfigChildItem(true), new ConfigChildItem(true) };
            this.ChildArr2 = new ConfigChildItem[] { new ConfigChildItem(true), new ConfigChildItem(true) };
            this.Cus = "zj";

        }
    }


    public struct ConfigStruct
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }


    public class ConfigChildItem
    {
        [ConfigItem("年龄", "1-200")]
        public int Age { get; set; }

        [ConfigIgnore]
        public string Name { get; set; }

        public string Name2 { get; set; }

        [ConfigItemAttribute("Bir", "生日", AllowNullValueElement = true, ConverterType = typeof(DateTimeConfigValueConverter))]
        public DateTime Bir { get; set; }


        [ConfigObject("Object", "对象")]
        public ConfigChildItem2 Complex { get; set; }

        [ConfigObject("StructObject", "struct对象")]
        public ConfigStruct Struct { get; set; }

        public ConfigStruct Struct2 { get; set; }



        [ConfigCollection("Numbers", "Item", "基础类型列表")]
        public List<int> Numbers { get; set; }

        public List<string> CodeList { get; set; }


        public Dictionary<int, string> Dic { get; set; }

        [ConfigCollection("Dictionary", "DictionaryItem", "value可null的字典")]
        public Dictionary<string, int?> Dic2 { get; set; }

        [ConfigCollection("Dic3Dictionary", "Dic3Item", "value为列表")]
        public Dictionary<string, List<ConfigChildItem2>> Dic3 { get; set; }

        public Dictionary<string, Dictionary<string, ConfigChildItem2>> Dic4 { get; set; }




        public List<ConfigChildItem2> ChildList1 { get; set; }

        [ConfigCollection("ChildList", "Child", "对象列表")]
        public List<ConfigChildItem2> ChildList2 { get; set; }

        public ConfigChildItem2[] ChildArr { get; set; }

        [ConfigCollection("Array", "Item", "对象数组")]
        public ConfigChildItem2[] ChildArr2 { get; set; }


        public string Cus { get; set; }


        public ConfigChildItem()
        {

        }

        public ConfigChildItem(bool flag)
        {
            this.Age = 23;
            this.Name = "yy";
            this.Name2 = "cc";
            this.Bir = DateTime.Parse("2012-12-12 12:34:56");

            this.Complex = new ConfigChildItem2(true);
            this.Struct = new ConfigStruct() { Name = "wzz", Age = 28 };
            this.Struct2 = new ConfigStruct() { Name = "zzx", Age = 18 };

            this.Numbers = new List<int>() { 1, 2, 3 };
            this.CodeList = new List<string>() { "aaa", "bbb", "ccc" };

            this.Dic = new Dictionary<int, string>(new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(11, "京东"), new KeyValuePair<int, string>(22, null) });
            this.Dic2 = new Dictionary<string, int?>(new KeyValuePair<string, int?>[] { new KeyValuePair<string, int?>("tls", 123), new KeyValuePair<string, int?>("chery", null) });

            this.Dic3 = new Dictionary<string, List<ConfigChildItem2>>();
            this.Dic3.Add("List1", new List<ConfigChildItem2>() { new ConfigChildItem2(true), new ConfigChildItem2(true) });
            this.Dic3.Add("List2", new List<ConfigChildItem2>() { new ConfigChildItem2(true), new ConfigChildItem2(true) });

            this.Dic4 = new Dictionary<string, Dictionary<string, ConfigChildItem2>>();
            this.Dic4.Add("DicDic1", new Dictionary<string, ConfigChildItem2>(new KeyValuePair<string, ConfigChildItem2>[]
            {
                new KeyValuePair<string, ConfigChildItem2>("tt", new ConfigChildItem2(true)),
                new KeyValuePair<string, ConfigChildItem2>("uu", new ConfigChildItem2(true)),
            }));
            this.Dic4.Add("DicDic2", new Dictionary<string, ConfigChildItem2>(new KeyValuePair<string, ConfigChildItem2>[]
            {
                new KeyValuePair<string, ConfigChildItem2>("dd", new ConfigChildItem2(true)),
                new KeyValuePair<string, ConfigChildItem2>("ff", new ConfigChildItem2(true)),
            }));

            this.ChildList1 = new List<ConfigChildItem2>() { new ConfigChildItem2(true), new ConfigChildItem2(true) };
            this.ChildList2 = new List<ConfigChildItem2>() { new ConfigChildItem2(true), new ConfigChildItem2(true) };
            this.ChildArr = new ConfigChildItem2[] { new ConfigChildItem2(true), new ConfigChildItem2(true) };
            this.ChildArr2 = new ConfigChildItem2[] { new ConfigChildItem2(true), new ConfigChildItem2(true) };
            this.Cus = "zj_in_cus";
        }
    }

    public class CusConfigRW : IConfigCustomerRW
    {
        public object Read(PropertyInfo propertyInfo, XElement element, ConfigCustomerAttribute attri)
        {
            var ele = element.Element(propertyInfo.Name);
            if (ele == null)
            {
                return null;
            }

            return ele.Value;
        }

        public void Write(PropertyInfo propertyInfo, object value, XElement element, ConfigCustomerAttribute attri)
        {
            element.Add(new XElement(propertyInfo.Name, value));
        }
    }


    public class ConfigChildItem2
    {
        [ConfigItem("ChildItem2.ID", "编号1...9999")]
        public long ID { get; set; }

        [ConfigItemAttribute("Bir", "生日", AllowNullValueElement = true, ConverterType = typeof(DateTimeConfigValueConverter))]
        public DateTime Bir { get; set; }


        public ConfigChildItem2()
        {

        }

        public ConfigChildItem2(bool flag)
        {
            this.ID = TimeEx.GetTimestamp();
            this.Bir = DateTime.Now;
        }
    }





    public class DateTimeConfigValueConverter : IConfigValueConverter
    {
        public object ConvertFrom(PropertyInfo propertyInfo, string value)
        {
            return DateTime.Parse(value);
        }

        public string ConvertTo(PropertyInfo propertyInfo, object value)
        {
            return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
