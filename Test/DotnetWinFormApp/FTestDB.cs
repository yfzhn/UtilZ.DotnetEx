using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.EF;
using UtilZ.Dotnet.DBIBase.Interface;
using UtilZ.Dotnet.DBIBase.Factory;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.Ex.Log;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.DBIBase;
using UtilZ.Dotnet.DBIBase.ExpressionTree;
using UtilZ.Dotnet.DBIBase.Interaction;
using UtilZ.Dotnet.DBIBase.ExpressionTree.CompareOperaterWhereGenerator;
using System.IO;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid;

namespace DotnetWinFormApp
{
    public partial class FTestDB : Form
    {
        public FTestDB()
        {
            InitializeComponent();
        }

        private void FTest_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            var redirectAppenderToUI = (RedirectAppender)Loger.GetAppenderByName(null, "RedirectToUI");
            if (redirectAppenderToUI != null)
            {
                redirectAppenderToUI.RedirectOuput += RedirectAppenderToUI_RedirectOuput; ;
            }


            List<DatabaseConfig> itemList = DatabaseConfigManager.GetAllConfigItems();
            DropdownBoxHelper.BindingIEnumerableGenericToComboBox<DatabaseConfig>(comboBoxDB, itemList, nameof(DatabaseConfig.ConName), itemList.Where(t => { return t.DBID == _pssqlDbid; }).FirstOrDefault());

            EFDbContext.OutputLog = true;

            EFEntityTypeManager.RegisterEntityType(3, typeof(DotnetWinFormApp.DB.Stu));
            EFEntityTypeManager.RegisterEntityType(5, typeof(DotnetWinFormApp.DB.Stu));
        }

