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
using System.IO;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Xml;

namespace Slickflow.Designer.Controllers.WebApi
{
    /// <summary>
    /// 文件上传控制器
    /// </summary>
    public class FineUploadController : ApiController
    {
        /// <summary>
        /// 导入流程XML文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync<InMemoryStreamProvider>(new InMemoryStreamProvider());

                //access form data
                NameValueCollection formData = provider.FormData;

                //access files
                IList<HttpContent> files = provider.Files;

                //read file content
                HttpContent file1 = files[0];
                Stream file1Stream = await file1.ReadAsStreamAsync();
                var sr = new StreamReader(file1Stream);
                string xmlContent = sr.ReadToEnd();

                var resp = new HttpResponseMessage { Content = new StringContent("{\"success\":true, \"Message\": \"文件上传成功！\" }", System.Text.Encoding.UTF8, "text/html") };
                return resp;

            }
            catch (System.Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception(string.Format("文件上传发生异常：{0}！", ex.Message), ex));
            }
        }
    }
}
