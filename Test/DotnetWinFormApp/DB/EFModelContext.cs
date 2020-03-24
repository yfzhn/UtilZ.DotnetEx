namespace DotnetWinFormApp.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using UtilZ.Dotnet.DBIBase.EF;
    using UtilZ.Dotnet.DBIBase.Model;

    public partial class EFModelContext : EFDbContext
    {
        public EFModelContext(int dbid)
            : base(dbid, DBVisitType.W, null)
        {
        }

        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<Role> Role { get; set; }

        protected override void EFOnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>()
                .HasMany(e => e.Role)
                .WithMany(e => e.Action)
                .Map(m => m.ToTable("RoleAction").MapLeftKey("ActionId").MapRightKey("RoleId"));
        }
    }
}
