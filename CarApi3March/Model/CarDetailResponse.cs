namespace CarApi3March.Model
{
    public class CarDetailResponse
    {
        public int Id { get; set; }
        public string CarMaker { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public string CubicCapacity { get; set; }
        public int NoOfWheels { get; set; }
        public int ManufactureYear { get; set; }
        public int NoOfPassengers { get; set; }
        public string Description { get; set; }
        public string CarPic1Base64 { get; set; }
        public string CarPic2Base64 { get; set; }
        public string CarPic3Base64 { get; set; }
        public string FileName { get; set; }
    }
}
