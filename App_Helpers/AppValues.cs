using RentACar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentACar.App_Helpers
{
    public static class AppValues
    {
        public struct GoogleOAuth2
        {
            public const string NAME = "urn:google:name";
            public const string GIVENNAME = "urn:google:givenname";
            public const string SURNAME = "urn:google:surname";
            public const string EMAIL = "urn:google:email";
            public const string DATEOFBIRTH = "urn:google:dateofbirth";
            public const string ACCESSTOKEN = "urn:google:accesstoken";
            public const string USERDATA = "urn:google:userdata";            
        }
        
        public struct AppRoles
        {
            public const string USER = "User";
            public const string EMPLOYEE = "Employee";
            public const string SUPEREMPLOYEE = "SuperEmployee";
            public const string ADMIN = "Admin";

            public static string ExceptUser { get { return String.Format("{0}, {1}, {2}", AppValues.AppRoles.ADMIN, AppValues.AppRoles.EMPLOYEE, AppValues.AppRoles.SUPEREMPLOYEE); } }
        }

        public struct EntityStatus
        {
            public const string ACTIVE = "Active";
            public const string ARCHIVED = "Archived";
            public const string NEW = "New";
        }

        public struct Messages
        {
            public const string CREATEFAILED = "Unable to create new entity. Try again.";
            public const string EDITFAILED = "Unable to edit entity. Try again.";
            public const string DELETEFAILED = "Unable to edit delete entity. Try again.";
            public const string OPERATIONFALIED = "Unable to execute operation. Try again.";
        }

        public struct ImageViewModel
        {
            private string _imageName;
            private string _imageBase64Content;
            private string _imageType;

            public ImageViewModel(string imageBase64Content, string imageName, string imageType)
            {
                this._imageBase64Content = imageBase64Content;
                this._imageName = imageName;
                this._imageType = imageType;
            }

            public string ImageName { get { return _imageName; } set { _imageName = value; } }
            public string ImageBase64Content { get { return _imageBase64Content; } set { _imageBase64Content = value; } }
            public string ImageType { get { return _imageType; } set { _imageType = value; } }
        }

        public const string DISPLAYIMAGEBASE64 = "data:image;base64,";

        public struct PhotoParameters
        {
            public const string VEHICLE = "Vehicle";
            public const string PERSON = "Person";
            public const string LOCATION = "Location";
        }

        public const int MINMODELYEAR = 1900;
        public const string ALL = "All";

        public static string[] VehicleTypes = new string[] { "Caravan", "Sports", "Van", "Pickup", "Minivan", "SUV", "Mini", "Regular" };

        public struct PaymentTypes
        {
            public const string CASH = "Cash";
            public const string ERSTE = "Erste Card";
            public const string PBZ = "PBZ";
            public const string VISA = "Visa";
            public const string PAYPAL = "PayPal";
            public const string MAESTRO = "Maestro";
            public const string MASTERCARD = "Master Card";
            public const string AMERICAN = "American";
        }

        public struct AppColors
        {
            public const string Gray = "#D3D3D3";
            public const string Green = "#BDFCC9";
            public const string Red = "#FF7256";
            public const string Yellow = "#FFEC8B";
        }
    }
}
