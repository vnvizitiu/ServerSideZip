using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ServerSideZip.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerSideZip : ControllerBase
    {
        private readonly FolderLocation _folderLocation;
        
        public ServerSideZip(IOptions<FolderLocation> folderLocationOptions)
        {
            _folderLocation = folderLocationOptions.Value;
        }
        
        [HttpGet("getArchive")]
        public async Task<IActionResult> GetArchive()
        {
            try
            {
                (string fileName, byte[] archiveBytes) = await GetArchiveAsync();

                return File(archiveBytes, "application/zip", fileName);
            }
            catch (Exception e)
            {
                return BadRequest(e);
                // log your error and respond in kind to the user to let them know what happened.
            }
        }

        private async Task<(string archiveName, byte[] archiveBytes)> GetArchiveAsync()
        {
            Dictionary<string, byte[]> filesToArchive = new();

            foreach (string file in Directory.GetFiles(_folderLocation.Path))
            {
                filesToArchive[Path.GetFileName(file)] = await System.IO.File.ReadAllBytesAsync(file);
            }

            using (MemoryStream archiveStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(archiveStream, ZipArchiveMode.Create, leaveOpen: false))
                {
                    foreach (var (fileName, fileBytes) in filesToArchive)
                    {
                        ZipArchiveEntry zipEntry = zipArchive.CreateEntry(fileName);

                        using (MemoryStream fileStream = new MemoryStream(fileBytes))
                        using (Stream zipEntryStream = zipEntry.Open())
                        {
                            await fileStream.CopyToAsync(zipEntryStream);
                        }
                    }
                }
                return ("testArchive.zip", archiveStream.ToArray());
            }
        }
    }
}