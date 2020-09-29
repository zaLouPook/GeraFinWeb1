using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.GeraFinWeb
{
    public class UserNotifications
    {
        [Key]
        public int UserId { get; set; }
        public int NotificationType { get; set; }
        public int NotificationId { get; set; }
        public int NotificationNo { get; set; }
        public string NotificationTxt { get; set; } = "";
        public string NotificationTxt1 { get; set; } = "";
        public string NotificationTxt2 { get; set; } = "";
        public string NotificationTxt3 { get; set; } = "";
        public DateTime NotificationDate { get; set; }
    }
}