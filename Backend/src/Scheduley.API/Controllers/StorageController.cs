using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scheduley.API.Extensions;
using Scheduley.API.Filters;
using Scheduley.Core.Contracts;

namespace Scheduley.API.Controllers;

[Authorize]
[ServiceFilter(typeof(UserConfidentialDataFilter))]
public class StorageController(IMinioService minioService) : CustomControllerBase
{
    [HttpPost("upload")]
    [EndpointSummary("Upload a file")]
    [EndpointDescription(
        "Takes a file from a form upload and stores it in the user's private bucket"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100MB limit
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        //!1) Open file stream
        using var stream = file.OpenReadStream();

        //!2) Retrieve bucket name
        var user = HttpContext.GetRequiredCurrentUser();
        if (user == null)
            return NotFound(
                "User does not exist, or may have been deleted after issuing the token"
            );

        //!3) Upload the file finally
        await minioService.UploadAsync(file.FileName, stream, file.Length, user);
        return Ok("File successfully uploaded!");
    }

    [HttpGet("download/{objectName}")]
    [EndpointDescription(
        "Generates a pre-signed URL that allows direct file download without additional authentication"
    )]
    [EndpointSummary("Download file by link")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    public async Task<IActionResult> Download([FromRoute] string objectName)
    {
        var user = HttpContext.GetRequiredCurrentUser();
        if (user == null)
            return NotFound(
                "User does not exist, or may have been deleted after issuing the token"
            );
        return Ok(await minioService.DownloadAsync(objectName, user));
    }

    [HttpGet("files")]
    [EndpointDescription(
        "Returns all files in the user's bucket along with total bucket size information"
    )]
    [EndpointSummary("Get user files")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    public IActionResult GetFilesPerUserBucket()
    {
        var user = HttpContext.GetRequiredCurrentUser();
        if (user == null)
            return NotFound(
                "User does not exist, or may have been deleted after issuing the token"
            );

        ulong rawSize = minioService.GetBucketSize(user);
        return Ok(
            new
            {
                bucketItems = minioService.GetBucketItems(user),
                sizeInBytes = rawSize,
                size = ((long)rawSize).Bytes().Humanize(),
            }
        );
    }
}
