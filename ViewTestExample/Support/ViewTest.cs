// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace ViewTestExample.Support
{
    public abstract class ViewTest
    {
        private const string ApplicationName = "ExampleWebApplication";
        
        protected static IHtmlDocument RenderView(string viewPath, object model,  List<KeyValuePair<string, object>> viewDataItems = null)
        {
            var result = RenderViewAsync(viewPath, model, viewDataItems).Result;
            return new HtmlParser().ParseDocument(result);
        }

        private static Task<string> RenderViewAsync(string viewPath, object model, List<KeyValuePair<string, object>> viewDataItems = null)
        {
            using var serviceScope = TestServiceScopeFactory.Build(ApplicationName).CreateScope();
            var helper = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            
            return helper.RenderViewToStringAsync(viewPath, model, viewDataItems);
        }
    }
}
