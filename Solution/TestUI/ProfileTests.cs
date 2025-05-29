using Microsoft.Playwright;

public class ProfileTests
{
    private const string BaseUrl = "https://localhost:7284";

    [Fact]
    public async Task ProfilePage_ShowsUserInfo_And_CanNavigateToEdit()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        await page.GotoAsync($"{BaseUrl}/profile");

        await page.WaitForSelectorAsync("p:has-text('Id:')");

        string email = await page.InnerTextAsync("p:has-text('Email:')");
        string username = await page.InnerTextAsync("p:has-text('User Name:')");
        Assert.False(string.IsNullOrEmpty(email));
        Assert.False(string.IsNullOrEmpty(username));

        await page.ClickAsync("button:has-text('Edit')");

        await page.WaitForURLAsync("**/edit-profile");

        Assert.Contains("/edit-profile", page.Url);
    }

    [Fact]
    public async Task User_Can_Edit_Profile_Successfully()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");

        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        await page.GotoAsync($"{BaseUrl}/edit-profile");

        await page.FillAsync("input >> nth=0", "NowaNazwa");
        await page.ClickAsync("button:has-text('Submit')");

        await page.WaitForURLAsync("**/profile");

        Assert.Contains("/profile", page.Url);
    }


    [Fact]
    public async Task User_Cannot_Edit_Profile_Successfully()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");

        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        await page.GotoAsync($"{BaseUrl}/edit-profile");

        await page.FillAsync("input >> nth=0", "");
        await page.ClickAsync("button:has-text('Submit')");

        await page.WaitForURLAsync("**/profile");

        Assert.Contains("/profile", page.Url);
    }

    [Fact]
    public async Task DeleteProfile_DeletesAccount_AndNavigatesHome()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync($"{BaseUrl}/login");
        await page.WaitForURLAsync(url => !url.ToString().EndsWith("/login"));

        await page.GotoAsync($"{BaseUrl}/delete-profile");

        page.Dialog += async (_, dialog) =>
        {
            Assert.NotNull(dialog.Message);
            await dialog.AcceptAsync();
        };

        await page.ClickAsync("button:has-text('Delete')");
        await page.WaitForURLAsync($"{BaseUrl}/");

        Assert.Equal($"{BaseUrl}/", page.Url);
    }
}
