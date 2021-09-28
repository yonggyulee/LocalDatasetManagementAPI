using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LocalDatasetManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using LocalDatasetManagementAPI.Controllers.Utils;

namespace LocalDatasetManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private IgnorePropertiesResolver IPResolver; 

        public ImagesController() 
        {
            IPResolver = new IgnorePropertiesResolver(new[] { "Metadata", "ImageFile", "Sample" });
        }

        // GET: api/Images/dataset_id
        // 전체 이미지 정보 조회
        [HttpGet("{dataset_id}")]
        public async Task<ActionResult<Object>> GetImage(string dataset_id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {                
                return await ldmdb
                            .Image
                            .Select(i => new
                            {
                                i.ImageID,
                                i.SampleID,
                                i.ImageNO,
                                i.ImageCode,
                                i.OriginalFilename,
                                i.ImageScheme,
                                Sample = new
                                {
                                    i.Sample.SampleID,
                                    i.Sample.DatasetID,
                                    i.Sample.SampleType,
                                    i.Sample.Metadata,
                                    i.Sample.ImageCount
                                }
                            })
                            .ToListAsync();
            }
        }

        // GET: api/Images/dataset_id
        // Sample_id로 이미지 정보 조회
        [HttpGet("{dataset_id}/sample={sample_id}")]
        public async Task<ActionResult<Object>> GetImage(string dataset_id, int sample_id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                return await ldmdb
                            .Image
                            .Select(i => new
                            {
                                i.ImageID,
                                i.SampleID,
                                i.ImageNO,
                                i.ImageCode,
                                i.OriginalFilename,
                                i.ImageScheme,
                                Sample = new
                                {
                                    i.Sample.SampleID,
                                    i.Sample.DatasetID,
                                    i.Sample.SampleType,
                                    i.Sample.Metadata,
                                    i.Sample.ImageCount
                                }
                            })
                            .Where(i => i.SampleID == sample_id)
                            .ToListAsync();
            }
        }

        // GET: api/Images/dataset_id/5
        // 이미지 아이디로 해당 이미지 정보 조회
        [HttpGet("{dataset_id}/{id}")]
        public async Task<Object> GetImage(string dataset_id, string id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                var image = await ldmdb
                                .Image
                                .Select(i => new
                                {
                                    i.ImageID,
                                    i.SampleID,
                                    i.ImageNO,
                                    i.ImageCode,
                                    i.OriginalFilename,
                                    i.ImageScheme,
                                    Sample = new
                                    {
                                        i.Sample.SampleID,
                                        i.Sample.DatasetID,
                                        i.Sample.SampleType,
                                        i.Sample.Metadata,
                                        i.Sample.ImageCount
                                    }
                                })
                                .Where(i => i.ImageID == id)            // WHERE 절
                                .FirstAsync();

                if (image == null)
                {
                    return NotFound();
                }

                return image;
            }
        }

        // PUT: api/Images/dataset_id/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // 이미지 정보 수정
        [HttpPut("{dataset_id}/{id}")]
        public async Task<IActionResult> PutImage(string dataset_id, string id, Image image)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                if (id != image.ImageID)
                {
                    return BadRequest();
                }

                ldmdb.Entry(image).State = EntityState.Modified;

                try
                {
                    await ldmdb.SaveChangesAsync();             // 수정한 데이터를 데이터베이스에 적용
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(ldmdb, id))                // id와 일치하는 이미지 데이터가 없을 경우
                    {
                        return NotFound();                      // NotFound
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
        }

        // POST: api/Images/dataset_id
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // 이미지 정보 입력
        [HttpPost("{dataset_id}")]
        public async Task<ActionResult<Image>> PostImage(string dataset_id, Image image)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                // 입력한 이미지 정보의 SampleID와 일치하는 Sample DbSet을 DbContext로 부터 조회.
                var sample = ldmdb.Sample.Include(b => b.Images).Where(s => s.SampleID == image.SampleID).First();
                Console.WriteLine($"POSTIMAGE FIND : {sample.SampleID}");

                Console.WriteLine($"POSTIMAGE sample add image : {image}");
                // 해당 Sample Dbset의 images 리스트에 image를 추가
                sample.Images.Add(image);
                // imageCount를 images의 개수로 갱신
                sample.ImageCount = sample.Images.Count;
                // Metadata 갱신
                sample.Metadata = GetChangedMetadata(sample);
                Console.WriteLine($"POSTIMAGE sample.Metadata : {sample.Metadata}");

                Console.WriteLine($"POSTIMAGE SAVE : {sample.SampleID}, {image.ImageID}");

                try
                {
                    await ldmdb.SaveChangesAsync();                 // 수행한 작업을 데이터베이스에 적용
                    Console.WriteLine($"POSTIMAGE SAVE COMPLETE : {sample.SampleID}, {image.ImageID}");
                }
                catch (DbUpdateException)
                {
                    if (ImageExists(ldmdb, image.ImageID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                // 입력한 결과를 GetImage 메소드 형식에 맞춰 출력
                return CreatedAtAction("GetImage", new { dataset_id = dataset_id, id = image.ImageID }, new
                {
                    image.ImageID,
                    image.SampleID,
                    image.ImageNO,
                    image.ImageCode,
                    image.OriginalFilename,
                    image.ImageScheme,
                    Sample = new
                    {
                        image.Sample.SampleID,
                        image.Sample.DatasetID,
                        image.Sample.SampleType,
                        image.Sample.Metadata,
                        image.Sample.ImageCount
                    }
                });
            }
        }

        // DELETE: api/Images/5
        // 이미지 삭제
        [HttpDelete("{dataset_id}/{id}")]
        public async Task<IActionResult> DeleteImage(string dataset_id, string id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                // id로 이미지 조회
                var image = await ldmdb.Image.FindAsync(id);
                if (image == null)          // 일치하는 이미지가 없을 경우
                {
                    return NotFound();      // NotFound
                }

                // 이미지의 SampleID에 해당하는 Sample DbSet을 조회하여 ImageCount를 감소시킴.
                var sample = ldmdb.Sample.Find(image.SampleID);
                sample.ImageCount -= 1;
                // Metadata 갱신
                sample.Metadata = GetChangedMetadata(sample);
                Console.WriteLine($"DELETEIMAGE sample.Metadata : {sample.Metadata}");

                // 데이터 베이스에서 이미지 정보 삭제
                ldmdb.Image.Remove(image);

                string current_path = Environment.CurrentDirectory + $"\\database\\{dataset_id}\\images";

                var verified_path = pathToVerifiedPath(Path.Combine(current_path, image.ImageID));

                removeFile(verified_path);

                await ldmdb.SaveChangesAsync();




                return NoContent();
            }
        }

        // POST:
        // 이미지 정보와 함께 이미지 파일 업로드
        [HttpPost("{dataset_id}/upload")]
        public async Task<Object> UploadImage(string dataset_id, [FromForm]Image image)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                // 입력한 이미지 정보의 SampleID와 일치하는 Sample DbSet을 DbContext로 부터 조회.
                var sample = ldmdb.Sample.Include(b => b.Images).Where(s => s.SampleID == image.SampleID).First();
                Console.WriteLine($"POSTIMAGE FIND : {sample.SampleID}");

                Console.WriteLine($"POSTIMAGE sample add image : {image}");
                // 해당 Sample Dbset의 images 리스트에 image를 추가
                sample.Images.Add(image);
                // imageCount를 images의 개수로 갱신
                sample.ImageCount = sample.Images.Count;
                // Metadata 갱신
                sample.Metadata = GetChangedMetadata(sample);
                Console.WriteLine($"POSTIMAGE sample.Metadata : {sample.Metadata}");

                if(image.ImageFile != null)
                {
                    Console.WriteLine($"POSTIMAGE SAVE : {sample.SampleID}, {image.ImageID}");
                    // 이미지 파일을 저장할 폴더 경로
                    string current_path = Environment.CurrentDirectory + $"\\database\\{dataset_id}\\images";

                    var verified_path = pathToVerifiedPath(Path.Combine(current_path, image.ImageID));
                    
                    try
                    {
                        // 파일 저장
                        saveFile(image.ImageFile, verified_path);
                    }
                    catch (IOException)
                    {
                        return Conflict();
                    }
                }

                try
                {
                    await ldmdb.SaveChangesAsync();
                    Console.WriteLine($"POSTIMAGE SAVE COMPLETE : {sample.SampleID}, {image.ImageID}");
                }
                catch (Exception)
                {
                    if (ImageExists(ldmdb, image.ImageID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtAction("GetImage", new { dataset_id = dataset_id, id = image.ImageID }, new
                {
                    image.ImageID,
                    image.SampleID,
                    image.ImageNO,
                    image.ImageCode,
                    image.OriginalFilename,
                    image.ImageScheme,
                    Sample = new
                    {
                        image.Sample.SampleID,
                        image.Sample.DatasetID,
                        image.Sample.SampleType,
                        image.Sample.Metadata,
                        image.Sample.ImageCount
                    }
                });
            }
        }

        // POST:
        // 이미지 파일 업로드
        [HttpPost("{dataset_id}/upload/{id}")]
        public async Task<Object> UploadImageFile(string dataset_id, string id, IFormFile imageFile)
        {
            var verified_id = idToVerifiedID(id);
            Console.WriteLine($"UPLOADIMAGEFILE : {verified_id}");
            if (imageFile == null)
            {
                Console.WriteLine("ImageFile is null");
                return NotFound("imageFile is null");
            }

            if (imageFile != null)
            {
                Console.WriteLine($"IMAGEFILE SAVE : {verified_id}");
                // 이미지 파일을 저장할 폴더 경로
                string current_path = Environment.CurrentDirectory + $"\\database\\{dataset_id}\\images";

                var verified_path = pathToVerifiedPath(Path.Combine(current_path, verified_id));
                
                try
                {
                    // 파일 저장
                    saveFile(imageFile, verified_path);
                }
                catch (IOException)
                {
                    Conflict();
                }
            }
            
            return Content("Success");
        }

        // GET:
        // 이미지 파일 다운로드
        [HttpGet("{dataset_id}/file/{id}")]
        public async Task<IActionResult> DownloadImage(string dataset_id, string id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                Console.WriteLine($"DownloadImage : {dataset_id} {id}");
                // 이미지 조회
                string verified_id = idToVerifiedID(id);
                Console.WriteLine($"DownloadImage : {dataset_id} {verified_id}");
                var image = await ldmdb.Image.FindAsync(verified_id);
                Console.WriteLine($"DownloadImage Find Success : {dataset_id} {verified_id} {image.ImageID}");


                // 이미지 파일 경로
                string current_path = Environment.CurrentDirectory + $"\\database\\{dataset_id}\\images";

                var verified_path = pathToVerifiedPath(Path.Combine(current_path, image.ImageID));

                Console.WriteLine($"DownloadImage Verified_path : {verified_path}");
                // 파일 반환
                return await GetFile(verified_path);
            }
        }
        
        private bool ImageExists(LDMContext ldmdb, string id)
        {
            return ldmdb.Image.Any(e => e.ImageID == id);
        }

        private void saveFile(IFormFile imgFile, string path)
        {
            // 경로 검증
            Console.WriteLine($"Path : {path}");
            string folder_path = path.Substring(0, path.LastIndexOf('\\'));
            Console.WriteLine($"FolderPath : {folder_path}");

            DirectoryInfo di = new DirectoryInfo(folder_path);
            if (di.Exists == false)
            {
                di.Create();
            }

            if (imgFile.Length > 0)
            {
                using (var imgStream = imgFile.OpenReadStream())
                {
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        imgStream.CopyTo(fileStream);
                    }
                }
            }
        }

        private bool removeFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (IOException)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<IActionResult> GetFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                byte[] bytes;
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    try
                    {
                        bytes = new byte[file.Length];
                        await file.ReadAsync(bytes);

                        return File(bytes, "application/octet-stream");
                    }catch(Exception)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            return NotFound();
        }

        private string pathToVerifiedPath(string path)
        {
            return path.Replace('/', '\\');
        }

        private string idToVerifiedID(string id)
        {
            return id.Replace("%2F", "/");
        }

        private string GetChangedMetadata(Sample sample)
        {
            //if (sample.Metadata != null)
            //    sample.Metadata = null;

            return JsonConvert.SerializeObject(
                            sample,
                            new JsonSerializerSettings()
                            {
                                ContractResolver = IPResolver
                            });
        }
    }
}
