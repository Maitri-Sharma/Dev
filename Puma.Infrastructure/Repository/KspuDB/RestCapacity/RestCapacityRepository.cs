using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Puma.Infrastructure.Interface.KsupDB.RestCapacity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puma.Infrastructure.Repository.KspuDB.RestCapacity
{
    public class RestCapacityRepository : IRestCapacityRepository
    {
        private readonly ILogger<RestCapacityRepository> _logger;
        //private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public RestCapacityRepository(ILogger<RestCapacityRepository> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<bool> DownloadFile()
        {
            //Uri blobUri = new Uri(_configuration.GetValue<string>("BlobConnectionString"));

            //CloudBlockBlob blockBlob = null;

            //blockBlob = new CloudBlockBlob(blobUri);

            //if (blockBlob != null)
            //{
            //    using (var file = new FileStream(txtFileName.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //    {
            //        blockBlob.UploadFromStream(file);
            //    }
            //    MessageBox.Show("File uploaded successfully");
            //}
            try
            {
                _logger.LogInformation("Entered in Download File");
                string fileName;
                bool fileExists = true;
                BlobContinuationToken blobContinuationToken = null;
                CloudBlockBlob blockBlob;
                await using (MemoryStream memoryStream = new MemoryStream())
                {
                    string blobstorageconnection = _configuration.GetSection("BlobConnectionString").Value;

                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);

                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_configuration.GetSection("BlobContainerName").Value);

                    var resultBlob = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    var blobs = resultBlob.Results.OfType<CloudBlockBlob>();
                    blobContinuationToken = resultBlob.ContinuationToken;

                    // var blob1s = ;

                    if (blobs != null && blobs?.Any() == true)
                    {
                        _logger.LogInformation("blobs is not null ");

                        foreach (var item in blobs)
                        {
                            _logger.LogInformation("Blobs Present");
                            fileName = item.Name;
                            //Console.WriteLine(item.Uri);
                            string directoryPath = _configuration.GetSection("FileLogging.ObjectLogPathZipped").Value;
                            string filePath = directoryPath + fileName;
                            _logger.LogInformation("File Path: " + filePath);
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }
                            if (!(System.IO.File.Exists(filePath)))
                            {
                                blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                                await blockBlob.DownloadToStreamAsync(memoryStream);

                                _logger.LogInformation("Blob Name: " + fileName);
                                //Stream blobStream = blockBlob.DownloadToStreamAsync("E:\Tools\BlobFile\" + fileName).Result;
                                var pageBlob = cloudBlobContainer.GetPageBlobReference(fileName);

                                _logger.LogInformation("Directory exist");
                                await blockBlob.DownloadToFileAsync(filePath, FileMode.OpenOrCreate);
                                _logger.LogInformation("File Downloaded: " + fileName);
                                fileExists = false;
                            }
                            else
                            {
                                _logger.LogInformation("File Already Exist: " + fileName);
                                fileExists = true;
                            }
                            //return File(blobStream, blockBlob.Properties.ContentType, blockBlob.Name);
                        } while (blobContinuationToken != null)
                            fileExists = true;
                        _logger.LogInformation("File not present");
                        await DeleteFile();
                    }
                    else
                    {
                        _logger.LogInformation("No File Found");
                    }
                    _logger.LogInformation("Exited from Download File");
                    return fileExists;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Download File: " + ex.Message);
                return false;
            }

        }


        public async Task DeleteFile()
        {
            try
            {
                _logger.LogInformation("Entered in Delete File");
                string fileName;
                BlobContinuationToken blobContinuationToken = null;
                CloudBlockBlob blockBlob;
                string blobstorageconnection = _configuration.GetSection("BlobConnectionString").Value;
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = _configuration.GetSection("BlobContainerName").Value;
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                //var blob = cloudBlobContainer.GetBlobReference(fileName);

                int daysToDelete = Convert.ToInt16(_configuration.GetSection("DaysToDeleteFiles").Value);

                var resultBlob = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                var blobs = resultBlob.Results.OfType<CloudBlockBlob>()
            .Where(b => b.Properties.LastModified != null && b.Properties.LastModified.Value.Date <= DateTime.Now.Date);
                blobContinuationToken = resultBlob.ContinuationToken;

                string directoryPath = _configuration.GetSection("FileLogging.ObjectLogPathZipped").Value;
                //string filePath = directoryPath;
                string[] files = Directory.GetFiles(directoryPath);
                _logger.LogInformation("Delete File path: " + directoryPath);
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < DateTime.Now.AddDays(-daysToDelete))
                    {
                        fileInfo.Delete();
                        _logger.LogInformation("File deleted from location");
                    }
                }

                string directoryPath_unzipped = _configuration.GetSection("FileLogging.ObjectLogPathUnZipped").Value;
                //string filePath = directoryPath;
                string[] files_unzipped = Directory.GetFiles(directoryPath_unzipped);
                _logger.LogInformation("Delete File path: " + directoryPath);
                foreach (string file in files_unzipped)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < DateTime.Now.AddDays(-daysToDelete))
                    {
                        fileInfo.Delete();
                        _logger.LogInformation("File deleted from location");
                    }
                }

                if (blobs != null)
                {
                    foreach (var item in blobs)
                    {
                        fileName = item.Name;
                        _logger.LogInformation("Delete Blob:" + fileName);
                        //Console.WriteLine(item.Uri);
                        blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                        //Stream blobStream = blockBlob.DownloadToStreamAsync("E:\Tools\BlobFile\" + fileName).Result;
                        var pageBlob = cloudBlobContainer.GetPageBlobReference(fileName);
                        await blockBlob.DeleteIfExistsAsync();
                        _logger.LogInformation("File Deleted from Blob");
                        //return File(blobStream, blockBlob.Properties.ContentType, blockBlob.Name);
                    } // while (blobContinuationToken != null)
                }


                _logger.LogInformation("Exited from Delete File");
               
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Delete File: " + ex.Message);
            }


        }
    }
}
