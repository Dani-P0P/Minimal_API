using Minimal_API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<Person>> GetAllPersons(DataContext context) => 
    await context.Persons.ToListAsync();

app.MapGet("/",()=>"Welcome to my Minimal API! <3");

app.MapGet("/persons", async (DataContext context)=> await context.Persons.ToListAsync());

app.MapGet("/person", async (DataContext context, int id) =>
await context.Persons.FindAsync(id) is Person person ?
Results.Ok(person) :
Results.NotFound("Person not found."));

app.MapPost("/person", async (DataContext context, Person person) =>
{
    context.Persons.Add(person);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllPersons(context));
});

app.MapPut("/person/{id}", async (DataContext context, Person person, int id) =>
{
    var dbPerson = await context.Persons.FindAsync(id);
    if (dbPerson == null)
        return Results.NotFound("Person not found");
    dbPerson.FirstName = person.FirstName;
    dbPerson.LastName = person.LastName;
    dbPerson.Age = person.Age;
    await context.SaveChangesAsync();

    return Results.Ok(await GetAllPersons(context));
});

app.MapDelete("/person/{id}", async (DataContext context, int id) =>
{
    var dbPerson = await context.Persons.FindAsync(id);
    if (dbPerson == null)
        return Results.NotFound("Person not found");

    context.Persons.Remove(dbPerson);
    await context.SaveChangesAsync();

    return Results.Ok(await GetAllPersons(context));
});

app.Run();