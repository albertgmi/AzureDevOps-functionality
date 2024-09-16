using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyBoards.Entities;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Po³¹czenie do bazy danych

builder.Services.AddDbContext<MyBoardsContext>(
    option=>option
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
    );

// Usuwanie "pêtli" podczas serializacji do JSON w aplikacjach MinimalAPI

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();
var pendingMigrations = dbContext.Database.GetPendingMigrations();

if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();


if (users.Count==0)
{
    var user1 = new User()
    {
        Email = "monika.wakula@vp.pl",
        FullName = "Monika Waku³a",
        Address = new Address()
        {
            City = "Siedlce",
            Street = "Sulimów"
        }
    };
    var user2 = new User()
    {
        Email = "mojbercik.kc@kc.kc",
        FullName = "Bercik Gmitrzaczek",
        Address = new Address()
        {
            City = "Warszawa",
            Street = "Ksawerów"
        }
    };
    dbContext.Users.Add(user1);
    dbContext.Users.Add(user2);

    dbContext.SaveChanges();
}

app.MapGet("data", async (MyBoardsContext mb) =>
{
    var epics = await mb.Epics
    .Where(e => e.StateId == 4)
    .OrderByDescending(e => e.Priority)
    .ToListAsync();
    return epics;
});

app.MapGet("userComments", async (MyBoardsContext mb) =>
{
    var user = await mb.Comments
    .GroupBy(a => a.AuthorId)
    .Select(x => new { AuthorId = x.Key, Count = x.Count() })
    .OrderByDescending(x => x.Count)
    .FirstAsync();

    var topUser = await mb.Users
    .FirstAsync(u => u.Id == user.AuthorId);

    return new { topUser, user.Count };
});

app.MapGet("lazyLoading", async (MyBoardsContext mb) =>
{
    var withAddress = true;

    var user = mb.Users
    .First(u => u.Id == Guid.Parse("EBFBD70D-AC83-4D08-CBC6-08DA10AB0E61"));

    if (withAddress)
    {
        var result = new {FullName = user.FullName, Adres = $"{user.Address.Street}, {user.Address.City}"};
        return result;
    }
    return new { user.FullName, Adres = "-" };
});

app.MapPost("update", async (MyBoardsContext mb) =>
{
    Epic epik = await mb.Epics
                        .FirstAsync(e=>e.Id==1);
    epik.StateId = 1;

    await mb.SaveChangesAsync();
    return epik;
});
app.MapPost("create", async (MyBoardsContext mb) =>
{
    Address adres = new Address()
    {
        Id = Guid.NewGuid(),
        City = "Siedlce",
        Country = "Poland",
        Street = "Sulimów 22/52"
    };
    User user = new User()
    {
        FullName = "Monika Monik",
        Email = "monisiunia@wp.pl",
        Address = adres
    };
    mb.Users.Add(user);
    await mb.SaveChangesAsync();
    return user;
});

app.MapDelete("delete", async (MyBoardsContext mb) =>
{
    var user = await mb.Users
    .Include(u=>u.Comments)
    .FirstAsync(u => u.Id == Guid.Parse("4EBB526D-2196-41E1-CBDA-08DA10AB0E61"));

    mb.Users.Remove(user);
    await mb.SaveChangesAsync();
});

app.Run();
