using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;

namespace MediaFunctionLib.Helper
{
    public class EmailHelper
    {
        public EmailHelper(Binder binder)
        {
        }

        public async Task SendMail(string[] recipients,string[] cc,string[] bcc,string subjects,string body)
        {
            throw new NotImplementedException("Email solution not yet chosen");
        }


    }
}
