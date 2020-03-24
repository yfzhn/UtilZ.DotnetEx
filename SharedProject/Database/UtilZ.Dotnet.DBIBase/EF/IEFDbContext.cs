using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.EF
{
    /// <summary>
    /// EF上下文接口
    /// </summary>
    public interface IEFDbContext : IDisposable
    {
        /// <summary>
        /// 设置为手动提交事务
        /// </summary>
        void Manual();

        /// <summary>
        /// 手动提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 插入实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        void Insert<T>(T entity) where T : class;

        /// <summary>
        /// 批量插入实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">实体对象集合</param>
        void InsertBulk<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        void Update<T>(T entity) where T : class;

        /// <summary>
        /// 更新实体对象,根据条件表达式及更新表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">更新条件表达式</param>
        /// <param name="updateExpression">更新表达式</param>
        void Update<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression) where T : class;

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="id">主键集合</param>
        void Delete<T>(params object[] id) where T : class;

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        void Delete<T>(T entity) where T : class;

        /// <summary>
        /// 删除实体对象,根据条件表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">删除条件表达式</param>
        void Delete<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="keyValues">The values of the primary key for the entity to be found</param>
        /// <returns>The entity found, or null.</returns>
        T QueryOne<T>(params object[] keyValues) where T : class;

        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>IQueryable</returns>
        IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate = null) where T : class;

        /// <summary>
        /// 根据表达式查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="includeProperty">要深度查询的属性名称集合</param>
        /// <param name="predicate">查询条件表达式</param>
        /// <returns>IQueryable</returns>
        IQueryable<T> Query<T>(IEnumerable<string> includeProperty, Expression<Func<T, bool>> predicate = null) where T : class;

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
        IQueryable<T> Query<T, TKey>(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out int total, Expression<Func<T, TKey>> orderKey, DbOrderType order = DbOrderType.Asc) where T : class;

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
        IQueryable<T> Query<T, TKey>(IEnumerable<string> includeProperty, Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out int total, Expression<Func<T, TKey>> orderKey, DbOrderType order = DbOrderType.Asc) where T : class;
    }
}
