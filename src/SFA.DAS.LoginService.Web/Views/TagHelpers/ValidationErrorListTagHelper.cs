using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.LoginService.Web.Views.TagHelpers
{
    [HtmlTargetElement("sfa-validation-error-list")] 
    public class ValidationErrorListTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var validationOrder = ViewContext.ViewBag.ValidationOrder != null ? ViewContext.ViewBag.ValidationOrder.Split(',') : new string[] { };
            output.TagName = null;
            foreach (var modelState in ViewContext.ModelState.OrderBy(x => Array.IndexOf(validationOrder, x.Key)))
            {
                foreach (var error in modelState.Value.Errors.Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage)).Take(1))
                {
                    var liTagBuilder = new TagBuilder("li");
                    var aTagBuilder = new TagBuilder("a");
                    aTagBuilder.Attributes.Add("href","#"+modelState.Key);
                    aTagBuilder.InnerHtml.Append(error.ErrorMessage);

                    liTagBuilder.InnerHtml.AppendHtml(aTagBuilder);
                    
                    output.Content.AppendHtml(liTagBuilder);
                }
            }
        }
    }
}