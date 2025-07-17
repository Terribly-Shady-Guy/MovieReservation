namespace ApplicationLogic.ViewModels
{
    public class MovieListItem
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required List<string> Genres { get; set; }
    }
}
