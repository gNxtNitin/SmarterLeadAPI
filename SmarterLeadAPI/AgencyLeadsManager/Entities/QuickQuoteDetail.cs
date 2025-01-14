using SmarterLead.API.AgencyLeadsManager.Entities;

namespace SmarterLead.API.AgencyLeadsManager.Entities
{
    public class QuickQuoteRoot
    {
        public QuickQuoteDetail QuickQuoteDetails { get; set; }
        public List<DriverDetail> Drivers { get; set; }
        public List<VehicleDetail> Vehicles { get; set; }
    }
    public class QuickQuoteDetail
    {
        public int CallId { get; set; }
        public int AgencyAssociateLeadId { get; set; }
        public string OwnerName { get; set; }
        public string EIN { get; set; }
        public string BusinessName { get; set; }
        public int BusinessYears { get; set; }
        public string BusinessType { get; set; }
        public string Violations { get; set; }
        public string CargoTypes { get; set; }
    }
    public class DriverDetail
    {
        public int QuickQuoteDetailId { get; set; }
        public string DriverName { get; set; }
        public string LicenseNumber { get; set; }
        public int CDLDuration { get; set; }
        public string LicenseState { get; set; }
    }

    public class VehicleDetail
    {
        public int QuickQuoteDetailId { get; set; }
        public string VIN { get; set; }
        public string MakeModel { get; set; }
        public int Value { get; set; }
        public string SafetyFeatures { get; set; }
        public string GaragingAddress { get; set; }
        public int GVW { get; set; }
    }
}

public class QQLead
{
    public string id { get; set; }
}

public class VehicleInformation
{
    public string VIN { get; set; }
    public string Make_Model { get; set; }
    public string Value { get; set; }
    public string Safety_Features { get; set; }
    public string Garaging_Address { get; set; }
    public string GVW { get; set; }
}

public class DriverInformation
{
    public string Driver_Name { get; set; }
    public string Driver_State { get; set; }
    public string Driver_License_Number { get; set; }
    public string CDL_Duration { get; set; }
}

public class BusinessData
{
    public string Name { get; set; }
    public string Business_Owner_Name { get; set; }
    public string EIN { get; set; }
    public string Business_Name { get; set; }
    public string Business_Years { get; set; }
    public string Business_Type { get; set; }
    public string Accidents_Violations { get; set; }
    public string Cargo_Types { get; set; }
    public List<VehicleInformation> Vehicles_Information { get; set; }
    public List<DriverInformation> Driver_Information { get; set; }
    public QQLead QQLead { get; set; }
}

public class QuickQuoteDetailRoot
{
    public List<BusinessData> data { get; set; }

    public QuickQuoteDetailRoot(QuickQuoteRoot root)
    {
        if (root != null && root.QuickQuoteDetails != null)
        {

            data = new List<BusinessData>();

            BusinessData businessData = new BusinessData();
            businessData.Business_Owner_Name = root.QuickQuoteDetails.OwnerName;
            businessData.Business_Name = root.QuickQuoteDetails.BusinessName;
            businessData.Business_Years = root.QuickQuoteDetails.BusinessYears.ToString();
            businessData.Business_Type = root.QuickQuoteDetails.BusinessType;
            businessData.Name = root.QuickQuoteDetails.BusinessName;
            businessData.EIN = root.QuickQuoteDetails.EIN;
            businessData.Accidents_Violations = root.QuickQuoteDetails.Violations;
            businessData.Cargo_Types = root.QuickQuoteDetails.CargoTypes;
            businessData.Vehicles_Information = new List<VehicleInformation>();
            businessData.Driver_Information = new List<DriverInformation>();

            if (root.Vehicles != null)
            {
                foreach (var vehicle in root.Vehicles)
                {
                    VehicleInformation vehicleInformation = new VehicleInformation();
                    vehicleInformation.VIN = vehicle.VIN;
                    vehicleInformation.Value = vehicle.Value.ToString();
                    vehicleInformation.Safety_Features = vehicle.SafetyFeatures;
                    vehicleInformation.Make_Model = vehicle.MakeModel;
                    vehicleInformation.Garaging_Address = vehicle.GaragingAddress;
                    vehicleInformation.GVW = vehicle.GVW.ToString();
                    businessData.Vehicles_Information.Add(vehicleInformation);
                }
            }


            if (root.Drivers != null)
            {
                foreach (var driver in root.Drivers)
                {
                    DriverInformation driverInformation = new DriverInformation();
                    driverInformation.Driver_Name = driver.DriverName;
                    driverInformation.CDL_Duration = driver.CDLDuration.ToString();
                    driverInformation.Driver_State = driver.LicenseState;
                    driverInformation.Driver_License_Number = driver.LicenseNumber;
                    businessData.Driver_Information.Add(driverInformation);
                }
            }

            data.Add(businessData);

        }
    }
}