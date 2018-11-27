namespace LenesKlinik.Core.Entities
{
    public class Work
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
    }
}