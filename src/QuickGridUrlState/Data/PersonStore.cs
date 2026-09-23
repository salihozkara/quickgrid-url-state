namespace QuickGridUrlState.Data;

public sealed class PersonStore
{
    private static readonly string[] FirstNames =
    [
        "Amelia", "Oliver", "Ava", "Noah", "Isla", "George", "Freya", "Arthur",
        "Ivy", "Leo", "Grace", "Oscar", "Ella", "Henry", "Sofia", "Archie",
        "Mia", "Joshua", "Lily", "Charlie", "Evie", "Jacob", "Poppy", "Alfie",
        "Salih", "Ahmet", "Daria", "Yunus", "Elif", "Mert", "Zeynep", "Emre"
    ];

    private static readonly string[] LastNames =
    [
        "Smith", "Jones", "Taylor", "Brown", "Williams", "Wilson", "Johnson",
        "Davies", "Robinson", "Wright", "Thompson", "Evans", "Walker", "White",
        "Roberts", "Green", "Hall", "Thomas", "Clarke", "Yilmaz", "Kaya",
        "Demir", "Celik", "Sahin", "Tiurina", "Novak", "Silva", "Costa"
    ];

    private static readonly string[] Countries =
    [
        "United Kingdom", "Türkiye", "Germany", "Portugal", "Czechia",
        "Brazil", "United States", "Netherlands"
    ];

    public IReadOnlyList<Person> People { get; }

    public PersonStore()
    {
        // Fixed seed: the same 247 rows on every run, so URLs are reproducible.
        var rng = new Random(Seed: 20260923);
        People = Enumerable.Range(1, 247)
            .Select(id => new Person(
                Id: id,
                Name: $"{FirstNames[rng.Next(FirstNames.Length)]} {LastNames[rng.Next(LastNames.Length)]}",
                Country: Countries[rng.Next(Countries.Length)],
                Age: rng.Next(21, 68),
                StartDate: DateOnly.FromDayNumber(rng.Next(
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-6)).DayNumber,
                    DateOnly.FromDateTime(DateTime.Today).DayNumber))))
            .ToList();
    }
}
