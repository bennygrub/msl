using System.Web;
using System.Web.Optimization;

namespace MySelfie
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/js/libs/jquery-2.0.2.min.js",
                        "~/Content/js/libs/jquery-ui-1.10.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/css/bootstrap.css",
                        "~/Content/css/font-awesome.css",
                        "~/Content/css/smartadmin-production.css",
                        "~/Content/css/smartadmin-skins.css",
                        "~/Content/css/demo.css",
                        "~/Content/css/datepicker.css",
                        "~/Content/css/myselfie-style.css"));

            bundles.Add(new StyleBundle("~/Content/css-public").Include(
                        "~/Content/css/bootstrap.css",
                        "~/Content/css/font-awesome.css",
                        "~/Content/css/smartadmin-production.css",
                        "~/Content/css/smartadmin-skins.css",
                        "~/Content/css/demo.css",
                        "~/Content/css/myselfie-style.css"));

            bundles.Add(new ScriptBundle("~/bundles/flot").Include(
                        "~/Content/js/plugin/flot/jquery.flot.cust.js",
                        "~/Content/js/plugin/flot/jquery.flot.resize.js",
                        "~/Content/js/plugin/flot/jquery.flot.tooltip.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/vectormap").Include(
                        "~/Content/js/plugin/vectormap/jquery-jvectormap-1.2.2.js",
                        "~/Content/js/plugin/vectormap/jquery-jvectormap-world-mill-en.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/calendar").Include(
                        "~/Content/js/plugin/fullcalendar/jquery.fullcalendar.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                        "~/Content/js/plugin/datatables/jquery.dataTables.js",
                        "~/Content/js/plugin/datatables/ColReorder.js",
                        "~/Content/js/plugin/datatables/FixedColumns.js",
                        "~/Content/js/plugin/datatables/ColVis.js",
                        "~/Content/js/plugin/datatables/ZeroClipboard.js",
                        "~/Content/js/plugin/datatables/media/js/TableTools.js",
                        "~/Content/js/plugin/datatables/DT_bootstrap.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/themejs").Include(
                        "~/Content/js/bootstrap/bootstrap.js",
                        "~/Content/js/notification/SmartNotification.js",
                        "~/Content/js/smartwidgets/jarvis.widget.js",
                        "~/Content/js/plugin/easy-pie-chart/jquery.easy-pie-chart.js",
                        "~/Content/js/plugin/sparkline/jquery.sparkline.js",
                        "~/Content/js/plugin/jquery-validate/jquery.validate.js",
                        "~/Content/js/plugin/masked-input/jquery.maskedinput.js",
                        "~/Content/js/plugin/select2/select2.js",
                        "~/Content/js/plugin/bootstrap-slider/bootstrap-slider.js",
                        "~/Content/js/plugin/colorpicker/bootstrap-colorpicker.min.js",
                        "~/Content/js/bootstrap/bootstrap-datepicker.js",
                        "~/Content/js/plugin/msie-fix/jquery.mb.browser.js",
                        "~/Content/js/plugin/fastclick/fastclick.js",
                        "~/Content/js/myselfie.js",
                        "~/Content/js/app.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/themejs-public").Include(
                        "~/Content/js/bootstrap/bootstrap.js",
                        "~/Content/js/notification/SmartNotification.js",
                        "~/Content/js/smartwidgets/jarvis.widget.js",
                        "~/Content/js/plugin/easy-pie-chart/jquery.easy-pie-chart.js",
                        "~/Content/js/plugin/sparkline/jquery.sparkline.js",
                        "~/Content/js/plugin/jquery-validate/jquery.validate.js",
                        "~/Content/js/plugin/masked-input/jquery.maskedinput.js",
                        "~/Content/js/plugin/select2/select2.js",
                        "~/Content/js/plugin/bootstrap-slider/bootstrap-slider.js",
                        "~/Content/js/bootstrap/bootstrap-datepicker.js",
                        "~/Content/js/plugin/msie-fix/jquery.mb.browser.js",
                        "~/Content/js/plugin/fastclick/fastclick.js",
                        "~/Content/js/app.js"
            ));
        }
    }
}
