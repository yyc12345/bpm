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

        public DatabaseDataContext PackageDatabaseDbContext { get; private set; }
        public DatabaseDataContext InstalledDatabaseDbContext { get; private set; }

        public void Open() {
            PackageDatabaseDbContext = new DatabaseDataContext();
            PackageDatabaseDbContext.Database.EnsureCreated();
            InstalledDatabaseDbContext = new DatabaseDataContext();
            InstalledDatabaseDbContext.Database.EnsureCreated();
        }

        public void Close() {
            PackageDatabaseDbContext.SaveChanges();
            PackageDatabaseDbContext.Dispose();
            InstalledDatabaseDbContext.SaveChanges();
            InstalledDatabaseDbContext.Dispose();
        }

    }

    public class DatabaseDataContext : DbContext {
        public DbSet<PackageDatabaseTablePackageItem> package { get; set; }
        public DbSet<PackageDatabaseTableVersionItem> version { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source = {Information.WorkPath.Enter("package.db").Path};");
        }
    }

    [Table("package")]
    public class PackageDatabaseTablePackageItem {
        [Key][Required]
        public string name { get; set; }
        public string aka { get; set; }
        [Required]
        public int type { get; set; }
        public string desc { get; set; }
    }

    [Table("version")]
    public class PackageDatabaseTableVersionItem {
        [Key][Required]
        public string name { get; set; }
        [Required]
        public string parent { get; set; }
        public string additional_desc { get; set; }
        [Required]
        public long timestamp { get; set; }
        [Required]
        public int suit_os { get; set; }
        public string dependency { get; set; }
        [Required]
        public bool reverse_conflict { get; set; }
        public string conflict { get; set; }
    }

    [Table("installed")]
    public class InstalledDatabaseTableInstalledItem {
        [Key]
        [Required]
        public string name { get; set; }
        [Required]
        public int reference_count { get; set; }
        public string reference { get; set; }
    }
}
