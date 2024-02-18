using BookStore.MultiLingualObjects;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Books;
public class AddBookTranslationDto : IObjectTranslation
{
    [Required]
    public string Language { get; set; }

    [Required]
    public string Name { get; set; }
}
