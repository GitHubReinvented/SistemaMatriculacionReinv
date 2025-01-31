﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Application.Persistence;

namespace Infrasctructure.Services.Email
{
    public class TemplateService : ITemplateService
    {
        private readonly IRazorViewEngine  _razorViewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;

        public TemplateService(
            IRazorViewEngine razorViewEngine,
           IServiceProvider serviceProvider,
           ITempDataProvider tempDataProvider)
        {
            _razorViewEngine = razorViewEngine;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model) where T : class, new()
        {
            var httpContext = new DefaultHttpContext() { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (StringWriter sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    return string.Empty;
                }

                var metadataProvider = new EmptyModelMetadataProvider();
                var msDictionary = new ModelStateDictionary();
                var viewDataDictionary = new ViewDataDictionary(metadataProvider, msDictionary);

                viewDataDictionary.Model = model;

                var tempDictionary = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDataDictionary,
                    tempDictionary,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
