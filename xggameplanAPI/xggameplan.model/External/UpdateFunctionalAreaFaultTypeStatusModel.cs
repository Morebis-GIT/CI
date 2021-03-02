using System;
using System.ComponentModel.DataAnnotations;

namespace xggameplan.Model
{
    public class UpdateFunctionalAreaFaultTypeStatusModel
    {
        [Required(ErrorMessage = "FunctionalAreaId is required")]
        public Guid? FunctionalAreaId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "FaultTypeId is required")]
        public int? FaultTypeId { get; set; }

        [Required(ErrorMessage = "IsSelected is required")]
        public bool? IsSelected { get; set; }
    }
}
