namespace ScrumExtreme.Domain.Entities;

public class UserStory : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
