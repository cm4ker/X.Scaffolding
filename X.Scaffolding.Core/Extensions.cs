﻿using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace X.Scaffolding.Core
{
    /// <summary>
    /// Extension methoda for Html helper
    /// </summary>
    public static class Extensions
    {
        private static string GetWebApplicationUrl()
        {
            try
            {
                var request = HttpContext.Current.Request;
                var urlHelper = new UrlHelper(request.RequestContext);
                var result = String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority,
                    urlHelper.Content("~"));
                return result;
            }
            catch
            {
                return "/";
            }
        }

        //
        // Summary:
        //     Returns an CKEdiotr for element for each property in the object that is represented
        //     by the System.Linq.Expressions.Expression expression.
        //
        // Parameters:
        //   html:
        //     The HTML helper instance that this method extends.
        //
        //   expression:
        //     An expression that identifies the object that contains the properties to
        //     display.
        //
        // Type parameters:
        //   TModel:
        //     The type of the model.
        //
        //   TValue:
        //     The type of the value.
        //
        // Returns:
        //     An HTML input element for each property in the object that is represented
        //     by the expression.
        public static MvcHtmlString HtmlEditorFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var id = html.IdFor(expression).ToString();
            var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var sb = new StringBuilder();

            sb.AppendLine(html.TextAreaFor(expression).ToString());

            var ckeditorScriptLocation = String.Format("{0}Scripts/ckeditor/ckeditor.js", GetWebApplicationUrl());

            sb.AppendFormat(@"<script src=""{2}""></script>
                                <script>
                                    var editor = CKEDITOR.replace('{0}', {{
                                        language: '{1}',
                                        filebrowserUploadUrl: '/system/CKEditorFileUpload',
                                        toolbar : 'Full',
 
                                        toolbar_Full :
                                        [
                                            {{ name: 'document', items : [ 'Source','-','Save','NewPage','DocProps','Preview','Print','-','Templates' ] }},
                                            {{ name: 'clipboard', items : [ 'Cut','Copy','Paste','PasteText','PasteFromWord','-','Undo','Redo' ] }},
                                            {{ name: 'editing', items : [ 'Find','Replace','-','SelectAll','-','SpellChecker', 'Scayt' ] }},
                                            //{{ name: 'forms', items : [ 'Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton',  'HiddenField' ] }},
                                            '/',
                                            {{ name: 'basicstyles', items : [ 'Bold','Italic','Underline','Strike','Subscript','Superscript','-','RemoveFormat' ] }},
                                            {{ name: 'paragraph', items : [ 'NumberedList','BulletedList','-','Outdent','Indent','-','Blockquote','CreateDiv', '-','JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock','-','BidiLtr','BidiRtl' ] }},
                                            {{ name: 'links', items : [ 'Link','Unlink','Anchor' ] }},
                                            {{ name: 'insert', items : [ 'Image','Flash','Table','HorizontalRule','Smiley','SpecialChar','PageBreak','Iframe' ] }},
                                            '/',
                                            {{ name: 'styles', items : [ 'Styles','Format','Font','FontSize' ] }},
                                            {{ name: 'colors', items : [ 'TextColor','BGColor' ] }},
                                            {{ name: 'tools', items : [ 'Maximize', 'ShowBlocks','-','About' ] }}
                                        ],
                                        removeButtons: 'Save,NewPage,Templates,Flash',

                                    }});
    
                                    $('.save').click(function() {{
                                        var teaxtarea = $(""#{0}"");
                                        var html = CKEDITOR.instances.{0}.getData();
                                        teaxtarea.val(html);
                                    }});
    
                                </script>", id, lang, ckeditorScriptLocation);

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString DatePickerFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var text = html.TextBoxFor(expression, new { type = "datetime", @class = "droplist date form-control" }).ToString();
            return MvcHtmlString.Create(text);
        }

        public static MvcHtmlString FileUploadFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var id = html.IdFor(expression).ToString();
            var value = html.ValueFor(expression).ToString();

            var sb = new StringBuilder();

            var name = value.ToLower();

            //sb.AppendLine(FileUpload.GetHtml(id, 1, false, false, null, null).ToString());
            sb.AppendLine(GetFileUploadHtml(id, 1, false, false, null, null, value));

            var isImage = name.EndsWith("jpg") ||
                          name.EndsWith("jpeg") ||
                          name.EndsWith("png") ||
                          name.EndsWith("gif") ||
                          name.EndsWith("bmp");

            if (!String.IsNullOrEmpty(value) && isImage)
            {
                sb.AppendFormat(RenderThumbnail(value));
                //sb.AppendFormat("<img class=\"preview\" src=\"{0}\" />", value);
            }

            sb.AppendLine();

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ThumbnailFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var value = html.ValueFor(expression).ToString();
            return MvcHtmlString.Create(RenderThumbnail(value));
        }

        private static string RenderThumbnail(string value)
        {
            return String.Format("<a href=\"#\" class=\"thumbnail\"><img src=\"{0}\" /\"></a>", value);
        }

        public static MvcHtmlString TextEditorFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var text = html.TextBoxFor(expression, new { type = "text", @class = "form-control" }).ToString();
            return MvcHtmlString.Create(text);
        }

        public static MvcHtmlString MultilineTextEditorFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var text = html.TextAreaFor(expression, new { type = "text", @class = "form-control" }).ToString();
            return MvcHtmlString.Create(text);
        }

        public static MvcHtmlString EmailEditorFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<div class=\"input-group\">");
            sb.AppendLine("<span class=\"input-group-addon\">@</span>");
            sb.AppendLine(html.TextBoxFor(expression, new { type = "text", @class = "form-control" }).ToString());
            sb.AppendLine("</div>");

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString VideoPlayerEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int width = 0, int height = 315)
        {
            var str = html.ValueFor(expression).ToString();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(html.TextEditorFor(expression).ToHtmlString());
            
            if (!string.IsNullOrEmpty(str))
            {
                stringBuilder.AppendLine(VideoPlayerFor(html, expression, width, height).ToHtmlString());
            }

            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static MvcHtmlString VideoPlayerFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int width = 0, int height = 315)
        {
            var url = html.ValueFor(expression).ToHtmlString();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<a class=\"thumbnail\" href=\"#\">");
            stringBuilder.AppendLine(new VideoParser().GetPlayer(url, width, height));
            stringBuilder.AppendLine("</a>");
            return MvcHtmlString.Create(((object)stringBuilder).ToString());
        }

        /// <summary>
        /// Only for compatible, while WebHepers assembly not support MVC 5
        /// </summary>
        /// <param name="name"></param>
        /// <param name="initialNumberOfFiles"></param>
        /// <param name="allowMoreFilesToBeAdded"></param>
        /// <param name="includeFormTag"></param>
        /// <param name="addText"></param>
        /// <param name="uploadText"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetFileUploadHtml(string name = null, int initialNumberOfFiles = 1, bool allowMoreFilesToBeAdded = true, bool includeFormTag = true, string addText = null, string uploadText = null, string value = null)
        {
            var sb = new StringBuilder();

            if (String.IsNullOrEmpty(value))
            {
                sb.AppendFormat(@"<div class=""file-upload"" id=""{0}""><div><input name=""{0}"" type=""file"" /></div></div>", name);
            }
            else
            {
                sb.AppendFormat(@"<div class=""file-upload"" id=""{0}""><div><input name=""{0}"" type=""file"" value=""{1}"" /></div></div>", name, value);
                sb.AppendFormat(@"<input type=""hidden"" id=""{0}"" name=""{0}"" value=""{1}"" /> ", name, value);
            }

            return sb.ToString();

        }
    }
}