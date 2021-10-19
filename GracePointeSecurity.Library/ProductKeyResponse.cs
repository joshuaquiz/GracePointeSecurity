namespace GracePointeSecurity.Library
{
    public sealed class ProductKeyResponse
    {
        public bool IsAlreadySetup { get; set; }

        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }
    }
}