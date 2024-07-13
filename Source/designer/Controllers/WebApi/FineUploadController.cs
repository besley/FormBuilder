/*
* Slickflow 工作流引擎遵循LGPL协议，也可联系作者商业授权并获取技术支持；
* 除此之外的使用则视为不正当使用，请您务必避免由此带来的商业版权纠纷。
*  
The Slickflow project.
Copyright (C) 2014  .NET Workflow Engine Library

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, you can access the official
web page about lgpl: https://www.gnu.org/licenses/lgpl.html
*/

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SlickOne.WebUtility;

namespace FormMaster.Designer.Controllers.WebApi
{
    /// <summary>
    /// File Upload  Controller
    /// </summary>
    public class FineUploadController : Controller
    {
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        /// <summary>
        /// Import file
        /// </summary>
        /// <returns>Import result</returns>
        [HttpPost]
        public async Task<ActionResult> Import()
        {
            string message = string.Empty;

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return Ok(new { success = false, Message = "Unsupported media type!" });
            }

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);
                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        try
                        {
                            using (var streamReader = new StreamReader(section.Body))
                            {
                                var fileContent = await streamReader.ReadToEndAsync();
                                var isOk = SaveYourFileHasBeenUploadedHere(fileContent, out message);

                                return Ok(new { success = isOk, Message =  message});
                            }
                        }
                        catch (System.Exception ex)
                        {
                            throw;
                        }
                    }
                }
                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }
            return Ok(new { success = false, Message = "Unknown other reason!" });
        }

        /// <summary>
        /// Saving file
        /// </summary>
        /// <param name="fileContent">file content</param>
        /// <param name="message">message</param>
        /// <returns></returns>
        private Boolean SaveYourFileHasBeenUploadedHere(string fileContent, out string message)
        {
            //Saving your filecontent into file server or database here;
            message = "Please implement the method SaveYourFileHasBeenUploadedHere() in the file FindUploadController.cs";
            return true;
        }
    }
}