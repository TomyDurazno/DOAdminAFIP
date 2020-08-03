﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DOAdminAFIP.Core;
using DOAdminAFIP.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DOAdminAFIP.Controllers
{
    [ApiController]
    public class FilesController : ControllerBase
    {
        string Nombre_Excel(string s) => $"Ventas Generadas_{s}.xls";

        #region Services

        IMemoryCache cache;
        IFormFileParser afipParser;

        #endregion

        #region Constructor

        public FilesController(IMemoryCache _cache, IFormFileParser _afipParser)
        {
            cache = _cache;
            afipParser = _afipParser;
        }

        #endregion
      
        #region Endpoints

        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var content = await file.ReadAsStringAsync();

            var generated = afipParser.GenerateWorkBook(content, Nombre_Excel(file.FileName));
            
            cache.Set(generated.Nombre, generated);

            return Ok(new { key = generated.Nombre });
        }

        [HttpGet]
        [Route("download")]
        public IActionResult Download(string key)
        {
            if (!cache.TryGetValue<WorkBookGenerationResult>(key, out var result))
                throw new FileNotFoundException("Element not in cache");

            using (var memory = new MemoryStream())
            {
                result.Workbook.Write(memory);
                return File(memory.ToArray(), "application/vnd.ms-excel", key);
            }
        }

        #endregion
    }
}