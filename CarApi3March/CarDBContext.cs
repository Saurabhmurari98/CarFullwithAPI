using CarApi3March.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CarApi3March
{
    public class CarDBContext : DbContext
    {
        public CarDBContext(DbContextOptions<CarDBContext> options) : base(options) { }

        public DbSet<CarDetails> CarsDetails { get; set; }


        public async Task InsertCarDetailsAsync(string carMaker, string carModel, string carType, string cubicCapacity,
                                                int noOfWheels, int manufactureYear, int noOfPassengers,
                                                string description, byte[] carPic1, byte[] carPic2, byte[] carPic3,
                                                string fileName, string carPic1Path, string carPic2Path, string carPic3Path)
        {
            var parameters = new[]
            {
                new SqlParameter("@CarMaker", carMaker),
                new SqlParameter("@CarModel", carModel),
                new SqlParameter("@CarType", carType),
                new SqlParameter("@CubicCapacity", cubicCapacity),
                new SqlParameter("@NoOfWheels", noOfWheels),
                new SqlParameter("@ManufactureYear", manufactureYear),
                new SqlParameter("@NoOfPassengers", noOfPassengers),
                new SqlParameter("@Description", description),
                new SqlParameter("@CarPic1", carPic1),
                new SqlParameter("@CarPic2", carPic2),
                new SqlParameter("@CarPic3", carPic3),
                new SqlParameter("@FileName", fileName),
                new SqlParameter("@CarPic1Path", carPic1Path),
                new SqlParameter("@CarPic2Path", carPic2Path),
                new SqlParameter("@CarPic3Path", carPic3Path)
            };

            await Database.ExecuteSqlRawAsync("EXEC dbo.InsertCarDetails @CarMaker, @CarModel, @CarType, @CubicCapacity, @NoOfWheels, @ManufactureYear, @NoOfPassengers, @Description, @CarPic1, @CarPic2, @CarPic3, @FileName, @CarPic1Path, @CarPic2Path, @CarPic3Path", parameters);
        }


        public async Task UpdateCarDetailsAsync(int id, string carMaker, string carModel, string carType, string cubicCapacity,
                                                 int noOfWheels, int manufactureYear, int noOfPassengers,
                                                 string description, string fileName, byte[] carPic1, byte[] carPic2, byte[] carPic3,
                                                 string carPic1Path, string carPic2Path, string carPic3Path)
        {
            var parameters = new[]
            {
                new SqlParameter("@CarId", id),
                new SqlParameter("@CarMaker", carMaker),
                new SqlParameter("@CarModel", carModel),
                new SqlParameter("@CarType", carType),
                new SqlParameter("@CubicCapacity", cubicCapacity),
                new SqlParameter("@NoOfWheels", noOfWheels),
                new SqlParameter("@ManufactureYear", manufactureYear),
                new SqlParameter("@NoOfPassengers", noOfPassengers),
                new SqlParameter("@Description", description),
                new SqlParameter("@FileName", fileName),
                new SqlParameter("@CarPic1", carPic1),
                new SqlParameter("@CarPic2", carPic2),
                new SqlParameter("@CarPic3", carPic3),
                new SqlParameter("@CarPic1Path", carPic1Path),
                new SqlParameter("@CarPic2Path", carPic2Path),
                new SqlParameter("@CarPic3Path", carPic3Path)
            };

            await Database.ExecuteSqlRawAsync("EXEC dbo.UpdateCarDetails @CarId, @CarMaker, @CarModel, @CarType, @CubicCapacity, @NoOfWheels, @ManufactureYear, @NoOfPassengers, @Description, @FileName, @CarPic1, @CarPic2, @CarPic3, @CarPic1Path, @CarPic2Path, @CarPic3Path", parameters);
        }
        public async Task<CarDetails> GetCarDetailsByIdAsync(int id)
        {
            var parameter = new SqlParameter("@CarId", id);

            var carDetailsList = await this.CarsDetails
                .FromSqlRaw("EXEC dbo.GetCarDetailsById @CarId", parameter)
                .ToListAsync();  

            return carDetailsList.FirstOrDefault(); 
        }




        public async Task<List<CarDetails>> GetCarDetailsAll()
        {
            return await this.CarsDetails.FromSqlRaw("EXEC dbo.GetCarDetailsAll").ToListAsync();
        }
    }
}
