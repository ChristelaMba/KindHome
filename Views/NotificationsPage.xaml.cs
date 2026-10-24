using KindHome.Models;
using KindHome.Services;
using KindHome.ViewModels;

namespace KindHome.Views;

public partial class NotificationsPage : ContentPage
{
    public NotificationsPage()
    {
        InitializeComponent();
        BindingContext = new NotificationsViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Les notifications de la base sont ajoutees en tete de la liste du compte
        var user = UserSession.CurrentUser;
        if (user == null) return;

        try
        {
            await NotificationService.LoadForUserAsync(user);
        }
        catch (Exception)
        {
            // Lecture impossible : on garde l'affichage actuel
            return;
        }

        // La personne connectee a change pendant la lecture : on ignore le resultat
        if (UserSession.CurrentUser != user) return;

        BindingContext = new NotificationsViewModel();
    }
}
