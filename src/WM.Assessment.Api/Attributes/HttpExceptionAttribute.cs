using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WM.Assessment.Api.Models;
using WM.Assessment.Application.Exceptions;

namespace WM.Assessment.Api.Attributes
{
    /// <summary>
    ///     Maps an exception to a relevant http status
    /// </summary>
    public class HttpExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                //400 BAD REQUEST
                case BadRequestException _:
                    var msg = context.Exception.Message ??
                              "The request contains malformed syntax or is missing required parameters.";
                    context.Result = new ObjectResult(new ErrorModel
                        {
                            Status = (int) HttpStatusCode.BadRequest,
                            Message = msg
                        })
                        {StatusCode = (int) HttpStatusCode.BadRequest};
                    return;

                // //403 NOT AUTHORIZED
                // case NotAuthorizedException _:
                //     context.Result = new ObjectResult(new ErrorModel
                //     {
                //         Status = (int)HttpStatusCode.Forbidden,
                //         Message = "The user does not have the required permissions to access the resource."
                //     })
                //     { StatusCode = (int)HttpStatusCode.Forbidden };
                //     return;

                //404 NOT FOUND
                case NotFoundException _:
                    context.Result = new ObjectResult(new ErrorModel
                        {
                            Status = (int) HttpStatusCode.NotFound,
                            Message = context.Exception.Message
                        })
                        {StatusCode = (int) HttpStatusCode.NotFound};
                    return;
            }

            //DON'T RETURN MESSAGE TO CLIENT (security risk)
            var internalMessage = "An error has occurred.";
//#if DEBUG
//            internalMessage = context.Exception.Message;
            //#endif
            internalMessage = context.Exception.Message;
            //500 - Unhandled
            context.Result = new ObjectResult(new ErrorModel
                {
                    Status = (int) HttpStatusCode.InternalServerError,
                    Message = internalMessage
                })
                {StatusCode = (int) HttpStatusCode.InternalServerError};
        }
    }
}