using System.Web;
using System.Web.Optimization;

namespace RentACar
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.12.1.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap*",
                      "~/Content/site.css",
                      "~/Content/custom.css",
                      "~/Content/Datatables/datatables.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                    "~/Scripts/Custom/Global/facebook-sdk.js",
                    "~/Scripts/Custom/Global/order-functions.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/modal").Include(
                    "~/Scripts/Custom/Modal/modal.js",
                    "~/Scripts/Custom/Modal/modal-submit.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                    "~/Scripts/moment.js",
                    "~/Scripts/moment-with-locales.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                    "~/Scripts/datepicker.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/fileprocess").Include(
                    "~/Scripts/Custom/Global/file-processing.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                   "~/Scripts/Custom/Datatables/datatables.min.js",
                   "~/Scripts/Custom/Datatables/dataTables-global.js"
               ));
        }
    }
}
