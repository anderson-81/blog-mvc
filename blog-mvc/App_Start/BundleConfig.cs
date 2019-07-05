using System.Web;
using System.Web.Optimization;

namespace blog_mvc
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/clean-blog.min.css",
                  "~/Content/font-awesome.min.css",
                  "~/Content/PagedList",
                  "~/Content/blog.css"));

            bundles.Add(new StyleBundle("~/Content/tether").Include(
                   "~/Content/tether/tether-theme-arrows-dark.min.css",
                   "~/Content/tether/tether-theme-arrows.min.css",
                   "~/Content/tether/tether-theme-basic.min.css",
                   "~/Content/tether/tether.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts").Include(
                      "~/Scripts/jquery-3.3.1.min.js",
                      "~/Scripts/tether.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/clean-blog.min.js",
                      "~/Scripts/scripts.js",
                      "~/Scripts/jquery.backDetect.min.js"));
        }
    }
}
