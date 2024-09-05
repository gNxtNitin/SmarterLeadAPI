
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SmarterLead.API.Models.RequestModel
{
    public class SearchLeadRequest
    {
        public int? ClientLoginID { get; set; }
        public int? UserLimit { get; set; }
        public string? State {get;set;}
        public string? EntityType {get;set;}
        public string? Cargo {get;set;}
        public string? Classifications {get;set;}
        public string? CargoCarried { get; set; }
        public string? PowerUnitSt {get;set;}
        public string? PowerUnitEnd {get;set;}
        public string? DriverSt {get;set;}
        public string? DriverEnd {get;set;}
        public string? VehicleInsSt {get;set;}
        public string? VehicleInsEnd {get;set;}
        public string? DriveInsSt {get;set;}
        public string? DriveInsEnd {get;set;}
        public string? HazmatSt {get;set;}
        public string? HazmatEnd {get;set;}
        public string? OOsSt {get;set;}
        public string? OOsEnd { get;set;}
    }
}
