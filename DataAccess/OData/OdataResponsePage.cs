namespace DataAccess.OData
{
    public class OdataResponsePage<T>
    {
        public List<T> Value { get; set; }
        public int Count { get; set; }
    }
}
