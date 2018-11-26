using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Malden.Portal.GUI.Azure.Webrole.Models
{
    public class PrallelBlobTransfer
    {
        // Async events and properties
        public event EventHandler TransferCompleted;
        private bool TaskIsRunning = false;
        private readonly object _sync = new object();
        // Used to calculate download speeds
        Queue<long> timeQueue = new Queue<long>
        (100);
        Queue<long> bytesQueue = new Queue<long>
        (100);
        public Microsoft.WindowsAzure.StorageClient.CloudBlobContainer Container { get; set; }
        public PrallelBlobTransfer(Microsoft.WindowsAzure.StorageClient.CloudBlobContainer container)
        {
            Container = container;
        }


        public void DownloadBlobToFileAsync(string filePath, string blobToDownload)
        {
            var worker = new Action<Stream, string>(ParallelDownloadFile);
            lock (_sync)
            {
                if (TaskIsRunning)
                    throw new InvalidOperationException("The control is currently busy.");
                AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                var fs = File.OpenWrite(filePath);
                worker.BeginInvoke(fs, blobToDownload,TaskCompletedCallback, async);
                TaskIsRunning = true;
            }
        }

        public void DownloadBlobToBufferAsync(byte[] buffer, string blobToDownload)
        {
            var worker = new Action<Stream, string>
            (ParallelDownloadFile);
            lock (_sync)
            {
                if (TaskIsRunning)
                    throw new
                    InvalidOperationException("The control is currently busy.");
                AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                var ms = new MemoryStream(buffer);
                worker.BeginInvoke(ms, blobToDownload,
                TaskCompletedCallback, async);
                TaskIsRunning = true;
            }
        }


        public bool IsBusy
        {
            get { return TaskIsRunning; }
        }

        private int GetBlockSize(long fileSize)
        {
            const long KB = 1024;
            const long MB = 1024 * KB;
            const long GB = 1024 * MB;
            const long MAXBLOCKS = 50000;
            const long MAXBLOBSIZE = 200 * GB;
            const long MAXBLOCKSIZE = 4 * MB;
            long blocksize = 100 * KB;
            //long blocksize = 4 * MB;
            long blockCount;
            blockCount = ((int)Math.Floor((double)(fileSize / blocksize))) + 1;
            while (blockCount > MAXBLOCKS - 1)
            {
                blocksize += 100 * KB;
                blockCount = ((int)Math.Floor((double)
                (fileSize / blocksize))) + 1;
            }
            if (blocksize > MAXBLOCKSIZE)
            {
                throw new ArgumentException("Blob too big to upload.");
            }
            return (int)blocksize;
        }
        /// <summary>
        /// Uploads content to a blob using multiple threads.
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="blobName"></param>
        private void ParallelUploadStream(Stream inputStream, string blobName)
        {
            // the optimal number of transfer threads
            int numThreads = 10;
            long fileSize = inputStream.Length;
            int maxBlockSize = GetBlockSize(fileSize);
            long bytesUploaded = 0;
            // Prepare a queue of blocks to be uploaded. Each queue item is a key-value pair where
            // the 'key' is block id and 'value' is the block length.
            var queue = new Queue<KeyValuePair<int,
            int>>();
            var blockList = new List<string>();
            int blockId = 0;
            while (fileSize > 0)
            {
                int blockLength =
                (int)Math.Min(maxBlockSize, fileSize);
                string blockIdString =
                Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("BlockId{0}", blockId.ToString("0000000"))));
                KeyValuePair<int, int> kvp = new
                KeyValuePair<int, int>(blockId++, blockLength);
                queue.Enqueue(kvp);
                blockList.Add(blockIdString);
                fileSize -= blockLength;
            }
            var blob =
            Container.GetBlockBlobReference(blobName);
            blob.DeleteIfExists();
            Microsoft.WindowsAzure.StorageClient.BlobRequestOptions options = new
            Microsoft.WindowsAzure.StorageClient.BlobRequestOptions()
            {
                RetryPolicy =
                RetryPolicies.RetryExponential(
                RetryPolicies.DefaultClientRetryCount,
                RetryPolicies.DefaultMaxBackoff),
                Timeout = TimeSpan.FromSeconds(90)
            };
            // Launch threads to upload blocks.
            var tasks = new List<Task>();
            for (int idxThread = 0; idxThread < numThreads; idxThread++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    KeyValuePair<int, int> blockIdAndLength;
                    using (inputStream)
                    {
                        while (true)
                        {
                            // Dequeue block details.
                            lock (queue)
                            {
                                if (queue.Count == 0)
                                    break;
                                blockIdAndLength =
                                queue.Dequeue();
                            }
                            byte[] buff = new
                            byte[blockIdAndLength.Value];
                            BinaryReader br = new
                            BinaryReader(inputStream);
                            // move the file system reader to the proper position
                            inputStream.Seek(blockIdAndLength.Key * (long)maxBlockSize, SeekOrigin.Begin);
                            br.Read(buff, 0, blockIdAndLength.Value);
                            // Upload block.
                            string blockName = Convert.ToBase64String(BitConverter.GetBytes(blockIdAndLength.Key));
                            using (MemoryStream ms = new MemoryStream(buff, 0, blockIdAndLength.Value))
                            {
                                string blockIdString =
                                Convert.ToBase64String(
                                ASCIIEncoding.ASCII.GetBytes(string.Format("BlockId{0}", blockIdAndLength.Key.ToString("0000000"))));
                                string blockHash = GetMD5HashFromStream(buff);
                                blob.PutBlock(blockIdString, ms, blockHash, options);
                            }
                        }
                    }
                }));
            }
            // Wait for all threads to complete uploading data.
            Task.WaitAll(tasks.ToArray());
            // Commit the blocklist.
            blob.PutBlockList(blockList, options);
        }
        /// <summary>
        /// Downloads content from a blob using multiple threads.
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="blobToDownload"></param>
        public void ParallelDownloadFile(Stream outputStream, string blobToDownload)
        {
            int numThreads = 10;
            var blob =
            Container.GetBlockBlobReference(blobToDownload);
            blob.FetchAttributes();
            long blobLength = blob.Properties.Length;
            int bufferLength = GetBlockSize(blobLength);
            // 4 * 1024 * 1024;
            long bytesDownloaded = 0;
            // Prepare a queue of chunks to be downloaded. Each queue item is a key-value pair
            // where the 'key' is start offset in the blob and 'value' is the chunk length.
            Queue<KeyValuePair<long, int>> queue = new Queue<KeyValuePair<long, int>>();
            long offset = 0;
            while (blobLength > 0)
            {
                int chunkLength =
                (int)Math.Min(bufferLength, blobLength);
                queue.Enqueue(new KeyValuePair<long,
                int>(offset, chunkLength));
                offset += chunkLength;
                blobLength -= chunkLength;
            }
            int exceptionCount = 0;
            using (outputStream)
            {
                // Launch threads to download chunks.
                var tasks = new List<Task>();
                for (int idxThread = 0; idxThread <
                numThreads; idxThread++)
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        KeyValuePair<long, int>
                        blockIdAndLength;
                        // A buffer to fill per read request.
                        byte[] buffer = new byte[bufferLength];
                        while (true)
                        {
                            // Dequeue block details.
                            lock (queue)
                            {
                                if (queue.Count == 0)
                                    break;
                                blockIdAndLength =
                                queue.Dequeue();
                            }
                            try
                            {
                                // Prepare the HttpWebRequest to download data from the chunk.
                                HttpWebRequest blobGetRequest = BlobRequest.Get(blob.Uri, 60, null, null);
                                // Add header to specify the range
                                blobGetRequest.Headers.Add("x-msrange", string.Format(System.Globalization.CultureInfo.InvariantCulture, "bytes={0}-{1}",
                                blockIdAndLength.Key, blockIdAndLength.Key +
                                blockIdAndLength.Value - 1));
                                // Sign request.
                                StorageCredentials credentials = blob.ServiceClient.Credentials;
                                credentials.SignRequest(blobGetRequest);
                                // Read chunk.
                                using (HttpWebResponse response = blobGetRequest.GetResponse() as HttpWebResponse)
                                {
                                    using (Stream stream =
                                    response.GetResponseStream())
                                    {
                                        int offsetInChunk = 0;
                                        int remaining =
                                        blockIdAndLength.Value;
                                        while (remaining > 0)
                                        {
                                            int read =
                                            stream.Read(buffer, offsetInChunk, remaining);
                                            lock (outputStream)
                                            {
                                                outputStream.Position =
                                                blockIdAndLength.Key + offsetInChunk;
                                                outputStream.Write(buffer, offsetInChunk, read);
                                            }
                                            offsetInChunk += read;
                                            remaining -= read;
                                            Interlocked.Add(ref bytesDownloaded, read);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Add block back to queue
                                queue.Enqueue(blockIdAndLength);
                                exceptionCount++;
                                // If we have had more than 100 exceptions then break
                                if (exceptionCount == 100)
                                {
                                    throw new Exception("Received 100 exceptions while downloading." + ex.ToString());
                                }
                                if (exceptionCount >= 100)
                                {
                                    break;
                                }
                            }
                        }
                    }));
                }
                // Wait for all threads to complete downloading data.
                Task.WaitAll(tasks.ToArray());
            }
        }
        private void
        TaskCompletedCallback(IAsyncResult ar)
        {
            // get the original worker delegate and the
            AsyncOperation instance;
            Action<Stream, string> worker = (Action<Stream, string>)
            ((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async = (AsyncOperation)ar.AsyncState;
            // finish the asynchronous operation
            worker.EndInvoke(ar);
            // clear the running task flag
            lock (_sync)
            {
                TaskIsRunning = false;
            }
            //Add comment
            // raise the completed event
            async.PostOperationCompleted(state =>
            OnTaskCompleted((EventArgs)state), new
            EventArgs());
        }

        protected virtual void OnTaskCompleted(EventArgs e)
        {
            if (TransferCompleted != null)
                TransferCompleted(this, e);
        }

        private string GetMD5HashFromStream(byte[] data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] blockHash = md5.ComputeHash(data);
            return Convert.ToBase64String(blockHash, 0, 16);
        }
    }

}
