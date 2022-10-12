namespace Google_authenticate.Models
{
    public class CreateMeetRoomViewModel
    {
        string? Summary {get; set;} 
        string? Description {get; set;} 
        int StartTime {get; set;} 
        int EndTime {get; set;} 
        string[]? Recurrence {get; set;} 
        string[]? AttendeesEmail {get; set;}
    }
}