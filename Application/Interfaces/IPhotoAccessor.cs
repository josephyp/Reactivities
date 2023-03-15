using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    //Handles Cloudinary tasks
    public interface IPhotoAccessor
    {
        //IFormFile represents the file sent with the HttpRequest
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        Task<string> DeletePhoto(string publicId);
    }
}