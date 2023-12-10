namespace shr.API.Models.Share
{
    public class ShareGet
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public DateTime UpdateDate { get; set; }

        public DateTime CreationDate { get; set; }

        public ShareGet(Share share)
        {
            Id = share.Id;
            Name = share.Name;
            ContentType = share.ContentType;
            UpdateDate = share.UpdateDate;
            CreationDate = share.CreationDate;
        }
    }
}
