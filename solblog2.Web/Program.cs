using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SolBlog2.Web.Components;
using SolBlog2.Web.Components.Account;
using SolBlog2.Web.Models;
using SolBlog2.Domain.Models;
using SolBlog2.Infrastructure.Auth;
using SolBlog2.Infrastructure.Data;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddControllers();          // add this
builder.Services.AddEndpointsApiExplorer();

// Needed for reading current user inside minimal APIs
builder.Services.AddHttpContextAccessor();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // tune later
        options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
        // optional: password policy, lockout, etc.
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// dev-time seeding/migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.BlogPosts.AnyAsync())
    {
        var post = new BlogPost
        {
            AuthorId = "system",
            Title = "Welcome to SolBlog2",
            Abstract = "First seeded blog post.",
            Content = "This is seeded content for testing UI.",
            Slug = "welcome-to-solblog2",
            IsPublished = true
        };

        post.MarkCreated("system");

        db.BlogPosts.Add(post);
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers(); 
// static files before components
app.MapStaticAssets();


// Blazor components (once)
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();
