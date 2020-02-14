using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Helpers
{
    public static class ButtonHtmlHelpers
    {
        /// <summary>
        /// Tipi submit olan Save button oluşturur.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString SaveButton(this HtmlHelper htmlHelper) //extension için statik olması lazım ve HtmlHelper a extension yazıyoruz
        {
            TagBuilder tagBuilder = new TagBuilder("button");
            tagBuilder.InnerHtml = "Save";
            tagBuilder.MergeAttribute("type", "submit");
            tagBuilder.MergeAttribute("class", "btn btn-success");
            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper, string buttonText, string buttonClass, string buttonType="")
        {
            TagBuilder tagBuilder = new TagBuilder("button");
            tagBuilder.InnerHtml = buttonText;
            if(buttonType != "")
                 tagBuilder.MergeAttribute("type", buttonType);
            tagBuilder.MergeAttribute("class", buttonClass);
            return MvcHtmlString.Create(tagBuilder.ToString());

        }
    }
}