using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyRestaurant.Models;

namespace MyRestaurant.TagHelpers
{
    [HtmlTargetElement("div",Attributes ="page-model")]
    public class PageLinkTagHelper:TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]

        public ViewContext ViewContext { get; set; }

        public PagingInfo PagingInfo { get; set; }
        public string  PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string  PageClass { get; set; }
        public string  PageClassNormal { get; set; }
        public string  PageClassSelected { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i < PagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                string url = PagingInfo.UrlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PagingInfo.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
