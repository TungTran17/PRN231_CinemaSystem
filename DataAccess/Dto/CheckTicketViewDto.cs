namespace DataAccess.Dto
{
    public class CheckTicketViewDto
    {
        public string UserName { get; set; }
        public string Film { get; set; }
        public string Room { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
    }
}
