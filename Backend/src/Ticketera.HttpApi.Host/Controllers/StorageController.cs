using CacheManager.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Threading.Tasks;
using Ticketera.Events;
using Ticketera.Extensions;
using Ticketera.Storages;
using Volo.Abp;
using Volo.Abp.Imaging;

namespace Ticketera.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StorageController : TicketeraController
    {
        private readonly IStorageAppService _storageAppService;
        private readonly IEventAppService _eventAppService;
        private readonly IImageResizer _imageResizer;
        private readonly IMemoryCache _cache;

        public StorageController(
            IStorageAppService storageAppService, 
            IEventAppService eventAppService,
            IImageResizer imageResizer,
            IMemoryCache cache)
        {
            _storageAppService = storageAppService;
            _eventAppService = eventAppService;
            _imageResizer = imageResizer;
            _cache = cache;
        }

        [HttpGet("storage/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                var file = await _storageAppService.GetFile(fileName);

                if (file == null) return NotFound();

                byte[] filedata = await file.GetStream().GetAllBytesAsync();
                string contentType = file.FileName.GetMimeType();

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName,
                    Inline = true,
                };

                Response.Headers.Add("Content-Disposition", cd.ToString());

                return File(filedata, contentType);

            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        [HttpGet("image/{fileName}")]
        public async Task<IActionResult> GetImage(string fileName, int? width, int? height)
        {
            byte[]? filedata = null;

            try
            {
                string cacheKey = $"image_{fileName}_{width}_{height}";
                
                if (!_cache.TryGetValue(cacheKey, out filedata))
                {
                    var file = await _storageAppService.GetFile(fileName);

                    if (file == null) return NotFound();

                    filedata = await file.GetStream().GetAllBytesAsync();
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = file.FileName,
                        Inline = true,
                    };

                    Response.Headers.Add("Content-Disposition", cd.ToString());
                
                    if (width > 0 && height > 0)
                    {
                        var resizeResult = await _imageResizer.ResizeAsync(
                        filedata, 
                        new ImageResizeArgs
                        {
                            Width = width.Value,
                            Height = height.Value,
                            Mode = ImageResizeMode.Stretch
                        },
                        mimeType: "image/jpeg"
                        );

                        filedata = resizeResult.Result;
                        _cache.Set(cacheKey, filedata);
                    }
                }

                if (filedata == null)
                {
                    return NotFound();
                }

                string contentType = "image/jpeg";
                return File(filedata, contentType);
            }               
            catch (System.Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [HttpGet("ticket/{purchaseCode}/{ticketId}")]
        public async Task<IActionResult> GetTicketCode(string purchaseCode, long ticketId)
        {
            try
            {
                var file = await _eventAppService.GetTicketCode(new Tickets.TicketInputDto { PurchaseCode = purchaseCode, TicketId = ticketId});

                byte[] filedata = await file.GetStream().GetAllBytesAsync();
                string contentType = "image/png";

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName,
                    Inline = true,
                };

                Response.Headers.Add("Content-Disposition", cd.ToString());

                return File(filedata, contentType);

            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        [HttpGet("resources/{fileName}")]
        public async Task<IActionResult> GetImageCode(string fileName)
        {
            try
            {
                var file = await _storageAppService.GetImage(fileName);

                byte[] filedata = await file.GetStream().GetAllBytesAsync();
                string contentType = "image/png";

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName,
                    Inline = true,
                };

                Response.Headers.Add("Content-Disposition", cd.ToString());

                return File(filedata, contentType);

            }
            catch (System.Exception ex)
            {

                throw;
            }
        }
    }
}
