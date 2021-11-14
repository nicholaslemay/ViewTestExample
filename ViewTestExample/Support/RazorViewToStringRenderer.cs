// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ViewTestExample.Support
{
    public class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }
        
        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, List<KeyValuePair<string, object>> viewDataItems = null)
        {
            viewDataItems ??= new List<KeyValuePair<string, object>>();
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName);

            return await RenderToString(view, model, viewDataItems, actionContext);
        }

        private async Task<string> RenderToString<TModel>(IView? view, TModel model, List<KeyValuePair<string, object>> viewDataItems, ActionContext? actionContext)
        {
            await using var output = new StringWriter();
            var viewDataDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model,
            };

            foreach (var item in viewDataItems)
            {
                viewDataDictionary.Add(item);
            }

            var viewContext = new ViewContext(actionContext,
                view,
                viewDataDictionary,
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);
            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            const bool isMainPage = true;
            var getViewResult = _viewEngine.GetView(null, viewName, isMainPage);
            
            if (getViewResult.Success)
                return getViewResult.View;

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage);
            if (findViewResult.Success)
                return findViewResult.View;

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            return new ActionContext(
                new DefaultHttpContext { RequestServices = _serviceProvider }, 
                RoutingHelper.GetRouteData(_serviceProvider), 
                new ActionDescriptor()
                );
        }
    }
}