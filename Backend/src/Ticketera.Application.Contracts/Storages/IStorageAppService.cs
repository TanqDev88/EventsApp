using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Ticketera.Storages
{
    public interface IStorageAppService
    {
        Task<long> PostFile(IRemoteStreamContent file);
        Task<long> PostImage(IRemoteStreamContent file);
        Task<IRemoteStreamContent?> GetFile(long fileId);
        Task<IRemoteStreamContent?> GetFile(string fileName);
        Task<bool> RemoveFile(long fileId);
        Task<IRemoteStreamContent?> GetImage(string fileName);
    }
}
