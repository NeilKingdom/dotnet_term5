using Assignment2.Data;
using Azure.Storage.Blobs;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using Assignment2.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly MarketDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "advertisements";
        private readonly string[] _permittedExtensions = { ".jpeg", ".jpg", ".png" };

        /**
            Default constructor 
         */
        public AdvertisementsController(MarketDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        /**
            Index page for Advertisements. Returns a 404
            if the brokerage ID is not provided. Otherwise,
            it displays all ads for the selected brokerage.
         */
        public async Task<IActionResult> Index(string id)
        {
            Brokerage brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null) return NotFound();

            AdsViewModel adsViewModel = new()
            {
                Brokerage = brokerage,
                Advertisements = await _context.Advertisements
                    .Where(a => a.BrokerageId == id)
                    .ToListAsync()
            };

            adsViewModel.Brokerage.Title = brokerage.Title;
            return View(adsViewModel);
        }

        /**
            Returns the Advertisement Create.cshtml page
            where the uesr can create a new advertisement.
            Sending an adsViewModel to the page is required
            since the razor code depends on a few properties
            of the model.
         */
        public async Task<IActionResult> Create(string id)
        {
            Brokerage brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null) return NotFound();

            AdsViewModel adsViewModel = new()
            {
                Brokerage = brokerage,
                Advertisements = await _context.Advertisements
                    .Where(a => a.BrokerageId == id)
                    .ToListAsync()
            };

            adsViewModel.Brokerage.Title = brokerage.Title;
            return View(adsViewModel);
        }

        /**
            Create a new advertisement by uploading the incoming 
            IFormFile to blob storage. A new Advertisement entity
            is also created and added to the DB.
         */
        [HttpPost]
        public async Task<IActionResult> Create(string id, IFormFile file)
        {
            Brokerage brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null) return NotFound();

            AdsViewModel adsViewModel = new()
            {
                Brokerage = brokerage,
                Advertisements = await _context.Advertisements
                    .Where(a => a.BrokerageId == id)
                    .ToListAsync()
            };

            if (file == null)
            {
                ViewBag.FileErr = "No file was selected";
                return View(adsViewModel);
            }
            else
            {
                // File extension validation
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !Array.Exists(_permittedExtensions, e => e.Contains(ext)))
                {
                    ViewBag.FileErr = "Permitted file extensions are .jpeg, .jpg, and .png";
                    return View(adsViewModel);
                }

                Advertisement ad = new() { BrokerageId = id };
                BlobContainerClient containerClient;

                // Create the container and return a container client object
                try
                {
                    containerClient = await _blobServiceClient.CreateBlobContainerAsync(_containerName);
                    // Provide public access to blob storage
                    containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer); 
                }
                catch (RequestFailedException)
                {
                    containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                }

                try
                {
                    string randomFileName = Path.GetRandomFileName();
                    ad.FileName = randomFileName;

                    // Create the blob to hold the data
                    var blockBlob = containerClient.GetBlobClient(randomFileName);
                    if (await blockBlob.ExistsAsync()) await blockBlob.DeleteAsync();

                    using (var memoryStream = new MemoryStream())
                    {
                        // Copy file data into memory
                        await file.CopyToAsync(memoryStream);

                        // Upload the file if less than 2 MB
                        if (memoryStream.Length < (2 * 1024 * 1024))
                        {
                            memoryStream.Position = 0; // Rewind to beginning of memory stream
                            await blockBlob.UploadAsync(memoryStream);
                            memoryStream.Close();
                        }
                        else
                        {
                            ViewBag.FileErr = "File is too large";
                            return View(adsViewModel);
                        }
                    }
                }
                catch (RequestFailedException)
                {
                    return View("Error", new ErrorViewModel("Something went wrong uploading blob"));
                }

                // Add advertisement to DB
                ad.AdUrl = containerClient.GetBlobClient(ad.FileName).Uri.AbsoluteUri;
                _context.Advertisements.Add(ad);
                await _context.SaveChangesAsync();
            }

            return Redirect($"/Advertisements/Index/{id}");
        }

        /**
            Redirect to the Delete.cshtml View for Advertisements.
            Returns a 404 if an advertisement with advertisementId
            matching id in the URL is not found.
         */
        public async Task<IActionResult> Delete(int id)
        {
            Advertisement ad = await _context.Advertisements.FindAsync(id);
            if (ad == null) return NotFound();

            Brokerage brokerage = await _context.Brokerages
                .Where(b => b.BrokerageId == ad.BrokerageId)
                .SingleAsync();

            AdsViewModel adsViewModel = new()
            {
                Brokerage = brokerage,
                Advertisements = await _context.Advertisements
                    .Where(b => b.BrokerageId == brokerage.BrokerageId && b.AdvertisementId == id)
                    .ToListAsync()
            };

            return View(adsViewModel);
        }

        /**
            Delete an advertisement by removing it from
            the DB and deleting it from blob storage. We
            only delete the image from blob storage if the
            image is only has one external reference to it.
         */
        [HttpPost]
        public async Task<IActionResult> Delete(int id, AdsViewModel adsViewModel)
        {
            Advertisement ad = await _context.Advertisements.FindAsync(id);
            if (ad == null) return NotFound();

            if (!ModelState.IsValid)
                return View("Error", new ErrorViewModel("Something went wrong deleting the advertisement."));

            BlobContainerClient containerClient;
            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error", new ErrorViewModel("Failed to retrieve blob container"));
            }

            // Only delete the image if only one reference to it exists
            if (containerClient.GetBlobs().Count() == 1)
            {
                foreach (var blob in containerClient.GetBlobs())
                {
                    try
                    {
                        // Get the blob that holds the data
                        var blockBlob = containerClient.GetBlobClient(blob.Name);
                        if (await blockBlob.ExistsAsync())
                        {
                            await blockBlob.DeleteAsync();
                        }
                    }
                    catch (RequestFailedException)
                    {
                        return View("Error", new ErrorViewModel("Failed to retrieve blob data"));
                    }
                }
            }

            // Remove advertisement from DB
            _context.Advertisements.Remove(ad);
            await _context.SaveChangesAsync();

            var brokerageId = ad.BrokerageId;
            return Redirect($"/Advertisements/Index/{brokerageId}");
        }
    }
}
