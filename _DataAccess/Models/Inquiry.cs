﻿using _DataAccess.helperEncryption;
using application.DataAccess.Models;

namespace API_Project.DataAccess.Models
{
    public class Inquiry
    {
        public int Id { get; set; } // الرقم التعريفي للاستفسار (Primary Key)
        public int UserId { get; set; } // الرقم التعريفي للمستخدم اللي بعت الاستفسار (Foreign Key)
        public int PropertyId { get; set; } // الرقم التعريفي للعقار اللي عليه الاستفسار (Foreign Key)
        public string PhoneNumber { get; set; } // رقم الهاتف للشخص الذي يريد الاستفسار
        private string _message;
        public string Message
        {
            get => DesHelper.Decrypt(_message);
            set => _message = DesHelper.Encrypt(value);
        }
        public DateTime DateSent { get; set; } // تاريخ إرسال الاستفسار

        // العلاقات
        public virtual User User { get; set; } // المستخدم اللي بعت الاستفسار
        public virtual Property Property { get; set; } // العقار اللي عليه الاستفسار
    }

}
