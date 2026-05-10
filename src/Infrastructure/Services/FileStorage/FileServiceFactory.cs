using Application.Configurations;
using Application.Services.Contracts;
using Infrastructure.Services.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class FileServiceFactory : IFileServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly StorageSettings _settings;

    public FileServiceFactory(IServiceProvider serviceProvider, IOptions<StorageSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    public IFileService Create()
    {
        return _settings.StorageType switch
        {
            "Cloudinary" => _serviceProvider.GetRequiredService<CloudinaryFileService>(),
            "Local" => _serviceProvider.GetRequiredService<LocalFileService>(),
            "Aws_S3" => _serviceProvider.GetRequiredService<AwsFileService>(),
            _ => throw new ArgumentException($"Invalid storage type: {_settings.StorageType}")
        };
    }
}