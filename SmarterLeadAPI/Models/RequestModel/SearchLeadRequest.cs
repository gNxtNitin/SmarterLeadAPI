﻿
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SmarterLead.API.Models.RequestModel
{
    public class SearchLeadRequest
    {
        public int? ClientLoginID { get; set; }
        public int? UserLimit { get; set; }
        public List<string>? State { get; set; }

        public List<string>? EntityType { get; set; }
        public string? role { get; set; }
        public List<string>? Classifications { get; set; }
        public List<string>? CargoCarried { get; set; }
        public List<string>? InsuranceCarrier { get; set; }
        public List<string>? RadiusOfOperation { get; set; }
        public string? statetext { get; set; }
        public string? cargocarriedtext { get; set; }
        public string? classificationtext { get; set; }
        public string? entitytypetext { get; set; }
        public string? insurancecarriertext { get; set; }
        public string? radiusofoperationtext { get; set; }
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
        public string? CoverageSt { get; set; }
        public string? CoverageEnd { get; set; }
        public string? ExpirySt { get; set; }
        public string? ExpiryEnd { get; set; }
        public string? MVRSt { get; set; }
        public string? MVREnd { get; set; }
    }
}
