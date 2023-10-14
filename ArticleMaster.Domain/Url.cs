namespace ArticleMaster.Domain;

public record Url(string Domain, string Lang, string EntitiesName, string Number)
{
    public readonly string UrlString = $"{Domain}/{Lang}/{EntitiesName}/{Number}/";
}