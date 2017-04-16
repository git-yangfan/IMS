using System.Web;
using System.Web.Optimization;

namespace IMS.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/bootstrap/js/jquery.js",
                        "~/Content/bootstrap/js/bootstrap.js",
                        "~/Content/bootstrap/table/bootstrap-table.js",
                        "~/Content/bootstrap/table/locale/bootstrap-table-zh-CN.js",
                        "~/Content/bootstrap/js/bootstrap-datetimepicker.js",
                       "~/Content/bootstrap/js/bootstrap-datetimepicker.zh-CN.js",
                       "~/Content/bootstrap/js/IMS.table.js"));

            bundles.Add(new ScriptBundle("~/Content/bootstrap/js/Table").Include(
                        "~/Content/bootstrap/table/bootstrap-table.js",
                        "~/Content/bootstrap/table/locale/bootstrap-table-zh-CN.js"
                        ));
            bundles.Add(new ScriptBundle("~/Content/bootstrap/js/DateTime").Include(
                       "~/Content/bootstrap/js/bootstrap-datetimepicker.js",
                       "~/Content/bootstrap/js/bootstrap-datetimepicker.zh-CN.js"
                       ));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap/css/style2.css",
                       //"~/Content/bootstrap/css/bootstrap.css",
                      "~/Content/bootstrap/table/bootstrap-table.css",
                      "~/Content/bootstrap/css/bootstrap-datetimepicker.min.css"
                     ));
            bundles.Add(new StyleBundle("~/Content/TableCSS").Include(
                     "~/Content/bootstrap/table/bootstrap-table.css"));
            bundles.Add(new StyleBundle("~/Content/DateTimeCSS").Include(
                    "~/Content/bootstrap/css/bootstrap-datetimepicker.min.css"));
        }
    }
}
