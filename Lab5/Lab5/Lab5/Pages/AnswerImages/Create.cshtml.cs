using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure;

namespace Lab5.Pages.AnswerImages
{
    [ValidateAntiForgeryToken]
    public class CreateModel : PageModel
    {
        private readonly AnswerImageDataContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";
        private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };

        public CreateModel(AnswerImageDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient= blobServiceClient;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public AnswerImage AnswerImage { get; set; }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            // Ensure that a file was provided
            if (file == null || file.Length == 0)
            {
                return RedirectToPage("/Error");
            }

            // File extension validation
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return RedirectToPage("/Error");
            }

            BlobContainerClient containerClient;

            // Select the appropriate container
            AnswerImage.Question = (Request.Form["question"] == "Computer") ? Question.Computer : Question.Earth;
            string containerName = (AnswerImage.Question == Question.Computer) ? computerContainerName : earthContainerName;

            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                // Give access to the public 
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                AnswerImage.FileName = Path.GetRandomFileName();
                // Create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(AnswerImage.FileName);

                // Overwrite file if it exists
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // Copy the file into memory
                    await file.CopyToAsync(memoryStream);

                    // Rewind file pointer for copy
                    memoryStream.Position = 0;

                    // Send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/Error");
            }

            // Set Url to be the Uri of the blob content in Azure storage
            AnswerImage.Url = containerClient.GetBlobClient(AnswerImage.FileName).Uri.AbsoluteUri;
            // Save properties to DB
            _context.AnswerImages.Add(AnswerImage);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
