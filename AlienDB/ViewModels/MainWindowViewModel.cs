using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using AlienDB.Models;

namespace AlienDB.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // Lista filmów
    public ObservableCollection<Film> Filmy { get; } = new();

    private Film? _selectedFilm;
    public Film? SelectedFilm
    {
        get => _selectedFilm;
        set
        {
            if (this.RaiseAndSetIfChanged(ref _selectedFilm, value) != null)
            {
                LoadRasy();
                FilterPostacie();
            }
        }
    }

    // Lista ras i przefiltrowanych postaci
    public ObservableCollection<string> Rasy { get; } = new();
    public ObservableCollection<Postac> PrzefiltrowanePostacie { get; } = new();

    private string? _selectedRasa;
    public string? SelectedRasa
    {
        get => _selectedRasa;
        set
        {
            if (!string.IsNullOrEmpty(this.RaiseAndSetIfChanged(ref _selectedRasa, value)))
            {
                FilterPostacie();
            }
        }
    }

    // Commandy
    public ReactiveCommand<Unit, Unit> AddFilmCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveFilmCommand { get; }

    public MainWindowViewModel()
    {
        // Przykładowy film z postaciami
        var film = new Film
        {
            TytulOryginalny = "Alien",
            TytulPolski = "Obcy – ósmy pasażer Nostromo",
            RokPremiery = 1979,
            Rezyser = "Ridley Scott",
            Gatunek = "Sci-Fi / Horror",
            CzasTrwania = 117,
            Ocena = 8.5,
            Statek = "USCSS Nostromo",
            OpisFabuly = "Załoga statku handlowego Nostromo odbiera sygnał z nieznanej planety...",
            Ciekawostka = "Scena z wyskakującym potworem była niespodzianką dla obsady."
        };

        film.Postacie.Add(new Postac { ImieNazwisko = "Arthur Dallas", Rola = "Kapitan", Aktor = "Tom Skerritt", Rasa = "Człowiek" });
        film.Postacie.Add(new Postac { ImieNazwisko = "Ellen Ripley", Rola = "Oficer pokładowy", Aktor = "Sigourney Weaver", Rasa = "Człowiek" });
        film.Postacie.Add(new Postac { ImieNazwisko = "Xenomorph", Rola = "Obcy", Aktor = "-", Rasa = "Obcy" });

        Filmy.Add(film);
        SelectedFilm = film;

        AddFilmCommand = ReactiveCommand.Create(AddFilm);
        RemoveFilmCommand = ReactiveCommand.Create(RemoveFilm);
    }

    private void LoadRasy()
    {
        Rasy.Clear();
        Rasy.Add("Wszystkie");

        if (SelectedFilm != null)
        {
            foreach (var r in SelectedFilm.Postacie.Select(p => p.Rasa).Distinct())
                Rasy.Add(r);
        }

        SelectedRasa = "Wszystkie";
    }

    private void FilterPostacie()
    {
        PrzefiltrowanePostacie.Clear();
        if (SelectedFilm == null) return;

        foreach (var p in SelectedFilm.Postacie)
        {
            if (SelectedRasa == "Wszystkie" || p.Rasa == SelectedRasa)
                PrzefiltrowanePostacie.Add(p);
        }
    }

    private void AddFilm()
    {
        var nowy = new Film
        {
            TytulOryginalny = "Nowy Film",
            TytulPolski = "(brak)",
            Ocena = 0
        };
        Filmy.Add(nowy);
        SelectedFilm = nowy; // teraz UI się odświeża poprawnie
    }

    private void RemoveFilm()
    {
        if (SelectedFilm != null)
        {
            Filmy.Remove(SelectedFilm);
            SelectedFilm = Filmy.FirstOrDefault();
        }
    }
}
