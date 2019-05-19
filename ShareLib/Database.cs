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

    #region package database

    public class PackageDatabase {

        public PackageDatabaseDataContext CoreDbContext { get; private set; }

        public void Open() {
            CoreDbContext = new PackageDatabaseDataContext();
            CoreDbContext.Database.EnsureCreated();
        }

        public void Close() {
            CoreDbContext.SaveChanges();
            CoreDbContext.Dispose();
        }

    }

    public class PackageDatabaseDataContext : DbContext {
        public DbSet<PackageDatabaseTablePackageItem> package { get; set; }
        public DbSet<PackageDatabaseTableVersionItem> version { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source = {Information.WorkPath.Enter("package.db").Path};");
        }
    }

    [Table("package")]
    public class PackageDatabaseTablePackageItem {
        [Key]
        [Required]
        public string name { get; set; }
        public string aka { get; set; }
        [Required]
        public int type { get; set; }
        public string desc { get; set; }
    }

    [Table("version")]
    public class PackageDatabaseTableVersionItem {
        [Key]
        [Required]
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

    #endregion

    #region installed database

    public class InstalledDatabase {

        public InstalledDatabaseDataContext CoreDbContext { get; private set; }

        public void Open() {
            CoreDbContext = new InstalledDatabaseDataContext();
            CoreDbContext.Database.EnsureCreated();
        }

        public void Close() {
            CoreDbContext.SaveChanges();
            CoreDbContext.Dispose();
        }

    }

    public class InstalledDatabaseDataContext : DbContext {
        public DbSet<InstalledDatabaseTableInstalledItem> installed { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source = {Information.WorkPath.Enter("installed.db").Path};");
        }
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

    #endregion

}
