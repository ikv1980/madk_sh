using System.Windows;

namespace Project.Views
{
    public partial class MessageOk : Window
    {
        public string Message { get; set; }
        public string IconPath { get; set; }

        public MessageOk(string title, string message, string messageType)
        {
            InitializeComponent();
            Title = title;
            Message = message;
            IconPath = GetIconPath(messageType);
            DataContext = this;
        }

        public static void Show(string title, string message, string messageType)
        {
            MessageOk custom = new MessageOk(title, message, messageType);
            custom.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string GetIconPath(string messageType)
        {
            switch (messageType.ToLower())
            {
                case "danger":
                    return "/Resources/ico/danger.png";
                case "attention":
                    return "/Resources/ico/attention.png";
                case "success":
                    return "/Resources/ico/success.png";
                default:
                    return string.Empty;
            }
        }
    }
}
