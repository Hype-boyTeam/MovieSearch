using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MovieSearch.Models;

public class PosterService
{
    private static readonly HashSet<string> AcceptableContentType = new()
    {
        "image/jpeg",
        "image/png",
        "image/webp",
    };

    private readonly BlobContainerClient _container;

    public PosterService(BlobContainerClient container)
    {
        _container = container;
    }

    /// <summary>
    /// 포스터 이미지를 Azure Blob Storage에 올립니다.
    /// </summary>
    /// <param name="movieId">포스터를 올릴 영화의 id입니다.</param>
    /// <param name="poster">등록할 포스터 사진 파일입니다.</param>
    /// <returns>
    /// 올린 사진 파일을 웹에서 접근할 수 있는 Url입니다.
    /// </returns>
    /// <exception cref="UnsupportedContentTypeException">
    /// 올리고자 하는 파일이 jpeg, png, webp 형식 중 하나에도 해당하지 않는다면 발생합니다.
    /// </exception>
    public async Task<Uri> UploadImage(Guid movieId, IFormFile poster)
    {
        // 이미지 파일 형식이 맞나 검사
        if (!AcceptableContentType.Contains(poster.ContentType))
        {
            throw new UnsupportedContentTypeException(
                $"{poster.FileName} has a content type {poster.ContentType} which is unacceptable as an image file.");
        }

        // 이미지를 Az blob storage에 업로드
        var blobClient = _container.GetBlobClient($"posters/{movieId}");
        await blobClient.UploadAsync(poster.OpenReadStream(), new BlobHttpHeaders
        {
            ContentType = poster.ContentType,
        });

        return blobClient.Uri;
    }
}
