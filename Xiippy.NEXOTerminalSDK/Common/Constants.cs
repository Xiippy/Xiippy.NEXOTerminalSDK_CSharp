// *******************************************************************************************
// Copyright © 2019 Xiippy.ai. All rights reserved. Australian patents awarded. PCT patent pending.
//
// NOTES:
//
// - No payment gateway SDK function is consumed directly. Interfaces are defined out of such interactions and then the interface is implemented for payment gateways. Design the interface with the most common members and data structures between different gateways. 
// - A proper factory or provider must instantiate an instance of the interface that is interacted with.
// - Any major change made to SDKs should begin with the c sharp SDK with the mindset to keep the high-level syntax, structures and class names the same to minimise porting efforts to other languages. Do not use language specific features that do not exist in other languages. We are not in the business of doing the same thing from scratch multiple times in different forms.
// - Pascal Case for naming conventions should be used for all languages
// - No secret or passwords or keys must exist in the code when checked in
//
// *******************************************************************************************

namespace Xiippy.NEXOTerminalSDK.Common
{
    public static class FCEConstants
    {
        public const string StatementItemIdentifier_BookingFee = "Booking Fees";
        public const string StatementItemIdentifier_CPVLevyRecoveryFees = "CPV Levy Recovery Fee";
        public const string StatementItemIdentifier_HighOccupancyFee = "High Occupancy Fee";
        public const string StatementItemIdentifier_LiftingFee = "Lifting Fee";
        public const string StatementItemIdentifier_LiftingFeePaymentToDriver = "Lifting Fee Payment to Driver";
        public const string StatementItemIdentifier_CleaningFees = "Cleaning Fees";
        public const string StatementItemIdentifier_HolidayFee = "Holiday Fee";
        public const string StatementItemIdentifier_WheelchairAccessibleTaxiWATfee = "Wheelchair Accessible Taxi (WAT) fee";
        public const string TaxiSubsidyClaimStatementItemID = "Taxi Subsidy Scheme Discount";
        public const string TaxiSubsidyClaimStatementItemDescription = "Taxi Subsidy Scheme Discount";
        public const string StatementItemIdentifier_PassengerServiceLevyFees = "Passenger Service Levy Fees";
        public const string StatementItemIdentifier_PeakTimeHireChargeFees = "Peak Time Hire Charge";
    }


    public static class TariffCostCategories
    {
        public const string Fares = "Fares";
        public const string DistanceCosts = "Distance";
        public const string TimeCosts = "Time";
        public const string AirportCosts = "Airport";
        public const string ExtraCosts = "Extras";
        public const string TollsCosts = "Tolls";
        public const string Surcharge = "Surcharges";
        public const string Others = "Others";
        public const string Lifting = "Lifting";
        public const string LiftingDriverPayouts = "LiftingDriverPayout";
        public const string TSSDiscounts = "TSSDiscounts";
    }
}
