using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N_Dexed.Deployment.Common.Domain.Storage
{
    public interface IStorageInterface<T>
    {
        string SaveFile(string fileKey, FileInfo itemFile);
        void DeleteFile(string fileKey);
        void DownloadFile(string fileKey, string destinationPath);
    }
}
