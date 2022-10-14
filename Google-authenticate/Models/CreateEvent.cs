using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Google_authenticate.Models
{
    public class CreateEvent
    {
        [Required]
        public string Summary {get; set;} 
        public string Description {get; set;} 
        [Required]
        public int StartTime {get; set;} 
        [Required]
        public int EndTime {get; set;} 
        public List<Recurrence>? Recurrence {get; set;} 
        [Required]
        public List<string>? AttendeesEmail {get; set;}
    }

    public class Recurrence {
        public string? freq {get; set;}
        public string? until {get; set;}
        public string? count {get; set;}
        public List<string>? byday {get; set;}

        public override string ToString()
        {
            string b = "RRULE:"; 

            if (freq != null) {
                b += "FREQ=" + freq;
            }
            if (count != null) {
                b += ";COUNT=" + count;
            }
            if (until != null) {
                b += ";UNTIL=" + until;
            }
            if (byday != null) {
                b += ";BYDAY=" + String.Join(',', byday);
            }

            return b;
        }
    }
}