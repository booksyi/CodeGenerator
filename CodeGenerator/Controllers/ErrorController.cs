using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CodeGenerator.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private IHostingEnvironment env;

        public ErrorController(IHostingEnvironment env)
        {
            this.env = env;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Error()
        {
            Exception exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception is ApiException customException)
            {
                return StatusCode(
                    customException.StatusCode,
                    customException.ThrowObject);
            }

            if (env.IsDevelopment())
            {
                // 開發階段，顯示詳細的錯誤訊息
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    new { exception.Message, detail = exception.ToString() });
            }
            else
            {
                // Warning: 如果要隱藏錯誤訊息，請改寫回傳的物件
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    new { exception.Message });
            }
        }
    }
}

namespace CodeGenerator.Data
{
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; private set; } = (int)HttpStatusCode.InternalServerError;
        public abstract object ThrowObject { get; }

        public ApiException() : base() { }
        public ApiException(string message) : base(message) { }
        public ApiException(string message, Exception innerException) : base(message, innerException) { }
        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public ApiException(HttpStatusCode statusCode) : base()
        {
            StatusCode = (int)statusCode;
        }
    }
}