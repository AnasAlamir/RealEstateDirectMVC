﻿using _DataAccess.helperEncryption;
using API_Project.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace application.DataAccess.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // الرقم التعريفي للمستخدم (Primary Key)
        [Required]
        [StringLength(20)]
        public string F_Name { get; set; } // اسم المستخدم
        [Required]
        [StringLength(20)]
        public string L_Name { get; set; } // اسم المستخدم
        public string FullName { get; private set; } // سيتم حسابها من قاعدة البيانات // اسم المستخدم

        [EmailAddress]
        [Required]
        public string Email { get; set; } // البريد الإلكتروني

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string PhoneNumber { get; set; } // رقم التليفون

        private string? _address;
        public string? Address
        {
            get => DesHelper.Decrypt(_address);
            set => _address = DesHelper.Encrypt(value);
        }
        public string ProfilePicture { get; set; } // صورة الملف الشخصي
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; } 



        // العلاقات
        public virtual IEnumerable<Property> Properties { get; set; } // العقارات التي يمتلكها المستخدم
        public virtual IEnumerable<Inquiry> Inquiries { get; set; } // الاستفسارات المرسلة من قبل المستخدم
        public virtual IEnumerable<Favorite> Favorite { get; set; } // العقارات المفضلة لدى المستخدم




        //[Required]
        //public string Password
        //{
        //    get => _passwordHash;
        //    set => _passwordHash = BCrypt.Net.BCrypt.HashPassword(value); // تشفير كلمة المرور
        //}
        //public string UserType { get; set; } // نوع المستخدم (بائع، مشتري، وكيل، مدير)


    }


}
