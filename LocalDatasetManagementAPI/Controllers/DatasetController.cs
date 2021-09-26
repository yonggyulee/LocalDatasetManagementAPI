using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LocalDatasetManagementAPI.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace LocalDatasetManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatasetController : ControllerBase
    {
        private string DbDirectoryPath { get; set; }

        public DatasetController() 
        {
            SetDbDirectoryPath();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetDataset()
        {
            return GetDatasetList();
        }

        [HttpGet("create_dataset={id}")]
        public async Task<ActionResult<string>> CreateDataset(string id)
        {
            var dl = GetDatasetList();
            if (dl.Contains(id))
            {
                return NoContent();
            }

            // 데이터셋 디렉토리 생성
            CreateDirectory(id);

            // 이미지 디렉토리 생성
            CreateDirectory($"{id}\\images");

            // db 생성
            using (var ldmdb = new LDMContext(id))
            {
                //var pendingMigrations = await ldmdb.Database.GetPendingMigrationsAsync();

                //Console.WriteLine($"pendingMigrations :::: {pendingMigrations.Any()}");
                //if (pendingMigrations.Any())
                //{
                //    Console.WriteLine($"You have {pendingMigrations.Count()} pending migrations to apply.");
                //    Console.WriteLine("Applying pending migrations now");
                //    await ldmdb.Database.MigrateAsync();
                //}
                
                // 기존에 Migration한 데이터베이스 정보를 db 파일에 적용.
                // id 위치에 db 파일이 없을 시 migrate하여 db 파일 생성. 
                await ldmdb.GetInfrastructure().GetService<IMigrator>().MigrateAsync("Database_v6");
                // 가장 최근에 적용된 Migration을 반환.
                var lastAppliedMigration = (await ldmdb.Database.GetAppliedMigrationsAsync()).Last();

                Console.WriteLine($"You're on schema version: {lastAppliedMigration}");
            }

            return id;
        }

        private List<string> GetDatasetList()
        {
            DirectoryInfo di = new DirectoryInfo(DbDirectoryPath);

            var directories = di.GetDirectories();

            List<string> d_list = new List<string>();
            foreach (var item in directories)
            {
                d_list.Add(item.Name);
            }

            return d_list;
        }
        
        private void CreateDirectory(string id)
        {
            DirectoryInfo di = new DirectoryInfo($"{DbDirectoryPath}\\{id}");
            di.Create();
        }

        // database 경로 설정
        private void SetDbDirectoryPath()
        {
            // 현재 프로젝트 경로
            // CurrentDirectory : ../솔루션 폴더/프로젝트 폴더/
            // 데이터베이스 위치 경로
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory + "\\database");
            Console.WriteLine($"DbDirectoryPath:{di.ToString()}");

            // database 폴더가 없을 시 생성
            if (!di.Exists)
            {
                di.Create();
            }

            DbDirectoryPath = di.ToString();
        }
    }
}
