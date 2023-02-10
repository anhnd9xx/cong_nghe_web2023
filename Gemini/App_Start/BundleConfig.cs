using System.Web.Optimization;

namespace Gemini
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                        "~/Content/kendo/kendo.common.min.css",
                        "~/Content/kendo/kendo.default.min.css"));

            bundles.IgnoreList.Clear();
            BundleTable.EnableOptimizations = true;
        }
    }
}