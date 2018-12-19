using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DigitalSignature.Service.Services;
using DigitalSignature.Domain;
using DigitalSignature.Domain.Core.Model;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace DigitalSignature.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocuSignPostController : ControllerBase
    {
        private readonly IDocuSignService _docuSignService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public DocuSignPostController(IDocuSignService docuSignService, IHostingEnvironment HostingEnvironment)
        {
            _docuSignService = docuSignService;
            _hostingEnvironment = HostingEnvironment;
        }

        public ActionResult<DigitalSignature.Domain.Core.Model.DocuSignPostResponse> SendDocumentforSign(List<DigitalSignature.Domain.Core.Model.Recipient> recipients, byte [] filedata)
        {
           
           // HttpContext.Request.Body.
            DocuSignPostResponse docuSignPostResponse = new DocuSignPostResponse();
            string directorypath = _hostingEnvironment.WebRootPath;
            if (!Directory.Exists(directorypath))
            {
                Directory.CreateDirectory(directorypath);
            }
           
           var serverpath = directorypath + Path.GetRandomFileName() + ".pdf";
           System.IO.File.WriteAllBytes(serverpath, filedata);
           docuSignPostResponse =_docuSignService.docusign(serverpath, recipients);
           if (docuSignPostResponse == null)
            {
                return NotFound(docuSignPostResponse);
            }
            return Ok(docuSignPostResponse);
        }
    }
}