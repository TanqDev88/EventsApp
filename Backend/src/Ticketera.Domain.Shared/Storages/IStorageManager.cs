using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Services;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Ticketera.Storages
{
    public interface IStorageManager : IDomainService
    {
        Task<long> UploadFileAsync(IRemoteStreamContent file);
        Task<long> UploadImageAsync(IRemoteStreamContent file);
        Task<IRemoteStreamContent?> DownloadFileAsync(long fileId);
        Task<IRemoteStreamContent?> DownloadFileAsync(string fileName);
        Task<bool> RemoveFileAsync(long fileId);
    }
}
