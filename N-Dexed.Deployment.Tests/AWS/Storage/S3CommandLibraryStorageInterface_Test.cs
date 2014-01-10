using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using N_Dexed.Deployment.AWS.Storage;
using System.Net;

namespace N_Dexed.Deployment.Tests.AWS.Storage
{
    [TestClass]
    public class S3CommandLibraryStorageInterface_Test
    {
        [TestMethod]
        public void SaveFileThenDownloadThenDeleteFile()
        {
            string filePath = "testFile.txt";
            StreamWriter writer = File.CreateText(filePath);
            using(writer)
            {
                writer.WriteLine("This is a test file");
            }

            Guid fileGuid = Guid.NewGuid();
            string fileKey = string.Format("{0}/{1}", fileGuid, filePath);

            S3CommandLibraryStorageInterface storage = new S3CommandLibraryStorageInterface();

            FileInfo testFile = new FileInfo(filePath);
            string fileLink = storage.SaveFile(fileKey, testFile);

            string downloadPath = "testFile.download.txt";

            storage.DownloadFile(fileKey, downloadPath);

            Assert.IsTrue(File.Exists(downloadPath));
            File.Delete(filePath);
            File.Delete(downloadPath);
            storage.DeleteFile(fileKey);
        }
    }
}
