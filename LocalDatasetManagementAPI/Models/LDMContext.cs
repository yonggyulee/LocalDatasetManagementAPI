using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace LocalDatasetManagementAPI.Models
{
    public class LDMContext : DbContext
    {
        private string DbFileName = "ldm.db";
        public string DbPath { get; private set; }
        public LDMContext(string ds_id = "dataset_1")
        {
            DbPath = GetDbPath(ds_id);
            DirectoryInfo dataset = new DirectoryInfo(DbPath);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        public DbSet<LocalDatasetManagementAPI.Models.Sample> Sample { get; set; }

        public DbSet<LocalDatasetManagementAPI.Models.Image> Image { get; set; }

        // 데이터베이스 위치 경로 반환.
        // 반환값 : ../솔루션 폴더/프로젝트 폴더/database
        private string GetDbPath(string dataset_id)
        {
            // 현재 프로젝트 경로
            // CurrentDirectory : ../솔루션 폴더/프로젝트 폴더
            // 데이터베이스 위치 경로
            DirectoryInfo database_di = new DirectoryInfo(Environment.CurrentDirectory + "\\database");

            // database 폴더가 없을 시 database 폴더 생성
            if (!database_di.Exists)
            {
                database_di.Create();
            }

            // database_path : ../솔루션 폴더/프로젝트 폴더/database/{dataset_id}/ldm.db
            string database_path = database_di.ToString() + "\\" + dataset_id + "\\" + DbFileName;
            Console.WriteLine($"LDMContext.GetDbPath Return : {database_path}");

            return database_path;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sample>().HasMany(s => s.Images).WithOne(i => i.Sample);
        }

    }
}
