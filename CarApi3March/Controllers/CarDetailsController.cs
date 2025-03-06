using CarApi3March.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;      
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.IO.Image;

namespace CarApi3March.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarDetailsController : ControllerBase
    {
        private readonly CarDBContext _context;
        public CarDetailsController(CarDBContext context)
        {
            _context = context;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadCarDetails([FromForm] CarDetailRequest request)
        {
            if (request.CarPic1 == null || request.CarPic2 == null || request.CarPic3 == null)
            {
                return BadRequest("All images are required.");
            }

            byte[] carPic1Data = await ConvertFileToByteArray(request.CarPic1);
            byte[] carPic2Data = await ConvertFileToByteArray(request.CarPic2);
            byte[] carPic3Data = await ConvertFileToByteArray(request.CarPic3);

            string carPic1Path = await SaveImage(request.CarPic1);
            string carPic2Path = await SaveImage(request.CarPic2);
            string carPic3Path = await SaveImage(request.CarPic3);

            await _context.InsertCarDetailsAsync(request.CarMaker, request.CarModel, request.CarType, request.CubicCapacity,
                                                 request.NoOfWheels, request.ManufactureYear, request.NoOfPassengers, request.Description,
                                                 carPic1Data, carPic2Data, carPic3Data, request.FileName, carPic1Path, carPic2Path, carPic3Path);

            return Ok(new { Message = "Car details uploaded successfully!" });
        }
        private async Task<byte[]> ConvertFileToByteArray(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCarDetails()
        {
            var carDetails = await _context.GetCarDetailsAll();

            if (carDetails == null || !carDetails.Any())
            {
                return NotFound("No car details found.");
            }

            var carDetailResponses = carDetails.Select(car => new CarDetailResponse
            {
                Id = car.Id,
                CarMaker = car.CarMaker,
                CarModel = car.CarModel,
                CarType = car.CarType,
                CubicCapacity = car.CubicCapacity,
                NoOfWheels = car.NoOfWheels,
                ManufactureYear = car.ManufactureYear,
                NoOfPassengers = car.NoOfPassengers,
                Description = car.Description,
                CarPic1Base64 = car.CarPic1Path,  
                CarPic2Base64 = car.CarPic2Path,
                CarPic3Base64 = car.CarPic3Path,
                FileName = car.FileName
            }).ToList();

            return Ok(carDetailResponses);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetailsById(int id)
        {
            var carDetail = await _context.GetCarDetailsByIdAsync(id); 
            if (carDetail == null)
            {
                return NotFound("Car details not found.");
            }

            var carDetailResponse = new CarDetailResponse
            {
                Id = carDetail.Id,
                CarMaker = carDetail.CarMaker,
                CarModel = carDetail.CarModel,
                CarType = carDetail.CarType,
                CubicCapacity = carDetail.CubicCapacity,
                NoOfWheels = carDetail.NoOfWheels,
                ManufactureYear = carDetail.ManufactureYear,
                NoOfPassengers = carDetail.NoOfPassengers,
                Description = carDetail.Description,
                CarPic1Base64 = carDetail.CarPic1Path,  
                CarPic2Base64 = carDetail.CarPic2Path,  
                CarPic3Base64 = carDetail.CarPic3Path,  
                FileName = carDetail.FileName
            };

            return Ok(carDetailResponse);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCarDetails(int id, [FromForm] CarDetailRequest request)
        {
            var carDetail = await _context.CarsDetails.FindAsync(id);
            if (carDetail == null)
            {
                return NotFound("Car details not found.");
            }

            var carPic1Data = request.CarPic1 != null ? await ConvertFileToByteArray(request.CarPic1) : carDetail.CarPic1;
            var carPic2Data = request.CarPic2 != null ? await ConvertFileToByteArray(request.CarPic2) : carDetail.CarPic2;
            var carPic3Data = request.CarPic3 != null ? await ConvertFileToByteArray(request.CarPic3) : carDetail.CarPic3;

            var carPic1Path = request.CarPic1 != null ? await SaveImage(request.CarPic1) : carDetail.CarPic1Path;
            var carPic2Path = request.CarPic2 != null ? await SaveImage(request.CarPic2) : carDetail.CarPic2Path;
            var carPic3Path = request.CarPic3 != null ? await SaveImage(request.CarPic3) : carDetail.CarPic3Path;

            await _context.UpdateCarDetailsAsync(
                id,
                request.CarMaker ?? carDetail.CarMaker,
                request.CarModel ?? carDetail.CarModel,
                request.CarType ?? carDetail.CarType,
                request.CubicCapacity ?? carDetail.CubicCapacity,
                request.NoOfWheels,
                request.ManufactureYear,
                request.NoOfPassengers,
                request.Description ?? carDetail.Description,
                request.FileName ?? carDetail.FileName,
                carPic1Data,
                carPic2Data,
                carPic3Data,
                carPic1Path,
                carPic2Path,
                carPic3Path
            );

            return Ok(new { Message = "Car details updated successfully!" });
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            try
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return "/images/" + fileName;
            }

            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the image.", ex);

            }
        }


        [HttpGet("download")]
        public async Task<IActionResult> DownloadAllCarDetails()
        {
            var carDetails = await _context.GetCarDetailsAll();

            if (carDetails == null || !carDetails.Any())
            {
                return NotFound("No car details found.");
            }

            using (var memoryStream = new MemoryStream())
            {
                // Create a PDF document
                using (var pdfWriter = new PdfWriter(memoryStream))
                {
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        var document = new Document(pdfDocument);

                        // Add a title
                        document.Add(new Paragraph("Car Details")
                            .SetFontSize(20)
                            .SetBold()
                            .SetTextAlignment(TextAlignment.CENTER));

                        // Base URL for images
                        // Base URL for images
                        string baseUrl = "https://localhost:44300";

                        foreach (var car in carDetails)
                        {
                            // Add car details
                            document.Add(new Paragraph($"Car Maker: {car.CarMaker}"));
                            document.Add(new Paragraph($"Car Model: {car.CarModel}"));
                            document.Add(new Paragraph($"Car Type: {car.CarType}"));
                            document.Add(new Paragraph($"Cubic Capacity: {car.CubicCapacity}"));
                            document.Add(new Paragraph($"No Of Wheels: {car.NoOfWheels}"));
                            document.Add(new Paragraph($"Manufacture Year: {car.ManufactureYear}"));
                            document.Add(new Paragraph($"No Of Passengers: {car.NoOfPassengers}"));
                            document.Add(new Paragraph($"Description: {car.Description}"));
                            document.Add(new Paragraph($"File Name: {car.FileName}"));

                            // Add images
                            if (!string.IsNullOrEmpty(car.CarPic1Path))
                            {
                                var img1Url = $"{baseUrl}{car.CarPic1Path}"; // Ensure no leading slash in CarPic1Path
                                var img1 = ImageDataFactory.Create(img1Url);
                                document.Add(new Image(img1).SetWidth(200).SetHeight(150));
                            }

                            if (!string.IsNullOrEmpty(car.CarPic2Path))
                            {
                                var img2Url = $"{baseUrl}{car.CarPic2Path}"; // Ensure no leading slash in CarPic2Path
                                var img2 = ImageDataFactory.Create(img2Url);
                                document.Add(new Image(img2).SetWidth(200).SetHeight(150));
                            }

                            if (!string.IsNullOrEmpty(car.CarPic3Path))
                            {
                                var img3Url = $"{baseUrl}{car.CarPic3Path}"; // Ensure no leading slash in CarPic3Path
                                var img3 = ImageDataFactory.Create(img3Url);
                                document.Add(new Image(img3).SetWidth(200).SetHeight(150));
                            }

                            document.Add(new Paragraph("\n")); // Add some space between cars
                        }
                        document.Close();
                    }
                }

                // Return the PDF file
                var pdfFileName = $"all.pdf";
                return File(memoryStream.ToArray(), "application/pdf", pdfFileName);
            }
        }
    }
}
