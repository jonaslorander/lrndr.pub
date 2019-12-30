using Microsoft.AspNetCore.Razor.TagHelpers;

namespace lrndrpub.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-disabled-if")]
    public class DisabledIfTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-disabled-if")]
        public bool Condition { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Condition)
            {
                // TODO: Check type of element here anc choose proper way to disable it
                output.Attributes.Add(new TagHelperAttribute("disabled"));
            }
        }
    }
}