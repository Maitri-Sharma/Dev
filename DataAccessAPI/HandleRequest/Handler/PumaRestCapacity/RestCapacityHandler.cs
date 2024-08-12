using DataAccessAPI.HandleRequest.Request.PumaRestCapacity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Shared;
using System.IO;
using static Puma.Shared.PumaEnum;
using System.IO.Compression;
using DataAccessAPI.Helper;
using Microsoft.Extensions.Configuration;
using DataAccessAPI.Controllers;
using Hangfire.Server;
using Hangfire.Console;
using System.Reflection;
using Puma.Infrastructure.Interface.KsupDB.RestCapacity;

namespace DataAccessAPI.HandleRequest.Handler.PumaRestCapacity
{
    public class RestCapacityHandler : IRequestHandler<RequestRestCapacity, bool>
    {
        #region Private variables

        private static string qName = string.Empty;
        private static CapacityInfo currentMessages;
        // private static string documentInputPath = string.Empty;

        private static string documentOutputPathZipped = string.Empty;
        private static string documentOutputPathUnZipped = string.Empty;

        private PerformContext _context;

        // private static string unZippedDocumentFilter = "*.xml";
        // private static string zippedDocumentFilter = "*.gz";

        private static string unZippedDocumentExtension = ".xml";
        private static string zippedDocumentExtension = ".gz";
        private static string fileNameZipped = string.Empty;
        private static string fileNameUnZipped = string.Empty;
        private static List<string> messages = new List<string>();
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<RestCapacityHandler> _logger;

        /// <summary>
        /// The get PRS calendar admin details repository
        /// </summary>
        private readonly IPumaRestCapacityRepository _pumaRestCapacityRepository;

        /// <summary>
        /// The XML logger
        /// </summary>
        private readonly XML _xml;
       // private readonly RestCapacityController restCapacityController;
        private readonly IConfiguration _configuration;
        private readonly IRestCapacityRepository _restCapacityRepository;
        private readonly EMail _email;
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="RestCapacityHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="pumaRestCapacityRepository">The get PRS calendar admin details repository.</param>
        /// <param name="xmllogger">XML logger.</param>
        /// <param name="configuration"></param>
        /// <param name="restCapacity"></param>
        /// <param name="email"></param>
        /// <param name="restCapacityRepository"></param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// getPrsCalendarAdminDetailsRepository
        /// or
        /// xmllogger
        /// or
        /// configuration
        /// or
        /// restCapacity
        /// </exception>
        public RestCapacityHandler(ILogger<RestCapacityHandler> logger, IPumaRestCapacityRepository pumaRestCapacityRepository, ILogger<XML> xmllogger, IConfiguration configuration, ILogger<EMail> email, IRestCapacityRepository restCapacityRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pumaRestCapacityRepository = pumaRestCapacityRepository ?? throw new ArgumentNullException(nameof(pumaRestCapacityRepository));
            _xml = new XML(xmllogger, pumaRestCapacityRepository, configuration);
            _email = new EMail(configuration);
            _configuration = configuration;
            //fileNameZipped = Convert.ToString(configuration.GetValue("FileNameZipped", typeof(string)));
            fileNameZipped = _configuration.GetValue<string>("fileNameZipped");
            fileNameUnZipped = _configuration.GetValue<string>("fileNameUnZipped");
            documentOutputPathZipped = _configuration.GetValue<string>("FileLogging.ObjectLogPathZipped");
            //documentOutputPathUnZipped = Convert.ToString(configuration.GetValue("FileLogging.ObjectLogPathUnZipped", typeof(string)));
            documentOutputPathUnZipped = _configuration.GetValue<string>("FileLogging.ObjectLogPathUnZipped");
            //unZippedDocumentExtension = Convert.ToString(configuration.GetValue("UnZippedDocumentExtension", typeof(string)));
            unZippedDocumentExtension = _configuration.GetValue<string>("UnZippedDocumentExtension");
            //zippedDocumentExtension = Convert.ToString(configuration.GetValue("ZippedDocumentExtension", typeof(string)));
            zippedDocumentExtension = _configuration.GetValue<string>("ZippedDocumentExtension");
            //currentParameter = "QueueName";
            //qName = Convert.ToString(configuration.GetValue("QueueName", typeof(string)));
            currentMessages = new CapacityInfo();
            _restCapacityRepository = restCapacityRepository ?? throw new ArgumentNullException(nameof(restCapacityRepository));
        }

