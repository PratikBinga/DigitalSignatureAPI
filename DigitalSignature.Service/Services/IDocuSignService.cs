using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignature.Service.Services
{
  public  interface IDocuSignService
    {
        DigitalSignature.Domain.Core.Model.DocuSignPostResponse docusign( string serverpath, List <DigitalSignature.Domain.Core.Model.Recipient> recipients);
    }
}
