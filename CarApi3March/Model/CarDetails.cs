namespace CarApi3March.Model
{
    public class CarDetails
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
        public byte[] CarPic1 { get; set; }
        public byte[] CarPic2 { get; set; }
        public byte[] CarPic3 { get; set; }
        public string FileName { get; set; }
        public string CarPic1Path { get; set; }  
        public string CarPic2Path { get; set; }  
        public string CarPic3Path { get; set; }
    }
}
