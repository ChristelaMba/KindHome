using KindHome.ViewModels;

namespace KindHome.Views;

public partial class NeedsPage : ContentPage
{
    public NeedsPage()
    {
        InitializeComponent();
        BindingContext = new NeedsViewModel();
    }
}
