using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace ImageUpload
{
    public static class funImageUpload
    {
        [FunctionName("UploadImage")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")]HttpRequestMessage req, TraceWriter log)
        {
            string name = null;
            log.Info("C# HTTP trigger function processed a request.");
            try
            {

                if (req.Content.IsMimeMultipartContent())
                {
                    var provider = new MultipartMemoryStreamProvider();
                    req.Content.ReadAsMultipartAsync(provider).Wait();
                    foreach (HttpContent ctnt in provider.Contents)
                    {
                        //now read individual part into STREAM
                        var stream = ctnt.ReadAsStreamAsync();

                        string storageConnectionString = System.Environment.GetEnvironmentVariable("AzureWebJobsStorage");

                        log.Info("storageConnectionString");
                        CloudStorageAccount storageAccount;
                        if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                        {
                            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("learningazurestor");
                            await cloudBlobContainer.CreateIfNotExistsAsync();
                            await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                            log.Info("blobl");
                            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(ctnt.Headers.ContentDisposition.FileName);
                            cloudBlockBlob.Properties.ContentType = ctnt.Headers.ContentType.ToString();

                            await cloudBlockBlob.UploadFromStreamAsync(stream.Result);
                        }

                        return req.CreateResponse(HttpStatusCode.OK, "Uploaded Image");



                    }
                }
                else
                {
                    HttpContent ctnt = req.Content;

                    var stream = await ctnt.ReadAsStreamAsync();

                    string storageConnectionString = System.Environment.GetEnvironmentVariable("AzureWebJobsStorage");

                    log.Info("storageConnectionString");
                    CloudStorageAccount storageAccount;
                    if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                    {
                        // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                        CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("learningazurestor");
                        await cloudBlobContainer.CreateIfNotExistsAsync();
                        await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                        log.Info("blobl");
                        CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference("myfiletest.jpg");
                        cloudBlockBlob.Properties.ContentType = ctnt.Headers.ContentType.ToString();

                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }

                }
               
                log.Info("done");
            }
            catch (Exception ex)
            { log.Info(ex.Message); log.Info(ex.StackTrace); }
            return name == null
            ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a Path  in the request body")
            : req.CreateResponse(HttpStatusCode.OK, "Uploaded to Storage Blob");
        }
    }

    //[FunctionName("AddPhoto")]
    //public static async  void RunAsync( [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]  HttpRequestMessage req)
    //{
    //    dynamic data = await req.Content.ReadAsAsync<object>();
    //    string photoBase64String = data.photoBase64;
    //    //Uri uri = await UploadBlobAsync(photoBase64String);
    //    //return req.CreateResponse(HttpStatusCode.Accepted, uri);
    //}

    ////public static async Task<Uri> UploadBlobAsync(string photoBase64String)
    ////{
    ////    var match = new Regex(
    ////      $@"^data\:(?<type>image\/(jpg|gif|png));base64,(?<data>[A-Z0-9\+\/\=]+)$",
    ////      RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase)
    ////      .Match(photoBase64String);
    ////    string contentType = match.Groups["type"].Value;
    ////    string extension = contentType.Split('/')[1];
    ////    string fileName = $"{Guid.NewGuid().ToString()}.{extension}";
    ////    byte[] photoBytes = Convert.FromBase64String(match.Groups["data"].Value);

    ////    //CloudStorageAccount storageAccount = CloudStorageAccount
    ////    //  .Parse(ConfigurationManager.AppSettings["BlobConnectionString"]);
    ////    //CloudBlobClient client = storageAccount.CreateCloudBlobClient();
    ////    //CloudBlobContainer container = client.GetContainerReference("img");

    ////    //await container.CreateIfNotExistsAsync(
    ////    //  BlobContainerPublicAccessType.Blob,
    ////    //  new BlobRequestOptions(),
    ////    //  new OperationContext());
    ////    //CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
    ////    //blob.Properties.ContentType = contentType;

    ////    //using (Stream stream = new MemoryStream(photoBytes, 0, photoBytes.Length))
    ////    //{
    ////    //    await blob.UploadFromStreamAsync(stream).ConfigureAwait(false);
    ////    //}

    ////    //return blob.Uri;
    ////}
}





