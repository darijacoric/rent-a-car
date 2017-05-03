using RentACar.DAT;
using RentACar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentACar.App_Helpers
{
    public static class AppMethods
    {
        public static List<string> GetModelStateErrorMsgs(ModelStateDictionary modelState)
        {
            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            var errorList = query.ToList();
            return errorList;
        }

        public static bool IsImage(this HttpPostedFileBase file)
        {
            if (file.ContentType.ToLower() != "image/jpg" &&
                file.ContentType.ToLower() != "image/jpeg" &&
                file.ContentType.ToLower() != "image/pjpeg" &&
                file.ContentType.ToLower() != "image/gif" &&
                file.ContentType.ToLower() != "image/x-png" &&
                file.ContentType.ToLower() != "image/png")
            {
                return false;
            }
            return true;
        }

        public static byte[] GetPicture(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength < 1)
            {
                return null;
            }

            if (file.IsImage() == false) { throw new InvalidDataException("Invalid image format!"); }

            byte[] data = new byte[file.ContentLength];

            file.InputStream.Read(data, 0, data.Length);

            string extension = Path.GetExtension(file.FileName).ToLower();

            return data;
        }

        public static int AddPhotoInDb(RcDbContext db, Photos photoEntity, bool saveChanges)
        {
            int status;

            if (photoEntity == null)
            {
                return -1;
            }

            db.Photos.Add(photoEntity);

            if (saveChanges == true)
            {
                status = db.SaveChanges();
            }
            else
            {
                status = 1;
            }
            return status;
        }

        
        // http://frontendplay.com/2014/12/03/view-to-string-in-aspnet-mvc/
        private static string GetHtmlFromPartialView(string viewName, object model, ControllerContext controllerContext, ViewDataDictionary viewData = null, TempDataDictionary tempData = null)
        {
            if (viewData == null)
            {
                viewData = new ViewDataDictionary();
            }

            if (tempData == null)
            {
                tempData = new TempDataDictionary();
            }

            // Assign model to the viewdata
            viewData.Model = model;

            // Implements a TextWriter for writing information to a string. The information is stored in an underlying StringBuilder.
            using (var sw = new StringWriter())
            {
                // Try to find specified view
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);

                // Create tha associated context
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);

                // Write rendered view with the given context to the stringwriter
                viewResult.View.Render(viewContext, sw);

                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        
        private static string GetHtmlFromView(string viewName, string masterViewName, object model, ControllerContext controllerContext, ViewDataDictionary viewData = null, TempDataDictionary tempData = null)
        {
            if (viewData == null)
            {
                viewData = new ViewDataDictionary();
            }

            if (tempData == null)
            {
                tempData = new TempDataDictionary();
            }

            // Assign model to the viewdata
            viewData.Model = model;

            // Implements a TextWriter for writing information to a string. The information is stored in an underlying StringBuilder.
            using (var sw = new StringWriter())
            {
                // Try to find specified view
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controllerContext, viewName, masterViewName);

                // Create tha associated context
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);

                // Write rendered view with the given context to the stringwriter
                viewResult.View.Render(viewContext, sw);

                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}