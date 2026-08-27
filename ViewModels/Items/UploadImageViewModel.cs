using Microsoft.AspNetCore.Http;

namespace Moonatna.ViewModels.Items;

// Multipart upload for the quick-add dialog's optional photo.
public class UploadImageViewModel
{
    public int ItemId { get; set; }
    public IFormFile? Photo { get; set; }
}