        public async Task<bool> Handle(RequestRestCapacity request, CancellationToken cancellationToken)
        {
            _context = request.context;
            _logger.LogInformation("Starter import av Restkapasitet. Ikke avbryt");
            //var appData = new AppSettingsReader();
            //string importSource = Conversions.ToString(appData.GetValue("ImportSource", typeof(string)));
            //_logger.LogInformation("Sletter eksisterende data i databasen", FileLogging.MessageType.InfoMessage);
            string executableLocation = Path.GetDirectoryName(
                   Assembly.GetExecutingAssembly().Location);
            string xslLocation = Path.Combine(executableLocation, "RestCapacity.xsd");
            _context.WriteLine("File Path" + xslLocation);
            CapacityInfo capacityInfo = null;
            capacityInfo = await ReadMessages();

            if (capacityInfo.NumberOfMessages == 0)
            {
                _email.SendMail("File not present");
                _logger.LogInformation("Ingen meldinger funnet. Sendt varsling.", RestCapacityMessageType.InfoMessage);
                _logger.LogInformation("Restkapasitet");
                return false;
            }
            else
            {
                _email.SendMail("File ready to process");
                _logger.LogInformation(string.Format("Antall meldinger funnet {0}", capacityInfo.NumberOfMessages.ToString()), RestCapacityMessageType.InfoMessage);
                await _pumaRestCapacityRepository.Kapasitet_Ruter_Dato_AllAsync();
                _xml.CheckCreationDate(ref capacityInfo);
                //if (!capacityInfo.IsFresh)
                //{
                //    EMail.SendMail(capacityInfo.NumberOfMessages, capacityInfo.FreshFileDate);
                //    _logger.LogInformation("Nyeste melding tilfredstiller IKKE krav til ferskhet. Sendt varsling", RestCapacityMessageType.InfoMessage);
                //    return false;
                //}
                //else
                //{
                await _xml.ExtractRestCapacityInfo(capacityInfo.FreshFile);
                //}
            }
            _logger.LogInformation("Flytter data fra input til destinasjonstabeller", RestCapacityMessageType.InfoMessage);
            await _pumaRestCapacityRepository.Kapasitet_Delete_And_ImportAsync();
            _email.SendMail("File processing completed");
            return true;
        }


        #region Private Methods

