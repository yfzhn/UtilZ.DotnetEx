using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UtilZ.Dotnet.DBIBase.Config;
using UtilZ.Dotnet.DBIBase.Connection;
using UtilZ.Dotnet.DBIBase.Model;
using UtilZ.Dotnet.Ex.Log;
using Z.EntityFramework.Plus;

namespace UtilZ.Dotnet.DBIBase.EF
{
    /// <summary>
    /// EF上下文实现类
    /// </summary>
    [DbConfigurationType(typeof(EFDbConfiguration))]
    public class EFDbContext : DbContext, IEFDbContext
    {
        /// <summary>
        /// 连接信息对象
        /// </summary>
        protected readonly IDbConnectionInfo _conInfo;

        /// <summary>
        /// 是否自动提交事务[true:自动;false:手动]
        /// </summary>
        protected bool _autoCommit = true;

        /// <summary>
        /// 自定义注册EF实体类型回调,已自定义注册实体返回true;否则返回false
        /// </summary>
        protected readonly Func<DatabaseConfig, DbModelBuilder, bool> _customRegisteEntityTypeFunc = null;

        /// <summary>
        /// 是否输出EF日志[true:输出;false:不输出]
        /// </summary>
        protected static bool _outputLog = false;

        /// <summary>
        /// 是否输出EF日志[true:输出;false:不输出]
        /// </summary>
        public static bool OutputLog
        {
            get { return _outputLog; }
            set
            {
                if (_outputLog == value)
                {
                    return;
                }

                _outputLog = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbid">数据库ID</param>
        /// <param name="visitType">数据库访问类型</param>
        /// <param name="customRegisteEntityTypeFunc">自定义注册EF实体类型回调,已自定义注册实体返回true;否则返回false</param>
        public EFDbContext(int dbid, DBVisitType visitType, Func<DatabaseConfig, DbModelBuilder, bool> customRegisteEntityTypeFunc)
            : this(new UtilZ.Dotnet.DBIBase.Connection.DbConnectionInfo(dbid, visitType), customRegisteEntityTypeFunc)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="conInfo">连接信息对象</param>
        /// <param name="customRegisteEntityTypeFunc">自定义注册EF实体类型回调,已自定义注册实体返回true;否则返回false</param>
        public EFDbContext(IDbConnectionInfo conInfo, Func<DatabaseConfig, DbModelBuilder, bool> customRegisteEntityTypeFunc)
            : base(conInfo.DbConnection, false)
        {
            this._conInfo = conInfo;
            this._customRegisteEntityTypeFunc = customRegisteEntityTypeFunc;
            Database.SetInitializer<EFDbContext>(null);
            this.RegisteLogOutput();
        }

        private void RegisteLogOutput()
        {
            if (!_outputLog)
            {
                return;
            }

            base.Database.Log = (logStr) =>
            {
                int index = logStr.LastIndexOf(Environment.NewLine);
                if (index > 0)
                {
                    logStr = logStr.Remove(index, logStr.Length - index);
                }

                Loger.Info(logStr);
            };
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized,
        /// but before the model has been locked down and used to initialize the context.
        /// The default implementation of this method does nothing, but it can be overridden
        /// in a derived class such that the model can be further configured before it is
        /// locked down.需要重写此方法时,请重写EFOnModelCreating方法
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        protected sealed override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.EFOnModelCreating(modelBuilder);
        }

        /// <summary>
        /// EF数据库上下文模型创建
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void EFOnModelCreating(DbModelBuilder modelBuilder)
        {
            var config = Config.DatabaseConfigManager.GetConfig(this._conInfo.DBID);

            //上下文模型创建
            var dbFactory = Factory.DBFactoryManager.GetDBFactory(config);
            var dbInteraction = dbFactory.GetDBInteraction();
            dbInteraction.EFSetting(modelBuilder);

            //注册数据模型类型
            if (this._customRegisteEntityTypeFunc == null || !this._customRegisteEntityTypeFunc(config, modelBuilder))
            {
                Type[] entityTypes = EFEntityTypeManager.GetEntityTypes(this._conInfo.DBID);
                dbInteraction.EFRegisteEntityType(modelBuilder, entityTypes);
            }
        }

        #region 辅助方法
        /// <summary>
        /// 分析异常信息
        /// </summary>
        /// <param name="ex">原始异常</param>
        /// <returns>EFDbContextException</returns>
        protected EFDbContextException AnalyzeException(Exception ex)
        {
            EFDbContextException exception;
            if (ex is DbEntityValidationException)
            {
                DbEntityValidationException e = (DbEntityValidationException)ex;
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var validationError in e.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        stringBuilder.AppendLine(error.ErrorMessage);
                    }
                }

                exception = new EFDbContextException(stringBuilder.ToString(), ex);
            }
            else if (ex is DataException)
            {
                Exception tmpEx = ex;
                while (tmpEx.InnerException != null)
                {
                    tmpEx = tmpEx.InnerException;
                }

                exception = new EFDbContextException(tmpEx.Message, ex);
            }
            else
            {
                exception = new EFDbContextException(ex.Message, ex);
            }

            return exception;
        }

