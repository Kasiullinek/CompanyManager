using Microsoft.Playwright;

public class ResourceTests
{
    private const string BaseUrl = "https://localhost:7284";

    [Fact]
    public async Task NotAuthorizedUser_SeesWelcomeMessage()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/");

        var heading = await page.TextContentAsync("h1");
        Assert.Equal("Hello, world!", heading);

        var welcomeText = await page.TextContentAsync("body");
        Assert.Contains("Welcome to Company Management App", welcomeText);
    }

    [Fact]
    public async Task Admin_SeesAllResourcesAndAddButton()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        var heading = await page.TextContentAsync("h1.text-center");
        Assert.Equal("List of All Resources", heading);

        var addButtonVisible = await page.IsVisibleAsync("button:has-text('Add Resource')");
        Assert.True(addButtonVisible);

        var tableVisible = await page.IsVisibleAsync("table");
        Assert.True(tableVisible);

        var rowsCount = await page.EvalOnSelectorAllAsync<int>("tbody tr", "els => els.length");
        Assert.True(rowsCount > 0);
    }

    [Fact]
    public async Task User_SeesOnlyAssignedResources()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        var heading = await page.TextContentAsync("h1.text-center");
        Assert.Equal("Your Assigned Resources", heading);

        var noResourcesLocator = page.Locator("p:has-text('No resources assigned to you.')");

        if (await noResourcesLocator.IsVisibleAsync())
        {
            var msg = await page.TextContentAsync("p:has-text('No resources assigned to you.')");
            Assert.Contains("No resources assigned to you.", msg);
        }
        else
        {
            var tableVisible = await page.IsVisibleAsync("table");
            Assert.True(tableVisible);

            var rowsCount = await page.EvalOnSelectorAllAsync<int>("tbody tr", "els => els.length");
            Assert.True(rowsCount > 0);
        }
    }

    [Fact]
    public async Task Admin_Can_Create_New_Resource_Successfully()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        await page.GotoAsync($"{BaseUrl}/create-resource");

        await page.FillAsync("input[placeholder='Name'], input.form-control >> nth=0", "Test Resource");
        await page.FillAsync("input[placeholder='Description'], input.form-control >> nth=1", "Description for test resource");
        await page.FillAsync("input[type='email']", "user1@example.com");

        await page.ClickAsync("button:has-text('Submit')");

        await page.WaitForURLAsync($"{BaseUrl}/");

        Assert.Contains("/", page.Url);
    }

    [Fact]
    public async Task Create_Resource_Shows_Error_If_Fields_Empty()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        await page.GotoAsync($"{BaseUrl}/create-resource");

        await page.ClickAsync("button:has-text('Submit')");

        var errorMessages = await page.Locator(".alert.alert-danger li").AllTextContentsAsync();

        Assert.Contains("Name is Required!", errorMessages);
        Assert.Contains("Description is Required!", errorMessages);
        Assert.Contains("Email is Required!", errorMessages);
    }

    [Fact]
    public async Task Admin_Can_Edit_Resource_Successfully()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        string resourceId = "31265cd8-a632-4e53-832d-2c9691e4a971";

        await page.GotoAsync($"{BaseUrl}/edit-resource/Id={resourceId}");

        await page.WaitForSelectorAsync("input.form-control >> nth=0");

        await page.FillAsync("input.form-control >> nth=0", "Zmieniona nazwa");
        await page.FillAsync("input.form-control >> nth=1", "Zmieniony opis");
        await page.FillAsync("input[type='email']", "user1@example.com");

        await page.ClickAsync("button:has-text('Submit')");

        await page.WaitForURLAsync($"{BaseUrl}/");

        Assert.Contains("/", page.Url);
    }

    [Fact]
    public async Task EditResource_Shows_Errors_When_Fields_Are_Empty()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        string resourceId = "31265cd8-a632-4e53-832d-2c9691e4a971";

        await page.GotoAsync($"{BaseUrl}/edit-resource/Id={resourceId}");

        await page.WaitForSelectorAsync("input.form-control >> nth=0");

        await page.FillAsync("input.form-control >> nth=0", "");
        await page.FillAsync("input.form-control >> nth=1", "");
        await page.FillAsync("input[type='email']", "");

        await page.ClickAsync("button:has-text('Submit')");

        var errors = await page.Locator(".alert.alert-danger li").AllTextContentsAsync();

        Assert.Contains("Name is Required!", errors);
        Assert.Contains("Description is Required!", errors);
        Assert.Contains("Email is Required!", errors);
    }

    [Fact]
    public async Task Admin_DeletesAndRedirects_Successfully()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        string resourceId = "31265cd8-a632-4e53-832d-2c9691e4a971";
        await page.GotoAsync($"{BaseUrl}/delete-resource/Id={resourceId}");

        await page.ClickAsync("button:has-text('Delete')");
        await page.WaitForURLAsync($"{BaseUrl}/");

        var heading = await page.TextContentAsync("h1.text-center");
        Assert.Equal("List of All Resources", heading);
    }

}