        #region ReadMessages

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<CapacityInfo> ReadMessages()
        {
            if (!(await _restCapacityRepository.DownloadFile()))

                PersistMessage();
            //commented for now
            //ErgoGroup.ToolBox.WebSphereMQ.Open(ErgoGroup.ToolBox.WebSphereMQ.qOpenFor.Read);

            //    while (true)
            //    {
            //       var message = ErgoGroup.ToolBox.WebSphereMQ.MessageGetBinary();
            //        if (ErgoGroup.ToolBox.WebSphereMQ.IsEndOfQueue)
            //        {
            //            break;
            //        }

            //    }

            return currentMessages;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private void PersistMessage()
        {
            string fileNameSuffix = CreateFileNameSuffix();
            var directory = new DirectoryInfo(documentOutputPathZipped);
            //var myFile = (from f in directory.GetFiles()
            //              orderby f.LastWriteTime descending
            //              select f).First();
            var outputFileZipped = directory.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .First();

            if (Directory.EnumerateFileSystemEntries(documentOutputPathUnZipped, "*.xml").ToList<string>().Count != 0)
            {
                var directory_unzipped = new DirectoryInfo(documentOutputPathUnZipped);

                var outputFile_UnZipped = directory_unzipped.GetFiles()
                 .OrderByDescending(f => f.LastWriteTime)
                 .First();
                if (outputFile_UnZipped.CreationTime.Date != DateTime.Now.Date)
                {
                    byte[] message = File.ReadAllBytes(outputFileZipped.ToString());
                    string outputFileUnZipped = string.Format("{0}{1}_{2}{3}", documentOutputPathUnZipped, fileNameUnZipped, fileNameSuffix, unZippedDocumentExtension);
                    WriteToFile(message, outputFileZipped.ToString());
                    currentMessages.ZipFiles.Add(outputFileZipped.ToString());
                    Console.WriteLine("Lagret komprimert melding");
                    _logger.LogInformation(string.Format("Lagret komprimert melding {0}", outputFileZipped), RestCapacityMessageType.InfoMessage);
                    DecompressAndSave(outputFileZipped.ToString(), outputFileUnZipped);
                    currentMessages.UnzipFiles.Add(outputFileUnZipped);
                    Console.WriteLine("Lagret dekomprimert melding");
                    _logger.LogInformation(string.Format("Lagret dekomprimert melding {0}", outputFileUnZipped), RestCapacityMessageType.InfoMessage);
                }
                else
                {
                    currentMessages.UnzipFiles.Add(outputFile_UnZipped.ToString());
                }
                currentMessages.NumberOfMessages += 1;
            }
            else
            {
                byte[] message = File.ReadAllBytes(outputFileZipped.ToString());
                string outputFileUnZipped = string.Format("{0}{1}_{2}{3}", documentOutputPathUnZipped, fileNameUnZipped, fileNameSuffix, unZippedDocumentExtension);
                WriteToFile(message, outputFileZipped.ToString());
                currentMessages.ZipFiles.Add(outputFileZipped.ToString());
                Console.WriteLine("Lagret komprimert melding");
                _logger.LogInformation(string.Format("Lagret komprimert melding {0}", outputFileZipped), RestCapacityMessageType.InfoMessage);
                DecompressAndSave(outputFileZipped.ToString(), outputFileUnZipped);
                currentMessages.UnzipFiles.Add(outputFileUnZipped);
                Console.WriteLine("Lagret dekomprimert melding");
                _logger.LogInformation(string.Format("Lagret dekomprimert melding {0}", outputFileUnZipped), RestCapacityMessageType.InfoMessage);
                currentMessages.NumberOfMessages += 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="outputPath"></param>
        private static void WriteToFile(byte[] fileContent, string outputPath)
        {
            var fs = new FileStream(outputPath, FileMode.Create);
            fs.Write(fileContent, 0, fileContent.Length);
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string CreateFileNameSuffix()
        {
            return string.Format("{0}", DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss").Replace(".", string.Empty));
        }

        #region DecompressAndSave

        public static void Decompress(string ZipFilePath, string UnZipFilePath)
        {
            var file = new FileInfo(ZipFilePath);
            using (var inFile = file.OpenRead())
            {
                using (var outFile = File.Create(UnZipFilePath))
                {
                    using (var lDecompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        lDecompress.CopyTo(outFile);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ZipFilePath"></param>
        /// <param name="UnZipFilePath"></param>
        public static void DecompressAndSave(string ZipFilePath, string UnZipFilePath)
        {
            var file = new FileInfo(ZipFilePath);
            try
            {
                using (var inFile = file.OpenRead())
                {
                    using (var outFile = File.Create(UnZipFilePath))
                    {
                        using (var Decompress = new GZipStream(inFile, CompressionMode.Decompress))
                        {
                            Decompress.CopyTo(outFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region CompressAndSave

        public static void Compress(string UnZipFilePath, string ZipFilePath)
        {
            var file = new FileInfo(UnZipFilePath);
            using (var inFile = file.OpenRead())
            {
                using (var outFile = File.Create(ZipFilePath))
                {
                    using (var lCompress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        inFile.CopyTo(lCompress);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UnZipFilePath"></param>
        /// <param name="ZipFilePath"></param>
        public static void CompressAndSave(string UnZipFilePath, string ZipFilePath)
        {
            var file = new FileInfo(UnZipFilePath);
            if (File.Exists(ZipFilePath))
            {
                File.Delete(ZipFilePath);
            }

            using (var inFile = file.OpenRead())
            {
                using (var outFile = File.Create(ZipFilePath))
                {
                    using (var Compress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        inFile.CopyTo(Compress);
                    }
                }
            }
        }


        #endregion
        #endregion
    }
}
