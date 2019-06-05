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
                .Property(b => b.name)
                .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTablePackageItem>()
                .Property(b => b.aka)
                .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTablePackageItem>()
                .Property(b => b.type)
                .HasDefaultValue((int)PackageType.Miscellaneous);
            modelBuilder.Entity<PackageDatabaseTablePackageItem>()
                .Property(b => b.desc)
                .HasDefaultValue(string.Empty);

            //version table
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.name)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.parent)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.additional_desc)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.timestamp)
               .HasDefaultValue(0);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.suit_os)
               .HasDefaultValue((int)OSType.None);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.dependency)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.reverse_conflict)
               .HasDefaultValue(false);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.conflict)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.require_decompress)
               .HasDefaultValue(string.Empty);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.internal_script)
               .HasDefaultValue(false);
            modelBuilder.Entity<PackageDatabaseTableVersionItem>()
               .Property(b => b.hash)
               .HasDefaultValue(string.Empty);
        }
    }

    [Table("package")]
    public class PackageDatabaseTablePackageItem {
        public PackageDatabaseTablePackageItem() {
            name = "";
            aka = "";
            type = (int)PackageType.Miscellaneous;
            desc = "";
        }

        [Key]
        [Required]
        public string name { get; set; }
        [Required]
        public string aka { get; set; }
        [Required]
        public int type { get; set; }
        [Required]
        public string desc { get; set; }
    }

    [Table("version")]
    public class PackageDatabaseTableVersionItem {
        public PackageDatabaseTableVersionItem() {
            name = "";
            parent = "";
            additional_desc = "";
            timestamp = 0;
            suit_os = (int)OSType.None;
            dependency = "";
            reverse_conflict = false;
            conflict = "";
            require_decompress = "";
            internal_script = false;
            hash = "";
        }

        [Key]
        [Required]
        public string name { get; set; }
        [Required]
        public string parent { get; set; }
        [Required]
        public string additional_desc { get; set; }
        [Required]
        public long timestamp { get; set; }
        [Required]
        public int suit_os { get; set; }
        [Required]
        public string dependency { get; set; }
        [Required]
        public bool reverse_conflict { get; set; }
        [Required]
        public string conflict { get; set; }
        [Required]
        public string require_decompress { get; set; }
        [Required]
        public bool internal_script { get; set; }
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
                .Property(b => b.name)
                .HasDefaultValue(string.Empty);
            modelBuilder.Entity<InstalledDatabaseTableInstalledItem>()
                .Property(b => b.reference_count)
                .HasDefaultValue(0);
            modelBuilder.Entity<InstalledDatabaseTableInstalledItem>()
                .Property(b => b.reference)
                .HasDefaultValue(string.Empty);
            //todo: check a empty Dictionary's correct JSON string
            modelBuilder.Entity<InstalledDatabaseTableInstalledItem>()
                .Property(b => b.data)
                .HasDefaultValue("{}");
        }
    }

    [Table("installed")]
    public class InstalledDatabaseTableInstalledItem {
        public InstalledDatabaseTableInstalledItem() {
            name = "";
            reference = "";
            reference_count = 0;
            data = "";
        }

        [Key]
        [Required]
        public string name { get; set; }
        [Required]
        public int reference_count { get; set; }
        [Required]
        public string reference { get; set; }
        [Required]
        public string data { get; set; }
    }

    #endregion

}
