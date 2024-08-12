using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Extensions;
using Ticketera.Localization;
using Ticketera.Providers;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Ticketera.Storages
{
    public class StorageManager : DomainService, IStorageManager
    {
        private readonly IRepository<FileAttachment, long> _attachmentRepository;
        private readonly IBlobContainer _blobContainer;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IStringLocalizer<TicketeraResource> _localizer;
        private readonly IConfig _config;
        private readonly HttpClient _httpClient;

        public StorageManager(IRepository<FileAttachment, long> attachmentRepository, IBlobContainer blobContainer, IUnitOfWorkManager unitOfWorkManager, IStringLocalizer<TicketeraResource> localizer, IConfig config)
        {
            _attachmentRepository = attachmentRepository;
            _blobContainer = blobContainer;
            _unitOfWorkManager = unitOfWorkManager;
            _localizer = localizer;
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<long> UploadFileAsync(IRemoteStreamContent file)
        {
            if (file == null) throw new UserFriendlyException(_localizer["ErrorFile"]);

            var fs = file.GetStream();
            var fileNameGuid = $"{Guid.NewGuid()}{file.FileName.GetExtension()}";

            await _blobContainer.SaveAsync(fileNameGuid, fs);

            var fileattachment = new FileAttachment()
            {
                FileName = file.FileName ?? "",
                Path = $"{_config.CurrentDomain}/storage/{file.FileName}",
                Size = file.ContentLength ?? 0,
                FileNameGuid = fileNameGuid
            };

            fileattachment = await _attachmentRepository.InsertAsync(fileattachment, true);

            return fileattachment.Id;
        }

        public async Task<long> UploadImageAsync(IRemoteStreamContent image)
        {
            if (image == null) throw new UserFriendlyException(_localizer["ErrorFile"]);

            const long maxSizeInBytes = 20 * 1024 * 1024;

            if (image.ContentLength > maxSizeInBytes) throw new UserFriendlyException(_localizer["ErrorFileSizeExceeded"]);
            
            var fs = image.GetStream();
            var imageNameGuid = $"{Guid.NewGuid()}{image.FileName?.GetExtension()}";

            await _blobContainer.SaveAsync(imageNameGuid, fs);

            var fileattachment = new FileAttachment()
            {
                FileName = image.FileName ?? "",
                Path = $"{_config.CurrentDomain}/image/{image.FileName}",
                Size = image.ContentLength ?? 0,
                FileNameGuid = imageNameGuid
            };

            fileattachment = await _attachmentRepository.InsertAsync(fileattachment, true);

            return fileattachment.Id;
        }

        public async Task<IRemoteStreamContent?> DownloadFileAsync(long fileId)
        {
            var file = await _attachmentRepository.FirstOrDefaultAsync(x => x.Id == fileId);
            if (file == null) return null;

            var isFileLocal = file.Path.StartsWith(_config.CurrentDomain);

            var bytes = isFileLocal ?
                await _blobContainer.GetAllBytesAsync(file.FileNameGuid) :
                await GetBytesFromUrl(file.Path);

            using var memoryStream = new MemoryStream(bytes);
            return new RemoteStreamContent(memoryStream, file.FileName);

        }

        public async Task<IRemoteStreamContent?> DownloadFileAsync(string fileName)
        {
            var file = await _attachmentRepository.FirstOrDefaultAsync(x => x.FileName == fileName);
            if (file == null) return null;

            var isFileLocal = file.Path.StartsWith(_config.CurrentDomain);

            var bytes = isFileLocal ?
                await _blobContainer.GetAllBytesAsync(file.FileNameGuid) :
                await GetBytesFromUrl(file.Path);

            using var memoryStream = new MemoryStream(bytes);
            return new RemoteStreamContent(memoryStream, file.FileName);
        }

        private async Task<byte[]> GetBytesFromUrl(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return bytes;
                }
                return null;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RemoveFileAsync(long fileId)
        {
            // 1. Obtener la entrada del archivo desde la base de datos utilizando fileId
            var fileAttachment = await _attachmentRepository.FirstOrDefaultAsync(x => x.Id == fileId);

            // 2. Verificar si el archivo existe
            if (fileAttachment == null)
            {
                // El archivo no existe
                return false;
            }

            // 3. Eliminar el blob correspondiente en el almacenamiento de blobs
            if (fileAttachment.Path.StartsWith(_config.CurrentDomain))
                await _blobContainer.DeleteAsync(fileAttachment.FileNameGuid);

            // 4. Eliminar la entrada del archivo de la base de datos
            await _attachmentRepository.DeleteAsync(fileAttachment, true);

            // La eliminación fue exitosa
            return true;
        }
    }
}
