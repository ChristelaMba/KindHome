using KindHome.ViewModels;

namespace KindHome.Views;

public partial class NotificationsPage : ContentPage
{
    public NotificationsPage()
    {
        InitializeComponent();
        BindingContext = new NotificationsViewModel();
    }
}