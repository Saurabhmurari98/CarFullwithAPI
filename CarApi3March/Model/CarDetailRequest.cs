namespace CarApi3March.Model
{
    public class CarDetailRequest
    {
        public string CarMaker { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public string CubicCapacity { get; set; }
        public int NoOfWheels { get; set; }
        public int ManufactureYear { get; set; }
        public int NoOfPassengers { get; set; }
        public string Description { get; set; }
        public IFormFile CarPic1 { get; set; }
        public IFormFile CarPic2 { get; set; }
        public IFormFile CarPic3 { get; set; }
        public string FileName { get; set; }
    }
}
