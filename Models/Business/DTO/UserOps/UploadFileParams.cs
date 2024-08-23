namespace MotorcycleRental.Models.DTO
{
    public class UploadFileParams
    {
        public Guid UserID { get; set; }
        public string? Format { get; set; }
        public string? FileContents { get; set; }

        public UploadFileParams() { }

        public UploadFileParams(Guid userId, string? fileContents, string? format)
        {
            UserID = userId;
            FileContents = fileContents;
            Format = format;
        }
    }
}