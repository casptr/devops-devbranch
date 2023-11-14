using FluentValidation;

namespace Foodtruck.Shared.Reservations;

public abstract class ReservationDto
{
    public class Index
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public StatusDto? Status { get; set; }
    }

    public class Create
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? Description { get; set; }
    }

    public class Detail
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Description { get; set; }
        public StatusDto? Status { get; set; }
    }

    public class Validator : FluentValidator<Create>
    {
        public Validator()
        {
            RuleFor(s => s.Start).NotEmpty().Must(s => s?.Date > DateTime.Now.Date);
            RuleFor(s => s.End).NotEmpty().Must(s => s?.Date > DateTime.Now.Date).Must((model, field) => field?.Date >= model.Start?.Date);
        }
    }
}
