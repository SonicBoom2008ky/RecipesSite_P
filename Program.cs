using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;
using UUIDNext;
using static Project.Utils;

namespace Project;

class Program
{
  static void Main()
  {
    /*───────────────────────────╮
    │ Creating the server object │
    ╰───────────────────────────*/
    var server = new HttpListener();
    server.Prefixes.Add("http://*:5000/");
    server.Start();

    Console.WriteLine("Server started. Listening for requests...");
    Console.WriteLine("Main page on http://localhost:5000/website/index.html");


    /*─────────────────────────────────────╮
    │ Creating the database context object │
    ╰─────────────────────────────────────*/
    var database = new Database();

    /*─────────────────────────╮
    │ Processing HTTP requests │
    ╰─────────────────────────*/
    while (true)
    {
      /*────────────────────────────╮
      │ Waiting for an HTTP request │
      ╰────────────────────────────*/
      var serverContext = server.GetContext();
      var response = serverContext.Response;

      try
      {
        /*────────────────────────╮
        │ Handeling file requests │
        ╰────────────────────────*/
        serverContext.ServeFiles();

        /*───────────────────────────╮
        │ Handeling custome requests │
        ╰───────────────────────────*/
        HandleRequests(serverContext, database);

        /*───────────────────────────────╮
        │ Saving changes to the database │
        ╰───────────────────────────────*/
        database.SaveChanges();

      }
      catch (Exception e)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e);
        Console.ResetColor();
      }

      /*───────────────────────────────────╮
      │ Sending the response to the client │
      ╰───────────────────────────────────*/
      response.Close();
    }
  }

  static void HandleRequests(HttpListenerContext serverContext, Database databaseContext)
  {
    var request = serverContext.Request;
    var response = serverContext.Response;

    string absPath = request.Url!.AbsolutePath;

    if (absPath == "/signUp")
    {
      (string username, string password) = request.GetBody<(string, string)>();

      var userId = Uuid.NewDatabaseFriendly(UUIDNext.Database.SQLite).ToString();

      var user = new User(userId, username, password);
      databaseContext.Users.Add(user);

      response.Write(userId);
    }

    else if (absPath == "/autoLogIn")
    {
      string userId = request.GetBody<string>();

      User user = databaseContext.Users.Find(userId)!;

      response.Write(new { username = user.Username });
    }
    else if (absPath == "/logIn")
    {
      (string username, string password) = request.GetBody<(string, string)>();

      User user = databaseContext.Users.First(
        u => u.Username == username && u.Password == password
      )!;

      response.Write(user.Id);
    }

    else if (absPath == "/getUsername")
    {
      string userId = request.GetBody<string>();

      User user = databaseContext.Users.Find(userId)!;

      response.Write(user.Username);
    }

    else if (absPath == "/addrecipe")
    {
      (string title, string imageSource, string difficulty, string time, string Ingridients, string method) =
        request.GetBody<(string, string, string, string, string, string)>();

      Recipe recipe = new Recipe(title, imageSource, difficulty, time, Ingridients, method);

      databaseContext.Recipes.Add(recipe);
    }

    else if (absPath == "/getrecipe")
    {
      int recipeId = request.GetBody<int>();

      Recipe recipe = databaseContext.Recipes.Find(recipeId)!;

      var data = new
      {
        title = recipe.Title,
        imageSource = recipe.ImageSource,
        difficulty = recipe.Difficulty,
        time = recipe.Time,
        Ingridients = recipe.Ingridients,
        method = recipe.Method,
      };

      response.Write(data);
    }

    else if (absPath == "/getPreviews")
    {
      var previews = databaseContext.Recipes.Select(
        recipe => new
        {
          id = recipe.Id,
          title = recipe.Title,
          imageSource = recipe.ImageSource,
          difficulty = recipe.Difficulty,
          time = recipe.Time,
        }
      ).ToArray();

      response.Write(previews);
    }

    else if (absPath == "/getIsFavorite")
    {
      (string userId, int recipeId) = request.GetBody<(string, int)>();

      bool isFavorite = databaseContext.Favorites.Any(
        f => f.UserId == userId && f.RecipeId == recipeId
      );

      response.Write(isFavorite);
    }

    else if (absPath == "/addToFavorites")
    {
      (string userId, int recipeId) = request.GetBody<(string, int)>();

      Favorite userFavorite = new Favorite(userId, recipeId);

      databaseContext.Favorites.Add(userFavorite);
    }

    else if (absPath == "/removeFromFavorites")
    {
      (string userId, int recipeId) = request.GetBody<(string, int)>();

      Favorite favorite = databaseContext.Favorites.First(
        f => f.UserId == userId && f.RecipeId == recipeId
      );

      databaseContext.Favorites.Remove(favorite);
    }
  }
}

class Database : DbContextWrapper
{
  public DbSet<User> Users { get; set; }
  public DbSet<Recipe> Recipes { get; set; }
  public DbSet<Favorite> Favorites { get; set; }

  public Database() : base("Database") { }
}

class User(string id, string username, string password)
{
  [Key]
  public string Id { get; set; } = id;
  public string Username { get; set; } = username;
  public string Password { get; set; } = password;
  public int[] Favorites = [];
}

class Recipe(string title, string imageSource, string Ingridients, string method, string difficulty, string time)
{
  [Key]
  public int Id { get; set; }
  public string Title { get; set; } = title;
  public string ImageSource { get; set; } = imageSource;
  public string Difficulty { get; set; } = difficulty;
  public string Time { get; set; } = time;
  public string Ingridients { get; set; } = Ingridients;
  public string Method { get; set; } = method;
}

class Favorite(string userId, int recipeId)
{
  [Key]
  public int Id { get; set; }

  public string UserId { get; set; } = userId;
  public int RecipeId { get; set; } = recipeId;

  [ForeignKey("UserId")]
  public User? User { get; set; }

  [ForeignKey("RecipeId")]
  public Recipe? Recipe { get; set; }
}