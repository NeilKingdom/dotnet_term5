using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab5.Models;
using Lab5.Data;
using Azure.Storage.Blobs;
using Azure;

namespace Lab5.Pages.AnswerImages
{
    [ValidateAntiForgeryToken]
    public class DeleteModel : PageModel
    {
        private readonly AnswerImageDataContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";

        public DeleteModel(AnswerImageDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        [BindProperty]
        public AnswerImage AnswerImage { get; set; }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (_context.AnswerImages == null || await _context.AnswerImages.FindAsync(id) == null)
            {
                return NotFound();
            }

            BlobContainerClient containerClient;

            // Find Answer image with matching id
            AnswerImage = await _context.AnswerImages.FindAsync(id);

            // Select the appropriate container
            string containerName = (AnswerImage.Question == Question.Computer) ? computerContainerName : earthContainerName;

            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/Error");
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
                        return RedirectToPage("/Error");
                    }
                }
            }

            // Delete answer image from DB
            _context.AnswerImages.Remove(AnswerImage);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
