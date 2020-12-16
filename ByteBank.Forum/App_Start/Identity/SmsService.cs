using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ByteBank.Forum.App_Start.Identity
{
    public class SmsService : IIdentityMessageService
    {
        private readonly string _twilioSID = ConfigurationManager.AppSettings["twilio:SID"];
        private readonly string _twilioAuthToken = ConfigurationManager.AppSettings["twilio:auth_token"];
        private readonly string _twilioFromNumber = ConfigurationManager.AppSettings["twilio:from_number"];

        public async Task SendAsync(IdentityMessage message)
        {
            TwilioClient.Init(_twilioSID, _twilioAuthToken);

            await MessageResource.CreateAsync(
                new PhoneNumber(message.Destination),
                from: _twilioFromNumber,
                body: message.Body);
        }
    }
}