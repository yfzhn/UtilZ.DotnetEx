namespace DotnetWinFormApp.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq.Expressions;

    [Table("Stu")]
    public partial class Stu : IStu
    {
        public long ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? Age { get; set; }

        public DateTime? Bir { get; set; }

        public void Test()
        {
            //Expression.Add()
        }
    }

    //[Table("STU")]
    public partial class StuOracle : IStu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Column("ID")]
        public long ID { get; set; }

        [StringLength(50)]
        //[Column("NAME")]
        public string Name { get; set; }

        //[Column("AGE")]
        public int? Age { get; set; }

        //[Column("BIR")]
        public DateTime? Bir { get; set; }
    }

    public interface IStu
    {
        long ID { get; set; }

        string Name { get; set; }

        int? Age { get; set; }

        DateTime? Bir { get; set; }
    }


}
