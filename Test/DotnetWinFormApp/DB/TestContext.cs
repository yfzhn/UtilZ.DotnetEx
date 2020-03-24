namespace DotnetWinFormApp.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using UtilZ.Dotnet.DBIBase.EF;
    using UtilZ.Dotnet.DBIBase.Model;
    using System.Data.Entity.ModelConfiguration;
    using UtilZ.Dotnet.DBIBase.Connection;
    using UtilZ.Dotnet.DBIBase.Config;

    public partial class TestContext : EFDbContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbid">数据库ID</param>
        /// <param name="visitType">数据库访问类型</param>
        public TestContext(int dbid, DBVisitType visitType) :
            base(dbid, visitType, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="conInfo">连接信息对象</param>
        public TestContext(IDbConnectionInfo conInfo)
            : base(conInfo, null)
        {

        }

        //public virtual DbSet<Stu> Stu { get; set; }
        protected override void EFOnModelCreating(DbModelBuilder modelBuilder)
        {
            if (base._conInfo.DBID == 3)
            {
                modelBuilder.HasDefaultSchema(string.Empty);
                //modelBuilder.Entity<Stu>().Property(p => p.ID).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

                //modelBuilder.Entity<Stu>().ToTable(nameof(Stu).ToUpper(), string.Empty);
                //modelBuilder.Entity<Stu>().Property(p => p.ID).HasColumnName(nameof(Stu.ID).ToUpper()).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                //modelBuilder.Entity<Stu>().Property(p => p.Name).HasColumnName(nameof(Stu.Name).ToUpper());
                //modelBuilder.Entity<Stu>().Property(p => p.Age).HasColumnName(nameof(Stu.Age).ToUpper());
                //modelBuilder.Entity<Stu>().Property(p => p.Bir).HasColumnName(nameof(Stu.Bir).ToUpper());

                modelBuilder.Entity<StuOracle>().ToTable(nameof(Stu).ToUpper(), string.Empty);
                modelBuilder.Entity<StuOracle>().Property(p => p.ID).HasColumnName(nameof(StuOracle.ID).ToUpper()).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                modelBuilder.Entity<StuOracle>().Property(p => p.Name).HasColumnName(nameof(StuOracle.Name).ToUpper());
                modelBuilder.Entity<StuOracle>().Property(p => p.Age).HasColumnName(nameof(StuOracle.Age).ToUpper());
                modelBuilder.Entity<StuOracle>().Property(p => p.Bir).HasColumnName(nameof(StuOracle.Bir).ToUpper());
            }
            else
            {
                modelBuilder.Entity<Stu>();
            }
        }
    }
}
