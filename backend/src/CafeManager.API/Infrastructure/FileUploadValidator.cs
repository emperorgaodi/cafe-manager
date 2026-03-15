namespace CafeManager.API.Infrastructure;

/// <summary>
/// Validates uploaded image files by checking both file size and magic bytes
/// (the actual file header), not just the Content-Type header sent by the client.
/// Trusting only Content-Type is a security vulnerability — any file can claim any type.
/// </summary>
public static class FileUploadValidator
{
    private const long MaxSizeBytes = 2 * 1024 * 1024; // 2 MB

    // Magic byte signatures for allowed image types
    private static readonly (byte[] Signature, string MimeType)[] AllowedSignatures =
    [
        (new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg"),
        (new byte[] { 0x89, 0x50, 0x4E, 0x47 }, "image/png"),
        (new byte[] { 0x47, 0x49, 0x46 }, "image/gif"),
        (new byte[] { 0x52, 0x49, 0x46, 0x46 }, "image/webp"),
    ];

    /// <summary>
    /// Validates the file size and reads magic bytes to confirm it is a genuine image.
    /// Throws <see cref="ArgumentException"/> with a user-facing message on failure.
    /// </summary>
    public static async Task ValidateAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new ArgumentException("Logo file is empty.");

        if (file.Length > MaxSizeBytes)
            throw new ArgumentException("Logo file must not exceed 2 MB.");

        // Read the first 8 bytes to check the magic bytes signature
        var header = new byte[8];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header);

        if (bytesRead < 3 || !AllowedSignatures.Any(sig => header.Take(sig.Signature.Length).SequenceEqual(sig.Signature)))
            throw new ArgumentException("Logo must be a valid image file (JPEG, PNG, GIF, or WebP).");
    }
}
