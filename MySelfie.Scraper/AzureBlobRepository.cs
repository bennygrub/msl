using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MySelfie.Scraper
{
    /// <summary>
    /// Stores blob data into an Azure Blob Storage account.
    /// Returnse the URI for the uploaded data.s
    /// </summary>
    public class AzureBlobRepository : IAzureBlobRepositoy
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudBlobContainer _blobContainer;

        #region Constructors
        public AzureBlobRepository()
        {
            var container = CloudConfigurationManager.GetSetting("StorageContainer");           //"queuephotos";
            var connection = CloudConfigurationManager.GetSetting("StorageConnectionString");

            this._storageAccount = CloudStorageAccount.Parse(connection);
            this._blobClient = _storageAccount.CreateCloudBlobClient();
            this._blobContainer = _blobClient.GetContainerReference(container);

            this._blobContainer.SetPermissions(this.GetPermissions()); 
        }
        public AzureBlobRepository(CloudStorageAccount storageAccount, CloudBlobClient blobClient, CloudBlobContainer blobContainer)
        {
            this._storageAccount = storageAccount;
            this._blobClient = blobClient;
            this._blobContainer = blobContainer;

            _blobContainer.SetPermissions(GetPermissions());
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Pulls down a file from a URL and saves it to Azure
        /// </summary>
        /// <param name="url">URL of the data to be saved</param>
        /// <param name="fileName">Desired file name to be saved as</param>
        /// <param name="contentType">Content Type of the URL data</param>
        /// <returns>URI to the new file in azure storage</returns>
        public Uri AddFileFromURL(string url, string fileName, string contentType)
        {
            var currentBlob = _blobContainer.GetBlockBlobReference(this._blobContainer.Uri + "/" + fileName);

            currentBlob.Properties.ContentType = contentType;

             using (var stream = GetStreamFromUrl(url)) {
                if (stream.Length > 0)
                {
                    stream.Position = 0;
                    currentBlob.UploadFromStream(stream);
                    stream.Flush();
                    stream.Close();
                }
                else
                {
                    return new Uri("http://www.susco.net/img/error.jpg");
                }
            }
            return currentBlob.Uri;
        }
            
        /// <summary>
        /// Adds an in-memory image to the azure storage
        /// NOT YET IMPLEMENTED
        /// </summary>
        /// <param name="img">In-memort image to be storeed</param>
        /// <param name="fileName">The new azure storage filenamae</param>
        /// <returns>URI to the new file in azure storage</returns>       
        public Uri AddFile(FileStream file, string fileName)
        {
            throw new NotImplementedException();

            //var currentBlob = _blobContainer.GetBlockBlobReference(this._blobContainer.Uri + "/" + fileName);

            //return currentBlob.Uri;
        }
        /// <summary>
        /// Pulls an image from a URL and saves it to Azure
        /// </summary>
        /// <param name="url">URL of the image to be saved</param>
        /// <param name="fileName">Desired file name to be saved as</param>
        /// <returns>RI to the image in azure storage</returns>
        public Uri AddImageFromURL(string url, string fileName)
        {
            var currentBlob = _blobContainer.GetBlockBlobReference(this._blobContainer.Uri + "/" + fileName);

            currentBlob.Properties.ContentType = "image/jpeg";

            using (var stream = GetStreamFromUrl(url)) {
                if (stream.Length > 0)
                {
                    stream.Position = 0;
                    currentBlob.UploadFromStream(stream);
                    stream.Flush();
                    stream.Close();
                }
                else
                {
                    return new Uri("http://www.susco.net/img/error.jpg");
                }
            }
            
            return currentBlob.Uri;
        }
        /// <summary>
        /// Adds an in-memory image to the azure storage
        /// NOT YET IMPLEMENTED
        /// </summary>
        /// <param name="img">In-memort image to be storeed</param>
        /// <param name="fileName">The new azure storage filenamae</param>
        /// <returns>URI to the image in azure storage</returns>
        public Uri AddImage(Image img, string fileName)
        {
            throw new NotImplementedException();

            //var currentBlob = _blobContainer.GetBlockBlobReference(this._blobContainer.Uri + "/" + fileName);

            //return currentBlob.Uri;
        }
        #endregion
        #region Private Methods
        private BlobContainerPermissions GetPermissions()
        {
            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;

            return permissions;
        }
        private Stream GetStreamFromUrl(string url)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    return new MemoryStream(webClient.DownloadData(url));
                }
            }
            catch (Exception ex)
            {
                Logger.Log("GetStreamFromUrl error: " + ex.ToString());

                return new MemoryStream();
            }
        }

        private static Image GetImageFromUrl(string url)
        {
            using (var webClient = new WebClient())
            {
                return ByteArrayToImage(webClient.DownloadData(url));
            }
        }

        private static Image ByteArrayToImage(byte[] fileBytes)
        {
            using (var stream = new MemoryStream(fileBytes))
            {
                return Image.FromStream(stream);
            }
        }
        #endregion
    }
}
