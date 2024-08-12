using Microsoft.AspNetCore.Authorization;
using System.IO;
using System;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Ticketera.Storages
{
    [Authorize]
    public class StorageAppService : TicketeraAppService, IStorageAppService
    { 
        private readonly IStorageManager _storageManager;
        
        public StorageAppService(IStorageManager storageManager) 
        {
            _storageManager = storageManager;
        }

        public async Task<long> PostFile(IRemoteStreamContent file) 
        {
            return await _storageManager.UploadFileAsync(file);
        }

        public async Task<long> PostImage(IRemoteStreamContent file) 
        {
            return await _storageManager.UploadImageAsync(file);
        }

        [AllowAnonymous]
        public async Task<IRemoteStreamContent?> GetFile(long fileId) 
        {
            return await _storageManager.DownloadFileAsync(fileId);
        }

        [AllowAnonymous]        
        public async Task<IRemoteStreamContent?> GetFile(string fileName)
        {
            return await _storageManager.DownloadFileAsync(fileName);
        }

        public async Task<bool> RemoveFile(long fileId)
        {
            return await _storageManager.RemoveFileAsync(fileId);
        }

        [AllowAnonymous]
        public async Task<IRemoteStreamContent?> GetImage(string fileName)
        {
            string pathEnvironment = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.Combine(pathEnvironment, "Resources", fileName);

            byte[] fileBytes = await GetFileBytesAsync(imagePath);
            using var memoryStreamQr = new MemoryStream(fileBytes);
            return new RemoteStreamContent(memoryStreamQr, "Image email");
        }

        private async Task<byte[]> GetFileBytesAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("El archivo no existe", filePath);
            }

            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
