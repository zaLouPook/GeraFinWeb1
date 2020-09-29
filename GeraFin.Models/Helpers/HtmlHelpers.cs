using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GeraFin
{
    public static class HtmlHelpers
    {
        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null, string style = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty; 
        }

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        public class ExtendedSelectListItem : SelectListItem
        {
            public IDictionary<string, object> HtmlAttributes { get; set; }
        }

        public static T Iff<T>(HtmlHelper html, bool condition, T trueValue, T falseValue)
        {
            return condition ? trueValue : falseValue;
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this ICollection<T> collection,
           Func<T, string> valueSelector,
           Func<T, string> textSelector, string initialValue = null)
        {
            if (collection == null)
                return new List<SelectListItem>();

            return collection.Select(c =>
            {
                var isSelected = (initialValue == valueSelector(c));
                return new SelectListItem()
                {
                    Text = textSelector(c),
                    Value = valueSelector(c),
                    Selected = isSelected
                };
            });
        }

        public static HtmlString MessageDialog(HtmlHelper htmlHelper, string message, bool isSuccess = true)
        {
            StringBuilder modalContentBuilder = new StringBuilder();
            modalContentBuilder.AppendFormat("<div class='modal fade in' id='{0}-modal' tabindex='-1' role='dialog' aria-labelledby='myModalLabel' aria-hidden='false' style='display: none;'>", isSuccess ? "success" : "error");
            modalContentBuilder.AppendLine("<div class='modal-dialog'>");
            modalContentBuilder.AppendLine("<div class='modal-content'>");
            modalContentBuilder.AppendLine("<div class='modal-body'>");
            modalContentBuilder.AppendFormat("<div id='message' class='{0}'>{1}</div>", isSuccess ? "successMsg" : "errorMsg", message);
            modalContentBuilder.AppendLine("</div>");
            modalContentBuilder.AppendLine("<div class='modal-footer'>");
            modalContentBuilder.AppendLine("<button type='button' class='' data-dismiss='modal'>Close</button>");
            modalContentBuilder.AppendLine("</div>");
            modalContentBuilder.AppendLine("</div>");
            modalContentBuilder.AppendLine("</div>");
            modalContentBuilder.AppendLine("</div>");

            return HtmlString(modalContentBuilder.ToString());
        }

        private static HtmlString HtmlString(string v)
        {
            throw new NotImplementedException();
        }
    }
}
