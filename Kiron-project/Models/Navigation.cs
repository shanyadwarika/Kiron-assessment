public class Navigation
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int? ParentId { get; set; } // For nested menus
    public bool IsVisible { get; set; }

    public Navigation Parent { get; set; }
    public ICollection<Navigation> Children { get; set; }
}