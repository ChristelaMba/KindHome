using KindHome.Models;

namespace KindHome.ViewModels;

// Une demande de benevolat affichee : le statut change quand l'organisation accepte ou refuse
public class OrgRequestItem : BaseViewModel
{
    public OrgRequestItem(OrgRequest request)
    {
        Request = request;
    }

    public OrgRequest Request { get; }

    public string Name => Request.Name;
    public string Initial => Request.Name.Length > 0 ? Request.Name.Substring(0, 1).ToUpper() : "";
    public string Domain => Request.Domain;
    public string Schedule => Request.Schedule;

    public string Status
    {
        get => Request.Status;
        set
        {
            if (Request.Status == value) return;
            Request.Status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPending));
        }
    }

    public bool IsPending => Request.Status == "En attente";
}