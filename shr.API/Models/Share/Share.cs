namespace shr.API.Models.Share
{
    public class Share
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Secret { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public DateTime UpdateDate { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
