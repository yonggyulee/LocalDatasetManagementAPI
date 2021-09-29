using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocalDatasetManagementAPI.Models
{
    public class DMContext : DbContext
    {
        private string DbFileName = "ldm.db";
        public string DbPath { get; private set; }
        public DMContext(string ds_id = "dataset_1")
        {
            DbPath = GetDbPath(ds_id);
            DirectoryInfo dataset = new DirectoryInfo(DbPath);
        }

        public async Task MigrateDB()
        {
            // 기존에 Migration한 데이터베이스 정보를 db 파일에 적용.
            // id 위치에 db 파일이 없을 시 migrate하여 db 파일 생성.
            await this.GetInfrastructure().GetService<IMigrator>().MigrateAsync("Database_v6");
            // 가장 최근에 적용된 Migration을 반환.
            var lastAppliedMigration = (await this.Database.GetAppliedMigrationsAsync()).Last();
            Console.WriteLine($"You're on schema version: {lastAppliedMigration}");
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
