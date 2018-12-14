using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
//using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShareLib {

    public class Database {

        public DatabaseDataContext CoreDbContext { get; private set; }

        public void Open() {
            CoreDbContext = new DatabaseDataContext();
            CoreDbContext.Database.EnsureCreated();
        }

        public void Close() {
            CoreDbContext.SaveChanges();
            CoreDbContext.Dispose();
        }

    }

    public class DatabaseDataContext : DbContext {
        public DbSet<DatabaseItem> package { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source = {Information.WorkPath.Enter("server.db").Path};");
        }
    }

    [Table("package")]
    public class DatabaseItem {
        [Key][Required]
        public string name { get; set; }

        public string aka { get; set; }
        [Required]
        public int type { get; set; }
        [Required]
        public string version { get; set; }
        public string desc { get; set; }
    }

}
