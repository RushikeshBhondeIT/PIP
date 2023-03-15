using EmployeeServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeServiceContracts
{
    public interface IEmailService
    {
        void SendEmailToVerify(MessageForEmail emailMessage);
    }
}
