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

        /// <summary>
        /// open database
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source = {Information.WorkPath.Enter("package.db").Path};");
        }

        /// <summary>
        /// set default value
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //package table
            modelBuilder.Entity<PackageDatabaseTablePackageItem>()
                .Property(b => b.aka)
                .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTablePackageItem>()
                .Property(b => b.desc)
                .HasDefaultValue(string.Empty);

            //version table
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.additional_desc)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.additional_desc)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.dependency)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.conflict)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.internal_script)
               .HasDefaultValue(string.Empty);
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
        [Required]
        public bool require_decompress { get; set; }
        public string internal_script { get; set; }
        [Required]
        public string hash { get; set; }
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

        /// <summary>
        /// open database
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source = {Information.WorkPath.Enter("installed.db").Path};");
        }

        /// <summary>
        /// set default value
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //package table
            modelBuilder.Entity<InstalledDatabaseTableInstalledItem>()
                .Property(b => b.reference)
                .HasDefaultValue(string.Empty);
            modelBuilder.Entity<InstalledDatabaseTableInstalledItem>()
                .Property(b => b.data)
                .HasDefaultValue(string.Empty);
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
        public string data { get; set; }
    }

    #endregion

}
