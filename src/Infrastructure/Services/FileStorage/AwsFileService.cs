using Application.Services.Contracts;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.FileStorage
{
    public class AwsFileService : IFileService
    {
        public Task<bool> DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadFile(IFormFile file, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}