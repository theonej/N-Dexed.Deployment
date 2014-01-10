using N_Dexed.Deployment.Common.Domain.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.IO;
using Amazon;
using N_Dexed.Deployment.Common.Domain;

namespace N_Dexed.Deployment.AWS.Storage
{
    public class S3CommandLibraryStorageInterface : IStorageInterface<CommandLibraryInfo>
    {
        const string LINK_BASE_PATH = "https://s3.amazonaws.com/n-dexed.deployment.commandLibraries";
        const string COMMAND_LIBRARY_BUCKET_NAME = "n-dexed.deployment.commandLibraries";

        public S3CommandLibraryStorageInterface()
        {

        }

        public string SaveFile(string fileKey, FileInfo itemFile)
        {
            IAmazonS3 context = InitializeContext();
            using (context)
            {
                S3FileInfo file = new S3FileInfo(context, COMMAND_LIBRARY_BUCKET_NAME, fileKey);
                Stream writeStream = file.OpenWrite();
                using (writeStream)
                {
                        FileStream readStream = itemFile.OpenRead();
                        using(readStream)
                        {
                            readStream.CopyTo(writeStream);
                        }
                }

                string returnValue = string.Format("{0}/{1}", LINK_BASE_PATH, fileKey);

                return returnValue;
            }
        }

        public void DeleteFile(string fileKey)
        {
             IAmazonS3 context = InitializeContext();
             using (context)
             {
                 S3FileInfo file = new S3FileInfo(context, COMMAND_LIBRARY_BUCKET_NAME, fileKey);
                 file.Delete();
             }
        }

        public void DownloadFile(string fileKey, string destinationPath)
        {
            IAmazonS3 context = InitializeContext();
            using (context)
            {
                FileStream writeStream = File.Create(destinationPath);
                using(writeStream)
                {
                    S3FileInfo file = new S3FileInfo(context, COMMAND_LIBRARY_BUCKET_NAME, fileKey);
                    Stream readStream = file.OpenRead();
                    using (readStream)
                    {
                        readStream.CopyTo(writeStream);
                    }
                }
            }
        }

        #region Private Methods

        private IAmazonS3 InitializeContext()
        {
            AmazonS3Config config = new AmazonS3Config();
            config.RegionEndpoint = RegionEndpoint.USEast1;
            
            AmazonS3Client context = new AmazonS3Client(config);

            return context;
        }

        #endregion
    }
}
