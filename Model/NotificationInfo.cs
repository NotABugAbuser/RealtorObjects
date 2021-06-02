using FontAwesome5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RealtorObjects.Model
{
    public class NotificationInfo
    {
        private string header = "Успешно";
        private EFontAwesomeIcon icon = EFontAwesomeIcon.Solid_Check;
        private string message = "Это тестовое сообщение";
        private SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(0, 128, 0));
        public NotificationInfo() {
        }
        public NotificationInfo(string message, CodeType type) {
            this.Message = message;
            if (type == CodeType.Successful) {
                Icon = EFontAwesomeIcon.Solid_Check;
                Brush = new SolidColorBrush(Color.FromRgb(0, 128, 0));
                Header = "Успешно";
            } else if (type == CodeType.Exclamation) {
                Icon = EFontAwesomeIcon.Solid_ExclamationTriangle;
                Brush = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                Header = "Внимание";
            } else {
                Icon = EFontAwesomeIcon.Regular_TimesCircle;
                Brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                Header = "Ошибка";
            }
        }

        public EFontAwesomeIcon Icon {
            get => icon;
            private set => icon = value;
        }
        public string Message {
            get => message;
            private set => message = value;
        }
        public SolidColorBrush Brush {
            get => brush;
            private set => brush = value;
        }
        public string Header {
            get => header; 
            private set => header = value;
        }
    }
}
