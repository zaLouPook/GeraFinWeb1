namespace GeraFin.Models.ViewModels.Syncfusion
{
    public class Crud<T> where T : class
    {
        public string action { get; set; }
        public object key { get; set; }
        public string antiForgery { get; set; }
        public T value { get; set; }
    }
}
