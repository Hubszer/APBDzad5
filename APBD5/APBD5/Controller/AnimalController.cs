using System.Data;
using APBD5.Models;
using APBD5.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Tutorial.Controllers;


[ApiController]
//[Route("api/animals")]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string orderBy = "name")
    {
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        string orderByClause = string.Empty;
        
        switch (orderBy.ToLower())
        {
            case "name":
                orderByClause = "ORDER BY Name";
                break;
            case "description":
                orderByClause = "ORDER BY Description";
                break;
            case "category":
                orderByClause = "ORDER BY Category";
                break;
            case "area":
                orderByClause = "ORDER BY Area";
                break;
            default:
                orderByClause = "ORDER BY Name";
                break;
        }
        command.CommandText = $"SELECT * FROM Animal {orderByClause}";
        using (var reader = command.ExecuteReader())
        {
            List<Animal> animals = new List<Animal>();

            int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
            int nameOrdinal = reader.GetOrdinal("Name");
            int descriptionOrdinal = reader.GetOrdinal("Description");
            int categoryOrdinal = reader.GetOrdinal("Category");
            int areaOrdinal = reader.GetOrdinal("Area");
            while (reader.Read())
            {
                Animal animal = new Animal()
                {
                    IdAnimal = reader.GetInt32(idAnimalOrdinal),
                    Name = reader.GetString(nameOrdinal),
                    Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                    Category = reader.IsDBNull(categoryOrdinal) ? null : reader.GetString(categoryOrdinal),
                    Area = reader.IsDBNull(areaOrdinal) ? null : reader.GetString(areaOrdinal)
                };
                animals.Add(animal);
            }
            return Ok(animals);
        }
    }
    
    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal (Name, Description, Category, Area) VALUES (@animalName, @animalDescription, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDescription", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);
        command.ExecuteNonQuery();

        return Created("", null);
    }
    
    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);
        int rowsAffected = command.ExecuteNonQuery();
        
        if (rowsAffected == 0)
        {
            return NotFound($"Animal with ID {idAnimal} not found.");
        }
        return Ok($"Deleted animal with ID={idAnimal}");
    }
    
    [HttpPut("{animalName}")]
    public IActionResult UpdateAnimal(string animalName, Animal animal)
    {
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        SqlCommand command = new SqlCommand();
            
        command.Connection = connection;
        command.CommandText = "UPDATE Animal SET Name = @animalName, Description = @animalDescription, Category = @animalCategory, Area = @animalArea WHERE Name = @originalName";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.Parameters.AddWithValue("@animalDescription", animal.Description);
        command.Parameters.AddWithValue("@animalCategory", animal.Category);
        command.Parameters.AddWithValue("@animalArea", animal.Area);
        command.Parameters.AddWithValue("@originalName", animalName);
        int rowsAffected = command.ExecuteNonQuery();
                
        if (rowsAffected == 0)
        {
            return NotFound($"Animal with name '{animalName}' not found.");
        }
        return Ok($"Updated the animal with name={animalName}");
    }
}
