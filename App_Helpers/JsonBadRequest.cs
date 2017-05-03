using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentACar.App_Helpers
{
    // JsonResult always returns HttpStatusCode 200 event when there are errors.
    // This class is used to send back StatusCode 400 (error) with additional data
    public class JsonBadRequest : JsonResult
    {
        public JsonBadRequest() { }

        public JsonBadRequest(string message)
        {
            this.Data = message;
        }

        public JsonBadRequest(Object data)
        {
            this.Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.RequestContext.HttpContext.Response.StatusCode = 400; // set status code
            base.ExecuteResult(context);
        }
    }
}