        private void RedirectAppenderToUI_RedirectOuput(object sender, RedirectOuputArgs e)
        {
            try
            {
                if (e == null || e.Item == null)
                {
                    return;
                }

                string logInfo = string.Format("{0} {1} {2}", e.Item.Time.ToString("yyyy-MM-dd HH:mm:ss"), LogConstant.GetLogLevelName(e.Item.Level), e.Item.Content);
                logControl.AddLog(logInfo, e.Item.Level);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private readonly int _oracleDbid = 3;
        private readonly int _pssqlDbid = 5;
        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                //string pluginAssemblyPath = @"D:\qwe\abc.dll";
                //DBAccessManager.LoadDBPlugin(pluginAssemblyPath);

                //pluginAssemblyPath = @"qwe\abc.dll";
                //DBAccessManager.LoadDBPlugin(pluginAssemblyPath);

                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                //IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                //string sql = @"SELECT A.* FROM user_indexes A WHERE TABLE_NAME='STU'";
                //DataTable dt = dbAccess.QueryDataToDataTable(sql);
                //DataColumn[] colArr = new DataColumn[dt.Columns.Count];
                //dt.Columns.CopyTo(colArr, 0);
                //string[] colNameArr = colArr.Select(t => { return t.ColumnName; }).ToArray();
                //string str = string.Join(",A.", colNameArr);
                //Loger.Info(str);

                //IDBInteraction interaction = DBFactoryManager.GetDBFactory(config).GetDBInteraction();
                //string conStr = interaction.GenerateDBConStr(config, DBVisitType.R);
                //Port=5432;Host=localhost;Username=postgres;Password=qwe123;Database=postgres
                //using (var con = new Npgsql.NpgsqlConnection(conStr))
                //{
                //    con.Open();
                //    var cmd = con.CreateCommand();
                //    cmd.CommandText = "select CURRENT_TIMESTAMP";
                //    object obj = cmd.ExecuteScalar();
                //}


                bool ret;
                try
                {
                    //var config = DatabaseConfigManager.GetConfig(Config.Instance.DBID);
                    UtilZ.Dotnet.DBIBase.Factory.IDBFactory dbFactory = UtilZ.Dotnet.DBIBase.Factory.DBFactoryManager.GetDBFactory(config);
                    UtilZ.Dotnet.DBIBase.Interaction.IDBInteraction dbInteraction = dbFactory.GetDBInteraction();
                    using (var con = dbInteraction.GetProviderFactory().CreateConnection())
                    {
                        string confStr = dbInteraction.GenerateDBConStr(config, DBVisitType.R);
                        con.ConnectionString = confStr;
                        con.Open();
                        ret = con.State == ConnectionState.Open;
                    }
                }
                catch(Exception exi)
                {
                    ret = false;
                    Loger.Error(exi);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnDataBaseSysTime_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                DateTime sysTime = dbAccess.Database.GetDataBaseSysTime();
                Loger.Info($"[{config.ConName}] [{sysTime.ToString()}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }


        private bool CustomRegisteEntityTypeFunc(DatabaseConfig config, DbModelBuilder modelBuilder)
        {
            if (config.DBID == this._oracleDbid)
            {
                //Oracel:创建表时,表名和字段名如果使用双引号修饰,则创建时字段名及表名不作转换,否则将会全部转换为全大写的形式,下面第一块是是插入表STU,后面是插入表Stu

                modelBuilder.Entity<DotnetWinFormApp.DB.StuOracle>().ToTable(nameof(DotnetWinFormApp.DB.Stu).ToUpper(), string.Empty);
                modelBuilder.Entity<DotnetWinFormApp.DB.StuOracle>().Property(p => p.ID).HasColumnName(nameof(DotnetWinFormApp.DB.StuOracle.ID)).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                modelBuilder.Entity<DotnetWinFormApp.DB.StuOracle>().Property(p => p.Name).HasColumnName(nameof(DotnetWinFormApp.DB.StuOracle.Name));
                modelBuilder.Entity<DotnetWinFormApp.DB.StuOracle>().Property(p => p.Age).HasColumnName(nameof(DotnetWinFormApp.DB.StuOracle.Age));
                modelBuilder.Entity<DotnetWinFormApp.DB.StuOracle>().Property(p => p.Bir).HasColumnName(nameof(DotnetWinFormApp.DB.StuOracle.Bir));

                //modelBuilder.Entity<StuOracle>().ToTable("Stu", string.Empty);
                //modelBuilder.Entity<StuOracle>().Property(p => p.ID).HasColumnName("Id").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            }
            else if (config.DBID == _pssqlDbid)
            {
                //pstsql 全小写
                modelBuilder.Entity<DotnetWinFormApp.DB.Stu>().ToTable(nameof(DotnetWinFormApp.DB.Stu).ToLower(), string.Empty);
                modelBuilder.Entity<DotnetWinFormApp.DB.Stu>().Property(p => p.ID).HasColumnName(nameof(DotnetWinFormApp.DB.Stu.ID).ToLower());
                modelBuilder.Entity<DotnetWinFormApp.DB.Stu>().Property(p => p.Name).HasColumnName(nameof(DotnetWinFormApp.DB.Stu.Name).ToLower());
                modelBuilder.Entity<DotnetWinFormApp.DB.Stu>().Property(p => p.Age).HasColumnName(nameof(DotnetWinFormApp.DB.Stu.Age).ToLower());
                modelBuilder.Entity<DotnetWinFormApp.DB.Stu>().Property(p => p.Bir).HasColumnName(nameof(DotnetWinFormApp.DB.Stu.Bir).ToLower());
                //PrimitiveEFRegisteEntityType(modelBuilder, new Type[] { typeof(Stu) });
            }
            else
            {
                modelBuilder.Entity<DotnetWinFormApp.DB.Stu>();
                //Expression<Func<Stu, long>> ee = p => p.ID;
            }

            return true;
        }

        /// <summary>
        /// 注册实体类型
        /// </summary>
        /// <param name="modelBuilder">DbModelBuilder</param>
        /// <param name="entityTypeArr">EF实体类型数组</param>
        protected void PrimitiveEFRegisteEntityType(DbModelBuilder modelBuilder, Type[] entityTypeArr)
        {
            //此处没想到更好的解决方案,目前只得在自定义注册实体类型或实体类定义处通过Attribute特性设置字段及表名
            throw new NotImplementedException("PostgreSQL数据库必须自定义注册实体类型,内置转换未找到相应的方法");
            //Type databaseGeneratedAttributeType = typeof(DatabaseGeneratedAttribute);
            //Type keyAttributeType = typeof(KeyAttribute);
            //var _dbModelBuilder_Entity_MethodInfo = typeof(DbModelBuilder).GetMethod(nameof(DbModelBuilder.Entity));

            ////pstsql:默认表名字段名全小写
            //foreach (var entityType in entityTypeArr)
            //{

            //    //修改泛型方法类型
            //    MethodInfo makeGenericMethod = _dbModelBuilder_Entity_MethodInfo.MakeGenericMethod(entityType);
            //    object obj = makeGenericMethod.Invoke(modelBuilder, new object[] { });//obj is EntityTypeConfiguration<T>

            //    //调用ToTable方法,指定表名
            //    Type etcType = obj.GetType();
            //    Type[] paraTypeArr = new Type[] { typeof(string), typeof(string) };
            //    MethodInfo etc_ToTable_MethodInfo = etcType.GetMethod(nameof(EntityTypeConfiguration<string>.ToTable), paraTypeArr);
            //    string newTblName = ((TableAttribute)entityType.GetCustomAttribute(typeof(TableAttribute), true)).Name.ToLower();
            //    obj = etc_ToTable_MethodInfo.Invoke(obj, new object[] { newTblName, string.Empty });//obj is EntityTypeConfiguration<T>


            //    //调用Property.HasColumnName方法,指定列名
            //    PropertyInfo[] propertyInfoArr = entityType.GetProperties();
            //    foreach (var propertyInfo in propertyInfoArr)
            //    {
            //        try
            //        {
            //            PropertyInfo idPropertyInfo = entityType.GetProperty(propertyInfo.Name);
            //            var exParameter = Expression.Parameter(entityType, "p");
            //            var meEx = Expression.Lambda(Expression.Property(exParameter, propertyInfo.Name), exParameter);


            //            Type[] paraTypeArr2 = new Type[] { meEx.GetType() };
            //            MethodInfo etc_Property_MethodInfo = etcType.GetMethod(nameof(EntityTypeConfiguration<string>.Property), paraTypeArr2);

            //            MethodInfo[] etc_Property_MethodInfoArr = etcType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => { return m.IsGenericMethod && string.Equals(m.Name, nameof(EntityTypeConfiguration<string>.Property)); }).ToArray();
            //            //etc_Property_MethodInfoArr[0].GetParameters()[0].Member.`
            //            etc_Property_MethodInfo = etc_Property_MethodInfoArr[0];
            //            obj = etc_Property_MethodInfo.Invoke(obj, new object[] { meEx });//obj is PrimitivePropertyConfiguration

            //            //var idPropertyInfoAccess = Expression.MakeMemberAccess(exParameter, idPropertyInfo);
            //            //var meEx = Expression.Lambda(idPropertyInfoAccess, exParameter);

            //            //Expression<Func<TStructuralType, T>> propertyExpression
            //        }
            //        catch (Exception ex)
            //        {
            //            //var att = propertyInfo.GetCustomAttribute(databaseGeneratedAttributeType);
            //            //if (att != null)
            //            //{
            //            //    //主键
            //            //    switch (((DatabaseGeneratedAttribute)att).DatabaseGeneratedOption)
            //            //    {
            //            //        case DatabaseGeneratedOption.None://数据库不生成值
            //            //            //modelBuilder.Entity<Stu>().Property(p => p.ID).HasColumnName(nameof(StuOracle.ID).ToLower()).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            //            //            break;
            //            //        case DatabaseGeneratedOption.Computed://插入或更新时数据库会生成一个值
            //            //        case DatabaseGeneratedOption.Identity://插入一行时数据库会生成一个值                                    
            //            //        default:
            //            //            //throw new NotSupportedException($"未知的DatabaseGeneratedOption值[{((DatabaseGeneratedAttribute)att).DatabaseGeneratedOption.ToString()}]");
            //            //            break;
            //            //    }
            //            //}
            //            //else
            //            //{
            //            //    att = propertyInfo.GetCustomAttribute(keyAttributeType);
            //            //    if (att != null)
            //            //    {
            //            //        //主键
            //            //    }
            //            //    else
            //            //    {
            //            //        if (propertyInfo.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase))
            //            //        {
            //            //            //主键
            //            //        }
            //            //        else
            //            //        {

            //            //        }
            //            //    }
            //            //}
            //        }
            //    }
            //}
        }


        private void btnTestEF_Click(object sender, EventArgs e)
        {
            var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
            Task.Factory.StartNew(TestEF, config);
        }

        private void TestEF(object state)
        {
            try
            {
                var config = (DatabaseConfig)state;
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);

                //    //ManualSettingPKValue
                //    //ManualAssignPKValue
                //    //Expression<Func<Stu, long>> expression = t => t.ID;
                //    //((EFDbContext)context)._Expression = expression;

                //    using (var con = dbAccess.CreateConnection(DBVisitType.W))
                //    {
                //        var paraSign = dbAccess.ParaSign;
                //        var cmd = con.DbConnection.CreateCommand();
                //        cmd.CommandText = $@"INSERT INTO STU (ID,NAME,AGE,BIR) VALUES({paraSign}ID,{paraSign}NAME,{paraSign}AGE,{paraSign}BIR)";
                //        cmd.AddParameter("ID", 2);
                //        cmd.AddParameter("NAME", "yf");
                //        cmd.AddParameter("AGE", 22);
                //        cmd.AddParameter("BIR", new DateTime(1988, 11, 30, 2, 22, 45));
                //        cmd.ExecuteNonQuery();
                //    }





                //if (config.DBID == this._oracleDbid)
                //{
                //    using (var context = dbAccess.CreateEFDbContext(DBVisitType.R, (cfg, modelBuilder) =>
                //    {
                //        modelBuilder.Entity<StuOracle>().ToTable(nameof(Stu).ToUpper(), string.Empty);
                //        modelBuilder.Entity<StuOracle>().Property(p => p.ID).HasColumnName(nameof(StuOracle.ID).ToUpper()).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                //        modelBuilder.Entity<StuOracle>().Property(p => p.Name).HasColumnName(nameof(StuOracle.Name).ToUpper());
                //        modelBuilder.Entity<StuOracle>().Property(p => p.Age).HasColumnName(nameof(StuOracle.Age).ToUpper());
                //        modelBuilder.Entity<StuOracle>().Property(p => p.Bir).HasColumnName(nameof(StuOracle.Bir).ToUpper());
                //    }))
                //    {
                //        var stu = new StuOracle();
                //        this.SetValue(stu);
                //        context.Insert(stu);
                //    }
                //}
                //else
                //{
                //    using (var context = new TestContext(config.DBID, DBVisitType.W))
                //    {
                //        var stu = new Stu();
                //        this.SetValue(stu);
                //        context.Insert(stu);
                //    }
                //}




                using (var context = dbAccess.CreateEFDbContext(DBVisitType.R, this.CustomRegisteEntityTypeFunc))
                {
                    if (config.DBID == this._oracleDbid)
                    {
                        //AssemblyEx.Enable = true;
                        var stu = new DotnetWinFormApp.DB.StuOracle();
                        this.SetValue(stu);
                        context.Insert(stu);

                        //var stuList = new List<StuOracle>();
                        //Random rnd = new Random();
                        //long id = 107;
                        //for (int i = 0; i < 1000; i++)
                        //{
                        //    var stu = new StuOracle();
                        //    this.SetValue(stu, id++);
                        //    stuList.Add(stu);
                        //}
                        //context.InsertBulk(stuList);
                    }
                    else
                    {
                        var stu = new DotnetWinFormApp.DB.Stu();
                        this.SetValue(stu);
                        context.Insert(stu);

                        //var stuList = new List<Stu>();
                        //Random rnd = new Random();
                        //for (int i = 0; i < 1000; i++)
                        //{
                        //    var stu = new Stu();
                        //    this.SetValue(stu);
                        //    stuList.Add(stu);
                        //}
                        //context.InsertBulk(stuList);
                    }
                }


                //using (var context = new TestContext(config.DBID, DBVisitType.W))
                //{
                //    if (config.DBID == this._oracleDbid)
                //    {
                //        var stu = new StuOracle();
                //        this.SetValue(stu);
                //        context.Insert(stu);
                //    }
                //    else
                //    {
                //        var stu = new Stu();
                //        this.SetValue(stu);
                //        context.Insert(stu);
                //    }
                //}


                Loger.Info("Insert Completed...");

                //var stus = context.Query<Stu>().ToArray();
                //if (collection != null && collection.Count > 0)
                //{
                //    List<IDbDataParameter> paras = new List<IDbDataParameter>();
                //    foreach (var para in collection)
                //    {
                //        paras.Add(this._interaction.CreateDbParameter(para));
                //    }

                //    return ((System.Data.Entity.DbContext)context).Database.SqlQuery<T>(sqlStr, paras.ToArray()).ToList();
                //}
                //else
                //{
                //    return ((System.Data.Entity.DbContext)context).Database.SqlQuery<T>(sqlStr, null).ToList();
                //}
                // }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private readonly Random _rnd = new Random();
        private void SetValue(DotnetWinFormApp.DB.IStu stu, long id = -1)
        {
            if (id == -1)
            {
                id = TimeEx.GetTimestamp();
            }

            stu.ID = id;
            stu.Age = _rnd.Next(0, 120);
            stu.Name = "yf" + id;
            //stu.Bir = new DateTime(1988, 11, 30, 2, 22, 45);
            stu.Bir = DateTime.Now;
        }

        private void btnDatabaseVer_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                var dataBaseVersion = dbAccess.Database.GetDataBaseVersion();
                Loger.Info($"[{config.ConName}] [{dataBaseVersion.Version}] [{dataBaseVersion.VersionInfo}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnTableInfo_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                var tableInfoList = dbAccess.Database.GetTableInfoList(ckGetFieldInfo.Checked);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("----------------------------------------------------------------------------");
                sb.AppendLine($"[{config.ConName}] [{tableInfoList.Count}]");
                sb.AppendLine("----------------------------------------------------------------------------");

                foreach (var tableInfo in tableInfoList)
                {
                    sb.AppendLine($"[TableName:{tableInfo.Name}]");
                    sb.AppendLine($"[Comments:{tableInfo.Comments}]");
                    sb.AppendLine($"[PriKeyField:{string.Join(";", tableInfo.PriKeyFieldInfos.Select(t => { return t.FieldName; })) }]");
                    sb.AppendLine($"[Field:{string.Join(";", tableInfo.DbFieldInfos.Select(t => { return t.FieldName; })) }]");
                    string tmp = tableInfo.Indexs == null ? string.Empty : string.Join("; ", tableInfo.Indexs.Select(t => { return $"{t.IndexName}:{t.FieldName}"; }));
                    sb.AppendLine($"[Index:{tmp }]");
                    sb.AppendLine("----------------------------------------------------------------------------");
                }

                Loger.Info(sb.ToString());
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnExistTable_Click(object sender, EventArgs e)
        {
            try
            {
                string tblName = txtTblName.Text;
                if (checkBoxIgnoreCase.Checked)
                {
                    tblName = tblName.ToUpper();
                }

                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                var isExist = dbAccess.Database.ExistTable(tblName);
                Loger.Info($"[{config.ConName}] [{txtTblName.Text}] [{isExist}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnExistField_Click(object sender, EventArgs e)
        {
            try
            {
                string tblName = txtTblName.Text;
                string fieldName = txtFieldName.Text;
                if (checkBoxIgnoreCase.Checked)
                {
                    tblName = tblName.ToUpper();
                    fieldName = fieldName.ToUpper();
                }

                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                var isExist = dbAccess.Database.ExistField(tblName, fieldName);
                Loger.Info($"[{config.ConName}] [{tblName}] [{fieldName}] [{isExist}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private IDBAccess _dbAccess = null;
        private int _pageSize = 10;
        private void btnPagingQuery_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                this._dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                this.QueryPageInfo(this._dbAccess);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void QueryPageInfo(IDBAccess dbAccess)
        {
            try
            {
                if (dbAccess == null)
                {
                    dgv.SetPageInfo(null);
                    return;
                }

                string sql = "select count(0) from stu";
                var pageInfo = dbAccess.QueryPageInfo(_pageSize, sql);
                Loger.Info($"[{dbAccess.Config.ConName}] [TotalCount:{pageInfo.TotalCount}] [PageCount:{pageInfo.PageCount}] [PageSize:{pageInfo.PageSize}]");
                dgv.SetPageInfo(new PageInfo(_pageSize, 1, pageInfo.TotalCount));
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void dgv_PageSizeChanged(object sender, PageSizeChangedArgs e)
        {
            _pageSize = e.PageSize;
            this.QueryPageInfo(this._dbAccess);
        }

        private void dgv_QueryData(object sender, QueryDataArgs e)
        {
            try
            {
                if (this._dbAccess == null)
                {
                    return;
                }

                string sql = "select * from stu";
                DataTable dt = this._dbAccess.QueryPagingData(sql, null, _pageSize, e.PageIndex);
                dgv.ShowData(dt);

                //using (var context = this._dbAccess.CreateEFDbContext(DBVisitType.R, this.OnModelCreatingFunc))
                //{
                //    object dataSource;
                //    if (this._dbAccess.Config.DBID == this._oracleDbid)
                //    {
                //        dataSource = context.Query<StuOracle>().OrderBy(t => t.ID).Skip((e.PageIndex - 1) * _pageSize).Take(_pageSize).ToArray();
                //    }
                //    else
                //    {
                //        dataSource = context.Query<Stu>().OrderBy(t => t.ID).Skip((e.PageIndex - 1) * _pageSize).Take(_pageSize).ToArray();
                //    }

                //    dgv.ShowData(dataSource);
                //}

                Loger.Info($"[{this._dbAccess.Config.ConName}] [dt.Rows.Count:{dgv.GridControl.Rows.Count}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (this._dbAccess == null)
                {
                    return;
                }

                var count = this._dbAccess.Update("stu", "ID", 1, "Name", "张成风");
                Loger.Info($"[{this._dbAccess.Config.ConName}] [受影响行数:{count}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void FTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            DBAccessManager.Release();
        }

        private void btnDBQueryExpression_Click(object sender, EventArgs e)
        {
            try
            {
                //                select treea.ID,treea.Name,treea.Age,treeb.City,treeb.Level From treea INNER JOIN treeb on treea.ID = treeb.AID
                //where treea.Age > 23 AND treeb.Level < 5


                //select A.ID,A.Name,A.Age,B.City,B.Level From treea A INNER JOIN treeb B on A.ID = B.AID
                //where A.Age > 23 AND B.Level < 5

                //string tblA = "TreeA";
                //string tblB = "TreeB";
                string tblA = "treea";
                string tblB = "treeb";

                var queryExpression = new DBQueryExpression();
                queryExpression.QueryFields.Add(new DBQueryField(tblA, "ID"));
                queryExpression.QueryFields.Add(new DBQueryField(tblA, "Name"));
                queryExpression.QueryFields.Add(new DBQueryField(tblA, "Age"));

                queryExpression.QueryFields.Add(new DBQueryField(tblB, "City"));
                queryExpression.QueryFields.Add(new DBQueryField(tblB, "Level"));

                queryExpression.ConditionExpressionNodes.LogicOperaters = LogicOperaters.AND;
                queryExpression.ConditionExpressionNodes.Add(new ExpressionNode(CompareOperater.GreaterThan, tblA, "Age", 23));
                queryExpression.ConditionExpressionNodes.Add(new ExpressionNode(CompareOperater.LessThan, tblB, "Level", 5));

                var tableAliaNameDic = new Dictionary<string, string>();
                tableAliaNameDic.Add("treea", "A");
                tableAliaNameDic.Add("treeb", "B");
                string fieldSql = queryExpression.QueryFields.ToQueryFieldSql(tableAliaNameDic);
                Loger.Info(fieldSql);

                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                var dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);

                List<DBTableInfo> dbTableInfos = dbAccess.Database.GetTableInfoList(true);
                var dbTableInfoDic = dbTableInfos.ToDictionary(k => { return k.Name; }, v => { return v.DbFieldInfos.ToDictionary(t => { return t.FieldName; }); });

                var filedValueConverterCollection = new DBFiledValueConverterCollection();


                Dictionary<string, object> parameterNameValueDic;
                int parameterIndex = 1;
                string where = queryExpression.ConditionExpressionNodes.ToWhere(dbTableInfoDic, tableAliaNameDic,
                    filedValueConverterCollection, dbAccess.ParaSign, ref parameterIndex, out parameterNameValueDic);
                Loger.Info(where);

                ISqlFieldValueFormator fieldValueFormator = DBAccessManager.GetFieldValueFormator(config);
                string whereNoPara = queryExpression.ConditionExpressionNodes.ToWhereNoParameter(dbTableInfoDic, tableAliaNameDic, filedValueConverterCollection, fieldValueFormator);
                Loger.Info(whereNoPara);




                //-------------------------------------------------------------------------------------------------------------------------------------


                //A.Age > 23 AND (B.Level < 4 OR B.Level > 6)
                var expressionNodeCollection = new ExpressionNodeCollection();
                expressionNodeCollection.LogicOperaters = LogicOperaters.AND;
                expressionNodeCollection.Add(new ExpressionNode(CompareOperater.GreaterThan, tblA, "Age", 23));

                var orCondition = new ExpressionNode();
                orCondition.Children.LogicOperaters = LogicOperaters.OR;
                orCondition.Children.Add(new ExpressionNode(CompareOperater.LessThan, tblB, "Level", 4));
                orCondition.Children.Add(new ExpressionNode(CompareOperater.GreaterThan, tblB, "Level", 6));
                expressionNodeCollection.Add(orCondition);

                parameterNameValueDic = null;
                parameterIndex = 1;
                string where2 = expressionNodeCollection.ToWhere(dbTableInfoDic, tableAliaNameDic, filedValueConverterCollection, dbAccess.ParaSign, ref parameterIndex, out parameterNameValueDic);
                Loger.Info(where2);

                string whereNoPara2 = expressionNodeCollection.ToWhereNoParameter(dbTableInfoDic, tableAliaNameDic, filedValueConverterCollection, fieldValueFormator);
                Loger.Info(whereNoPara2);

                tableAliaNameDic.Clear();
                parameterNameValueDic = null;
                parameterIndex = 1;
                string where3 = expressionNodeCollection.ToWhere(dbTableInfoDic, tableAliaNameDic, filedValueConverterCollection, dbAccess.ParaSign, ref parameterIndex, out parameterNameValueDic);
                Loger.Info(where3);

                string whereNoPara3 = expressionNodeCollection.ToWhereNoParameter(dbTableInfoDic, tableAliaNameDic, filedValueConverterCollection, fieldValueFormator);
                Loger.Info(whereNoPara3);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            logControl.Clear();
        }

        private void btnEF_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                using (var ef = new DotnetWinFormApp.DB.EFModelContext(config.DBID))
                {
                    var actionArr = ef.Action.ToArray();
                    var roleArr = ef.Role.ToArray();

                    //Action
                    var roles = ef.Query<DotnetWinFormApp.DB.Role>(r => r.ID == 2).ToArray();
                    var newRole = new DotnetWinFormApp.DB.Role();
                    newRole.Name = "员工";
                    newRole.Des = "打酱油的员工";
                    if (newRole.Action == null)
                    {
                        newRole.Action = new List<DotnetWinFormApp.DB.Action>();
                    }

                    foreach (var action in actionArr)
                    {
                        newRole.Action.Add(action);
                    }

                    ef.Insert(newRole);

                    var actionArr2 = ef.Action.ToArray();
                    var roleArr2 = ef.Role.ToArray();

                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }

        }

        private void btnDatabasePropertyInfo_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                DatabasePropertyInfo databasePropertyInfo = dbAccess.Database.GetDatabasePropertyInfo();
                Loger.Info($"[{config.ConName}] [{databasePropertyInfo.ToString()}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnUserName_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                string userName = dbAccess.Database.GetLoginUserName();
                Loger.Info($"[{config.ConName}] [LoginUserName:{userName}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnDataBaseName_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                string databaseName = dbAccess.Database.GetDatabaseName();
                Loger.Info($"[{config.ConName}] [DatabaseName:{databaseName}]");
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                var config = DropdownBoxHelper.GetGenericFromComboBox<DatabaseConfig>(comboBoxDB);
                IDBAccess dbAccess = DBAccessManager.GetDBAccessInstance(config.DBID);
                string sql = @"SELECT  ID,SensNo,SensLocation,SensType,AddTime FROM Sensors";
                DataTable dt = dbAccess.QueryDataToDataTable(sql);
                dgv.ShowData(dt);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
