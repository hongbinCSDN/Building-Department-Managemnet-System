using System.Web;
using System.Web.Optimization;

namespace MWMS2
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/flatpickr.min.css",
                "~/Content/all.min.css",
                "~/Content/w3.css",
                "~/Content/datatables.min.css",
                "~/Content/css.css"
            ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/themes/base/jquery.ui.core.css",
                "~/Content/themes/base/jquery.ui.autocomplete.css",
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.12.1.js", "~/Scripts/jquery-ui-1.12.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery.min.js",
                "~/Scripts/base.js",
                "~/Scripts/searcher.js",
                "~/Scripts/tabber.js",
                "~/Scripts/popuper.js",
                "~/Scripts/demoData.js",
                "~/Scripts/all.min.js",
                "~/Scripts/pdfmake.min.js",
                "~/Scripts/vfs_fonts.js",
                "~/Scripts/datatables.min.js",
                "~/Scripts/flatpickr.js",
                "~/Scripts/GlobalFunction.js",
                "~/Scripts/CRMinputMask.js",
                "~/Scripts/jquery.multicolselect.js",
                "~/Scripts/atcp.js",
                "~/Scripts/handlebars.min-v4.1.2.js",
                "~/Scripts/setting.js"
            ));
        }
    }
}
