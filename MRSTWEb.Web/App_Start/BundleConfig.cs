﻿using System.Web;
using System.Web.Optimization;

namespace MRSTWEb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").
                         Include("~/Content/style.css"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                      "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.validate*"));

            bundles.Add(new Bundle("~/bundles/main/js").Include(
            "~/Scripts/script.js"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                     "~/Scripts/bootstrap.js"));

        }
    }
}