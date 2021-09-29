using LocalDatasetManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
            using (var ldmdb = new DMContext(id))
            {
                // 데이터베이스 마이그레이션
                await ldmdb.MigrateDB();
            }

            return id;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(string id)
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
            using (var ldmdb = new DMContext(id))
            {
                // 데이터베이스 마이그레이션
                await ldmdb.MigrateDB();
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
