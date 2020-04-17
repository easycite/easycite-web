using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EasyCite.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "asp-nav")]
    public class ActiveNavTagHelper : TagHelper
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelper _urlHelper;

        public ActiveNavTagHelper(IActionContextAccessor actionContextAccessor, IUrlHelper urlHelper)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelper = urlHelper;
        }

        [HtmlAttributeName("asp-active-class")]
        public string ActiveClass { get; set; } = "active";
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Remove attribute
            output.Attributes.Remove(output.Attributes["asp-nav"]);

            // Compare href and current route
            var href = output.Attributes["href"].Value.ToString();
            var currentRoute = _urlHelper.Action();

            if(href == currentRoute)
                output.AddClass(ActiveClass, HtmlEncoder.Default);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output) => base.ProcessAsync(context, output);
    }
}