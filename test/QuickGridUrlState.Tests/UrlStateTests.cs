using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace QuickGridUrlState.Tests;

public class UrlStateTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DefaultRender_SortHeadersAreLinks()
    {
        var html = await _client.GetStringAsync("/people");
        Assert.Contains("class=\"col-title\" href=", html);
        Assert.Contains("sort=Name&amp;direction=asc", html);
        Assert.DoesNotContain("<button class=\"col-title\"", html);
    }

    [Fact]
    public async Task SortAndPage_AreAppliedFromUrl()
    {
        var html = await _client.GetStringAsync("/people?sort=Name&direction=desc&page=2");
        Assert.Contains("aria-sort=\"descending\"", html);
        Assert.Contains("Zeynep Brown", html);          // first row of Name desc, page 2
        Assert.DoesNotContain("Ahmet Sahin", html);     // page-1-only name
        Assert.Contains("Page <strong", html);
    }

    [Fact]
    public async Task Paginator_PreservesSortInLinks()
    {
        var html = await _client.GetStringAsync("/people?sort=Name&direction=desc&page=2");
        Assert.Contains("go-next\" href=\"http://localhost/people?sort=Name&amp;direction=desc&amp;page=3\"", html);
    }

    [Fact]
    public async Task OwnFilterParam_CoexistsAndIsPreserved()
    {
        var html = await _client.GetStringAsync("/people?country=Germany&sort=Age&direction=asc");
        // grid links keep the foreign country parameter
        Assert.Contains("country=Germany&amp;sort=", html);
        // every rendered body row is Germany
        var rows = System.Text.RegularExpressions.Regex.Matches(html, "<tr[^>]*>(.*?)</tr>", System.Text.RegularExpressions.RegexOptions.Singleline);
        var bodyRows = rows.Where(r => r.Value.Contains("<td")).ToList();
        Assert.NotEmpty(bodyRows);
        Assert.All(bodyRows, r => Assert.Contains("Germany", r.Value));
    }

    [Theory]
    [InlineData("/people?page=999", "241")]   // clamps to last page
    [InlineData("/people?page=abc", "1")]     // invalid -> first page
    [InlineData("/people?page=0", "1")]       // zero -> first page
    [InlineData("/people?sort=Bogus", "1")]   // unknown column ignored
    [InlineData("/people?sort=Name", "1")]    // sort without direction ignored
    public async Task MalformedOrOutOfRange_FallsBackSafely(string url, string firstId)
    {
        var html = await _client.GetStringAsync(url);
        var firstCell = System.Text.RegularExpressions.Regex.Match(html, "<td[^>]*>(\\d+)</td>");
        Assert.Equal(firstId, firstCell.Groups[1].Value);
    }

    [Fact]
    public async Task SecondGrid_UsesPrefixedParameters()
    {
        var html = await _client.GetStringAsync("/multi?c_sort=People&c_direction=desc&c_page=2");
        Assert.Contains("c_sort=Name", html);
        Assert.Contains("c_page=", html);
        // second grid shows highest People counts on page 2 sorted desc
        Assert.Contains("Netherlands", html);
    }

    [Fact]
    public async Task InteractivePage_PrerendersUrlState()
    {
        var html = await _client.GetStringAsync("/people-interactive?sort=Name&direction=asc");
        Assert.Contains("aria-sort=\"ascending\"", html);
        Assert.Contains("class=\"col-title\" href=", html);
    }

    [Fact]
    public async Task StatusCodes_AreOk()
    {
        foreach (var url in new[] { "/people", "/multi", "/provider", "/people-interactive" })
            Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync(url)).StatusCode);
    }
}
