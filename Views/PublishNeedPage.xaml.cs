using KindHome.ViewModels;

namespace KindHome.Views;

public partial class PublishNeedPage : ContentPage
{
    public PublishNeedPage()
    {
        InitializeComponent();
        BindingContext = new PublishNeedViewModel();
    }
}