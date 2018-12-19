using DigitalSignature.Domain.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using System.IO;
using DocuSign.eSign.Api;
using Newtonsoft.Json;

namespace DigitalSignature.Service.Services
{
    public class DocuSignService : IDocuSignService
    {
        private string INTEGRATOR_KEY = "b43a7a81-bd0a-4894-9d8d-b5eebff80d66";
        public string _accountId = "";
        // private readonly CoreContext _context;
        private readonly IMapper _mapper;
        public DocuSignService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public DocuSignPostResponse docusign(string serverpath, List<Recipient> recipients)
        {
            DocuSignPostResponse docuSignPostResponse = new DocuSignPostResponse();

            ApiClient apiClient = new ApiClient("https://demo.docusign.net/restapi");
            Configuration.Default.ApiClient = apiClient;

            string accountId = loginApi("", "");
            // Read a file from disk to use as a document.  
            byte[] fileBytes = System.IO.File.ReadAllBytes(serverpath);

            List<EnvelopeEvent> _lstEnvelopeEvents = new List<EnvelopeEvent>();
            _lstEnvelopeEvents.Add(new EnvelopeEvent() { EnvelopeEventStatusCode = "sent" });
            _lstEnvelopeEvents.Add(new EnvelopeEvent() { EnvelopeEventStatusCode = "delivered" });
            _lstEnvelopeEvents.Add(new EnvelopeEvent() { EnvelopeEventStatusCode = "completed" });
            _lstEnvelopeEvents.Add(new EnvelopeEvent() { EnvelopeEventStatusCode = "declined" });
            _lstEnvelopeEvents.Add(new EnvelopeEvent() { EnvelopeEventStatusCode = "voided" });


            List<RecipientEvent> _lstRecipientEvents = new List<RecipientEvent>();
            _lstRecipientEvents.Add(new RecipientEvent() { RecipientEventStatusCode = "Sent" });
            _lstRecipientEvents.Add(new RecipientEvent() { RecipientEventStatusCode = "Delivered" });
            _lstRecipientEvents.Add(new RecipientEvent() { RecipientEventStatusCode = "Completed" });
            _lstRecipientEvents.Add(new RecipientEvent() { RecipientEventStatusCode = "Declined" });
            _lstRecipientEvents.Add(new RecipientEvent() { RecipientEventStatusCode = "AuthenticationFailed" });
            _lstRecipientEvents.Add(new RecipientEvent() { RecipientEventStatusCode = "AutoResponded" });


            EventNotification _eventNotification = new EventNotification();
            EnvelopeDefinition envDef = new EnvelopeDefinition();

            _eventNotification.Url = "https://docusigndemoapp.azurewebsites.net/api/DocusignDemoo/WebhookDocuSignResponse";
            _eventNotification.LoggingEnabled = "true";
            _eventNotification.RequireAcknowledgment = "true";
            _eventNotification.UseSoapInterface = "true";
            _eventNotification.IncludeCertificateWithSoap = "false";
            _eventNotification.SignMessageWithX509Cert = "false";
            _eventNotification.IncludeDocuments = "true";
            _eventNotification.IncludeEnvelopeVoidReason = "true";
            _eventNotification.IncludeTimeZone = "true";
            _eventNotification.IncludeSenderAccountAsCustomField = "true";
            _eventNotification.IncludeDocumentFields = "true";
            _eventNotification.IncludeCertificateOfCompletion = "true";
            _eventNotification.EnvelopeEvents = _lstEnvelopeEvents;
            _eventNotification.RecipientEvents = _lstRecipientEvents;


            envDef.EmailSubject = "Please sign this doc";
            // Add a document to the envelope  
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = Path.GetFileName(serverpath);
            doc.DocumentId = "1";
            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);
            // Add a recipient to sign the documeent  
            DocuSign.eSign.Model.Signer signer = new DocuSign.eSign.Model.Signer();

            envDef.Recipients = new DocuSign.eSign.Model.Recipients();
            envDef.Recipients.Signers = new List<DocuSign.eSign.Model.Signer>();

            foreach (var item in recipients)
            {
                signer.Email = item.Email;
                signer.Name = item.Name;
                signer.RecipientId = "1";
                envDef.Recipients.Signers.Add(signer);
            }

           
          
            
            

            envDef.EventNotification = _eventNotification;
            envDef.PurgeState = "documents_and_metadata_queued";
            //set envelope status to "sent" to immediately send the signature request  
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)  
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef, null);
            // envelopesApi.Update("", "", new EnvelopesApi.UpdateOptions() { "purgeState": "documents_and_metadata_queued" });
            // EnvelopesApi.UpdateOptions obj = new EnvelopesApi.UpdateOptions();



            //envelopesApi.DeleteDocuments(accountId, envelopeSummary.EnvelopeId, envDef);
            if (envelopeSummary != null || envelopeSummary.EnvelopeId == null)
            {

            }
            // print the JSON response  
            var result = JsonConvert.SerializeObject(envelopeSummary);

            // storing the envelope in DB
            Recipient recipient = new Recipient();
            //recipient.Description = "envDef.EmailSubject";
            //recipient.Email = recipientEmail;
            //recipient.Name = recipientName;
            //recipient.Status = envelopeSummary.Status;
            //recipient.Documents = fileBytes;
            //recipient.SentOn = System.Convert.ToDateTime(envelopeSummary.StatusDateTime);
            //recipient.EnvelopeID = envelopeSummary.EnvelopeId;
            //DocusignDBEntities _context = new DocusignDBEntities();
            //_context.Recipients.Add(recipient);
            //_context.SaveChanges();


            if (result != null)
            {
                // Console.WriteLine("Document sent Successfully!");
                string successMessage = "Document sent Successfully!";
             //   ViewBag.SucMessage = successMessage;
            }
            else
            {
                //Console.WriteLine("Something went wrong!");
               // ViewBag.SucMessage = "Something went wrong!";

            }

            return docuSignPostResponse;
        }

        public string loginApi(string usr, string pwd)
        {
            usr = "Pratikswvk@gmail.com";
            pwd = "Cns@12345";
            // we set the api client in global config when we configured the client  
            ApiClient apiClient = Configuration.Default.ApiClient;
            string authHeader = "{\"Username\":\"" + usr + "\", \"Password\":\"" + pwd + "\", \"IntegratorKey\":\"" + INTEGRATOR_KEY + "\"}";
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);
            // we will retrieve this from the login() results  
            string accountId = null;
            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object  
            AuthenticationApi authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();
            // find the default account for this user  
            foreach (DocuSign.eSign.Model.LoginAccount loginAcct in loginInfo.LoginAccounts)
            {
                if (loginAcct.IsDefault == "true")
                {
                    accountId = loginAcct.AccountId;
                    _accountId = accountId;
                    break;
                }
            }
            if (accountId == null)
            { // if no default found set to first account  
                accountId = loginInfo.LoginAccounts[0].AccountId;
            }
            return accountId;
        }



    }
}
