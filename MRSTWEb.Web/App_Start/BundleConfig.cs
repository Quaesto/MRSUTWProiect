using System.Web;
using System.Web.Optimization;

namespace MRSTWEb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").
                Include("~/Content/style.css"));


            bundles.Add(new Bundle("~/bundles/main/js").Include(
                "~/Scripts/script.js"));

        }
    }
}