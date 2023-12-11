using GameStore.Server.Data;
using GameStore.Server.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(option => option.AddDefaultPolicy(builder => {
    builder.WithOrigins("http://localhost:5114")
           .AllowAnyHeader()
           .AllowAnyMethod();
}));

var connString = builder.Configuration.GetConnectionString("GameStoreContext");
builder.Services.AddSqlServer<GameStoreContext>(connString);

var app = builder.Build();

app.UseCors();

var group = app.MapGroup("/games")
               .WithParameterValidation();
               

//GET /games
group.MapGet("/", async (string? filter, GameStoreContext context) =>  {
   var games = context.Games.AsNoTracking();

   if(filter is not null){
    games = games.Where(game => game.Name.Contains(filter)||game.Genre.Contains(filter));
   }

   return await games.ToListAsync();

});

//GET games by Id
group.MapGet("/{id}", async (int id, GameStoreContext context) =>{ 
   Game? game = await context.Games.FindAsync(id);

   if(game is null){
    return Results.NotFound();
   }
    return Results.Ok(game);
}).WithName("GetGame");

group.MapPost("/", async (Game game, GameStoreContext context)=>{
    context.Games.Add(game);
    await context.SaveChangesAsync();

    return Results.CreatedAtRoute("GetGame", new {id = game.Id}, game);
}).WithParameterValidation();

group.MapPut("/{id}", async (int id, Game updateGame, GameStoreContext context) => {
    var rowsAffected = await context.Games.Where(game => game.Id == id)
                .ExecuteUpdateAsync(updates => 
                            updates.SetProperty(game => game.Name, updateGame.Name)
                                    .SetProperty(game => game.Genre, updateGame.Genre)
                                    .SetProperty(game => game.Price, updateGame.Price)
                                    .SetProperty(game => game.RelaseDate, updateGame.RelaseDate));

    return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
});

group.MapDelete("/{id}", async (int id, GameStoreContext context) =>{
    var rowsAffected = await context.Games.Where(game => game.Id == id)
                            .ExecuteDeleteAsync();

    return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
});

app.Run();
