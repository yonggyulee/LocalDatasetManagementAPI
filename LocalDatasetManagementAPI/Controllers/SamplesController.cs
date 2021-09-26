using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LocalDatasetManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using LocalDatasetManagementAPI.Controllers.Utils;

namespace LocalDatasetManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplesController : ControllerBase
    {
        private IgnorePropertiesResolver IPResolver;

        public SamplesController()
        {
            IPResolver = new IgnorePropertiesResolver(new[] { "Metadata", "Sample", "ImageFile"});
        }

        // GET: api/Samples/dataset_id
        [HttpGet("{dataset_id}")]
        public async Task<ActionResult<Object>> GetSample(string dataset_id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                return await ldmdb
                            .Sample
                            .Select(s => new
                            {
                                s.SampleID,
                                s.DatasetID,
                                s.SampleType,
                                s.Metadata,
                                s.ImageCount,
                                Images = s.Images.Select(i =>
                                                    new
                                                    {
                                                        i.ImageID,
                                                        i.SampleID,
                                                        i.ImageNO,
                                                        i.ImageCode,
                                                        i.OriginalFilename,
                                                        i.ImageScheme
                                                    }).ToList()
                            }).ToListAsync();
                
            }
        }

        // GET: api/Samples/dataset_id/5
        [HttpGet("{dataset_id}/{id}")]
        public async Task<ActionResult<Sample>> GetSample(string dataset_id, int id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                var sample = await ldmdb.Sample.FindAsync(id);

                if (sample == null)
                {
                    return NotFound();
                }

                return sample;
            }
        }

        // PUT: api/Samples/dataset_id/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{dataset_id}/{id}")]
        public async Task<IActionResult> PutSample(string dataset_id, int id, Sample sample)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                if (id != sample.SampleID)
                {
                    return BadRequest();
                }

                ldmdb.Entry(sample).State = EntityState.Modified;

                try
                {
                    await ldmdb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SampleExists(ldmdb, id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
        }

        // POST: api/Samples/dataset_id
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{dataset_id}")]
        public async Task<ActionResult<Sample>> PostSample(string dataset_id, Sample sample)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                sample.DatasetID = dataset_id;
                string body = JsonConvert.SerializeObject(
                                        sample,
                                        new JsonSerializerSettings() { 
                                            ContractResolver = IPResolver
                                        });
                
                Console.WriteLine($"SAMPLE POST {dataset_id} : {body}");
                sample.Metadata = body;
                Console.WriteLine($"SAMPLE POST ADD : {dataset_id} - {sample.SampleID}");
                ldmdb.Sample.Add(sample);
                Console.WriteLine($"SAMPLE POST SAVE : {dataset_id} - {sample.SampleID}");
                await ldmdb.SaveChangesAsync();

                return CreatedAtAction("GetSample", new { dataset_id = dataset_id, id = sample.SampleID }, new
                {
                    sample.SampleID,
                    sample.DatasetID,
                    sample.SampleType,
                    sample.Metadata,
                    sample.ImageCount,
                    Images = sample.Images.Select(i =>
                                                    new
                                                    {
                                                        i.ImageID,
                                                        i.SampleID,
                                                        i.ImageNO,
                                                        i.ImageCode,
                                                        i.OriginalFilename,
                                                        i.ImageScheme
                                                    }).ToList()
                });
            }
        }

        // DELETE: api/Samples/dataset_id/5
        [HttpDelete("{dataset_id}/{id}")]
        public async Task<IActionResult> DeleteSample(string dataset_id, int id)
        {
            using (var ldmdb = new LDMContext(dataset_id))
            {
                var sample = await ldmdb.Sample.FindAsync(id);
                if (sample == null)
                {
                    return NotFound();
                }

                ldmdb.Sample.Remove(sample);
                await ldmdb.SaveChangesAsync();

                return NoContent();
            }
        }

        private bool SampleExists(LDMContext ldmdb, int id)
        {
            return ldmdb.Sample.Any(e => e.SampleID == id);
        }
    }
}
