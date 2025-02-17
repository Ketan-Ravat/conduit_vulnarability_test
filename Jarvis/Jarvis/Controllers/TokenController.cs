using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Jarvis.Filter;
using Jarvis.Resource;
using Jarvis.Service.Abstract;
using Jarvis.Shared.StatusEnums;
using Jarvis.ViewModels;
using Jarvis.ViewModels.RequestResponseViewModel;
using Jarvis.ViewModels.Utility;
using Jarvis.ViewModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Jarvis.Controllers
{
    [TypeFilter(typeof(requestvalidationFilter))]
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(ResultServiceFilter))]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IUserService userService;
        private readonly IBaseService baseService;
        private readonly IJwtHandler jwtHandler;
        public JWTExternalClientsKeyDetails _options { get; set; }
        public TokenController(IUserService userService, IBaseService baseService,IJwtHandler jwtHandler, IOptions<JWTExternalClientsKeyDetails> options)
        {
            this.userService = userService;
            this.baseService = baseService;
            this._options = options.Value;
            this.jwtHandler = jwtHandler;
        }
        [HttpPost]
        [Route("api/Authenticate")]
        public async Task<Response_Message> GenerateJwt([FromBody] AuthenticateTokenRequestModel login)
        {
            Response_Message response_Message = new Response_Message();
            if (ModelState.IsValid)
            {
                Guid guidOutput;
                bool isValid = true;
                if (login.barcodeId != null && login.barcodeId != String.Empty)
                {
                    isValid = Guid.TryParse(login.barcodeId, out guidOutput);
                }
                if (isValid)
                {
                    JwtResponse response = new JwtResponse();
                    JWTTokenRequestModel jwtDetails = new JWTTokenRequestModel();
                    jwtDetails.privateKey = _options.RsaPrivateKey;
                    jwtDetails.Audiance = _options.Audience;
                    jwtDetails.Issuer = _options.Issuer;
                    response = await userService.UserBarcodeLogin(login,jwtDetails);
                    if (response.response_status > 0)
                    {
                        response_Message.data = response;
                        response_Message.success = (int)ResponseStatusNumber.Success;
                    }
                    else if (response.response_status == (int)ResponseStatusNumber.NotFound)
                    {
                        response_Message.success = (int)ResponseStatusNumber.Success;
                        response_Message.message = ResponseMessages.UserNotFound;
                    }
                    else
                    {
                        response_Message.success = (int)ResponseStatusNumber.Error;
                        response_Message.message = ResponseMessages.Error;
                    }
                }
                else
                {
                    response_Message.message = ResponseMessages.NotValidGuid;
                    response_Message.success = (int)ResponseStatusNumber.Error;
                }
            }
            else
            {
                response_Message.success = (int)ResponseStatusNumber.Error;
                response_Message.message = ResponseMessages.NotValidModel;
            }
            return response_Message;
        }

        [HttpGet]
        [Route("api/token/validate")]
        public Response_Message ValidateJwtAsync([FromQuery] string token)
        {
            Response_Message response = new Response_Message();
            if (jwtHandler.ValidateToken(_options.RsaPublicKey, token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                var user_id = jwtToken.Claims.First(claim => claim.Type == "user_id").Value;
                if (!String.IsNullOrEmpty(user_id))
                {
                    response.data = user_id;
                    response.success = (int)ResponseStatusNumber.Success;
                }
                else
                {
                    response.success = (int)ResponseStatusNumber.Error;
                    response.message = ResponseMessages.InvalidToken;
                }
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = ResponseMessages.InvalidToken;
            }
            return response;
        }

        [HttpGet]
        [Route("api/token/validateauthtoken")]
        public Response_Message ValidateAWSAuthToken([FromQuery] string token , [FromQuery] string domain_name)
        {
            Response_Message response = new Response_Message();

            VerifyCognitoTokenResponseModel authresponse = VerifyCognitoToken.Verify(token, domain_name, 2);

            if (authresponse.success > 0 && authresponse.data != null)
            {
                response.success = (int)ResponseStatusNumber.Success;
                response.message = authresponse.message;
            }
            else
            {
                response.success = (int)ResponseStatusNumber.Error;
                response.message = authresponse.message;
            }
            return response;
        }
    }
}
