using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Koala.Persistence
{
    public abstract class BlobStorage
    {
        protected CloudStorageAccount _storageAccount;
        protected CloudBlobContainer _blobContainer;
        protected string _containerName;
        protected bool _debug = false;

        public BlobStorage()
        {
            _storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnString"].ConnectionString);
        }

        protected async Task CreateBlob(BlobContainerPublicAccessType permission = BlobContainerPublicAccessType.Blob)
        {
            if (_blobContainer == null)
            {
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
                _blobContainer = blobClient.GetContainerReference(_containerName);
                await _blobContainer.CreateIfNotExistsAsync();
                _blobContainer.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = permission
                    });
            }
        }

        public async Task DeleteBlob(string fileName)
        {
            await CreateBlob();
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);
            await blockBlob.DeleteIfExistsAsync();
        }

        public async Task<Uri> GetBlobUri(string fileName)
        {
            await CreateBlob();
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);
            if (blockBlob != null)
            {
                return blockBlob.Uri;
            }
            return null;
        }

        public async Task<bool> Exists(string fileName)
        {
            await CreateBlob();
            return await _blobContainer.GetBlockBlobReference(fileName).ExistsAsync();
        }

        public async Task<byte[]> Download(string fileName)
        {
            byte[] bytes = null;
            await CreateBlob();
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);
            if (blockBlob != null)
            {
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    await blockBlob.DownloadToStreamAsync(memoryStream);
                    bytes = memoryStream.ToArray();
                }
            }
            return bytes;
        }
    }

    public abstract class ImageStorage : BlobStorage
    {
        public async Task UploadImage(System.IO.Stream stream, string fileName)
        {
            await CreateBlob();
            CloudBlockBlob blockBlob = _blobContainer.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = "image/jpeg";
            if (blockBlob != null)
            {
                await blockBlob.UploadFromStreamAsync(stream);
            }
        }
    }

    public class PhotoStorage : ImageStorage
    {
        public PhotoStorage()
        {
            _containerName = "photos";
        }
    }
}