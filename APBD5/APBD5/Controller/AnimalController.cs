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
    public IActionResult GetAnimals()
    {
        //Otwieramy polaczenie
        SqlConnection connection=new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        
        
        //Definiujemy commanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Animal";
        
        //Wykonanie commanda
        var reader = command.ExecuteReader();
        List<Animal> animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        
        while (reader.Read())
        {
                animals.Add(new Animal()
                {
                    //Name = reader.GetString(1)
                    IdAnimal = reader.GetInt32(idAnimalOrdinal),
                    Name = reader.GetString(nameOrdinal)
                    
                });
        }
        
        
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {

        /*using ()
        {
            
        }

        try
        {



        }

        finally
        {

            connection.Dispose();

        }*/
        //Otwieramy polaczenie
        SqlConnection connection=new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        
        
        //Definiujemy commanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName, '', '', '')";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        command.ExecuteNonQuery();
        
        
        return Created("", null);
    }
}