        /// <summary>
        /// 自动提交更改
        /// </summary>
        protected void AutoCommit()
        {
            if (this._autoCommit)
            {
                this.PrimitiveCommit();
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        protected void PrimitiveCommit()
        {
            var transaction = base.Database.CurrentTransaction;
            try
            {
                base.SaveChanges();
                if (transaction != null)
                {
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
            finally
            {
                try
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Error("事务回滚异常", ex);
                }
            }
        }
        #endregion

        #region IEFDbContext接口
        /// <summary>
        /// 设置为手动提交事务
        /// </summary>
        public void Manual()
        {
            this._autoCommit = false;
            if (this.Database.CurrentTransaction == null)
            {
                this.Database.BeginTransaction();
            }
        }

        /// <summary>
        /// 手动提交事务
        /// </summary>
        public void Commit()
        {
            this.PrimitiveCommit();
        }

        /// <summary>
        /// 插入实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        public void Insert<T>(T entity) where T : class
        {
            try
            {
                this.Set<T>().Add(entity);
                this.AutoCommit();
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 批量插入实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">实体对象集合</param>
        public void InsertBulk<T>(IEnumerable<T> entities) where T : class
        {
            try
            {
                this.Set<T>().AddRange(entities);
                this.AutoCommit();
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        public void Update<T>(T entity) where T : class
        {
            try
            {
                this.Entry(entity).State = EntityState.Modified;
                this.AutoCommit();
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 更新实体对象,根据条件表达式及更新表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">更新条件表达式</param>
        /// <param name="updateExpression">更新表达式</param>
        public void Update<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression) where T : class
        {
            try
            {
                this.Set<T>().Where(predicate).Update(updateExpression);
                this.AutoCommit();
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="id">主键集合</param>
        public void Delete<T>(params object[] id) where T : class
        {
            try
            {
                var entity = this.Set<T>().Find(id);
                if (entity != null)
                {
                    this.Set<T>().Remove(entity);
                    this.AutoCommit();
                }
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        public void Delete<T>(T entity) where T : class
        {
            try
            {
                this.Entry(entity).State = EntityState.Deleted;
                this.AutoCommit();
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 删除实体对象,根据条件表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">删除条件表达式</param>
        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            try
            {
                this.Set<T>().Where(predicate).Delete();
                this.AutoCommit();
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="keyValues">The values of the primary key for the entity to be found</param>
        /// <returns>The entity found, or null.</returns>
        public T QueryOne<T>(params object[] keyValues) where T : class
        {
            try
            {
                return this.Set<T>().Find(keyValues);
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>IQueryable</returns>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            return this.Query<T>(null, predicate);
        }

        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="includeProperty">要深度查询的属性名称集合</param>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>IQueryable</returns>
        public IQueryable<T> Query<T>(IEnumerable<string> includeProperty, Expression<Func<T, bool>> predicate = null) where T : class
        {
            try
            {
                var query = this.Set<T>().AsNoTracking().AsQueryable();
                if (includeProperty != null)
                {
                    foreach (var include in includeProperty)
                    {
                        query = query.Include(include);
                    }
                }

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                return query;
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">排序主键类型</typeparam>
        /// <param name="predicate">条件表达式</param>
        /// <param name="pageIndex">查询目标页数</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="total">满足查询条件的总记录数</param>
        /// <param name="orderKey">排序表达式</param>
        /// <param name="order">排序类型</param>
        /// <returns>IQueryable</returns>
        public IQueryable<T> Query<T, TKey>(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out int total, Expression<Func<T, TKey>> orderKey, DbOrderType order = DbOrderType.Asc) where T : class
        {
            return this.Query<T, TKey>(null, predicate, pageIndex, pageSize, out total, orderKey, order);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">排序主键类型</typeparam>
        /// <param name="includeProperty">要深度查询的属性名称集合</param>
        /// <param name="predicate">条件表达式</param>
        /// <param name="pageIndex">查询目标页数</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="total">满足查询条件的总记录数</param>
        /// <param name="orderKey">排序表达式</param>
        /// <param name="order">排序类型</param>
        /// <returns>IQueryable</returns>
        public IQueryable<T> Query<T, TKey>(IEnumerable<string> includeProperty, Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out int total, Expression<Func<T, TKey>> orderKey, DbOrderType order = DbOrderType.Asc) where T : class
        {
            try
            {
                if (pageIndex <= 0)
                {
                    pageIndex = 1;
                }

                var query = this.Set<T>().AsNoTracking().AsQueryable();
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                total = query.Count();
                if (includeProperty != null)
                {
                    foreach (var include in includeProperty)
                    {
                        query = query.Include(include);
                    }
                }

                if (pageSize <= 0)
                {
                    return query;
                }

                if (order == DbOrderType.Asc)
                {
                    query = query.OrderBy(orderKey);
                }
                else
                {
                    query = query.OrderByDescending(orderKey);
                }

                query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                return query;
            }
            catch (Exception ex)
            {
                throw this.AnalyzeException(ex);
            }
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放资源标识</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this._conInfo.Dispose();
        }
    }
}
