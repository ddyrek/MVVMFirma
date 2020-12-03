using GalaSoft.MvvmLight.Messaging;
using MVVMFirma.Helpers;
using MVVMFirma.Models.EntitiesForAllView;
using MVVMFirma.ViewModels.DokumentyVM;
using MVVMFirma.ViewModels.Zasoby;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace MVVMFirma.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields
        //okno główne zawiera:
        //1. kolekcje linków do zakładek wyświetlaną z lewej strony
        private ReadOnlyCollection<CommandViewModel> _Commands;
        //2. kolekcje zakładek wyświetlanych z prawej strony
        private ObservableCollection<WorkspaceViewModel> _Workspaces;
        #endregion Fields

        #region Commands
        //to jest wlasciwość pobierają wszystkie komendy z lewej strony(linki)
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if(_Commands == null)
                {
                    //Tworzymy listę złożoną ze wszystkich linków z lewej strony
                    //Tę listę tworzymy przy pomocy funkcji CreateCommands
                    //która tworzy linki
                    List<CommandViewModel> cmds = this.CreateCommands();
                    //listę tę stworzyliśmy po to żeby utworzyć z niej 
                    //kolekcję tylko do odczytu
                    _Commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _Commands;
            }
        }

        //utworzymy funkcję która stworzy komendę towar i komendę towary
        //czyli linki z lewej stony
        public List<CommandViewModel> CreateCommands()
        {
            // to jest Messenger ktory nasluchuje na komunikat typu string,
            // ten komunikat jest wysylany z klasy WszystkieViewModel
            // jak messenger złapie ten komunikat to wywola metodę open ktora 
            // zdefinujemy w sekcji Helpers

            Messenger.Default.Register<string>(this, open); //wykonaj Resolve
            Messenger.Default.Register<MessengerClass>(this, open2); //messanger łapiący MessengerClass (modyfikacja rekordu)

            //tworzymy nową listę CommandViewModeli inicjalizując ją przy pomocy 
            //inicjalizatora. W inicjlalizatorze tworzymy kolejne linki okreslajac
            //ich nazwę oraz nazwę funkcji, która wywoła zakładkę.
            return new List<CommandViewModel>
            {
                new CommandViewModel("Towar",new BaseCommand(()=>this.createTowar())),
                new CommandViewModel("Towary",new BaseCommand(()=>this.showAllTowar())),
                new CommandViewModel("Kategoria towaru", new BaseCommand(()=>this.createKategoria())),
                new CommandViewModel("Kategorie towarów", new BaseCommand(()=>this.showAllKategorie())),
                new CommandViewModel("Typ towaru", new BaseCommand(()=>this.createTypTowaru())),
                new CommandViewModel("Typy towaru", new BaseCommand(()=>this.showAllTypyTowaru())),
                new CommandViewModel("Magazyn", new BaseCommand(()=>this.createMagazyn())),
                new CommandViewModel("Magazyny", new BaseCommand(()=>this.showAllMagazyny())),
                new CommandViewModel("Okres mag.", new BaseCommand(()=>this.createOkresMag())),
                new CommandViewModel("Okresy mag.", new BaseCommand(()=>this.showAllOkresyMag())),
                new CommandViewModel("Zasoby", new BaseCommand(()=>this.showAllZasoby())),
                //new CommandViewModel("Waluta", new BaseCommand(()=>this.createWaluta())),
                //new CommandViewModel("Waluty", new BaseCommand(() => this.showAllWaluty())),
                //new CommandViewModel("Stawka VAT", new BaseCommand(()=>this.createStawkaVat())),
                //new CommandViewModel("Stawki VAT", new BaseCommand(()=>this.showAllStawkiVat())),
                //new CommandViewModel("Kraj", new BaseCommand(()=>this.createKraj())),
                //new CommandViewModel("Kraje",new BaseCommand(()=>this.showAllKraje())),
                //new CommandViewModel("Pracownik",new BaseCommand(()=>this.createPracownik())),
                //new CommandViewModel("Pracownicy",new BaseCommand(()=>this.showAllPracownik())),
                new CommandViewModel("PZ", new BaseCommand(()=>this.createPZ())),
                new CommandViewModel("Pozycja przyjęcia",new BaseCommand(()=>this.createPozycjaPrzyjeciaMag())),
                new CommandViewModel("Przyjecia zewnętrzne",new BaseCommand(()=>this.showAllPz())),
                new CommandViewModel("Nowa Faktura", new BaseCommand(()=>this.createFaktura())),
                new CommandViewModel("Faktury", new BaseCommand(() => this.showAllFaktury())),
                new CommandViewModel("Pozycja faktury",new BaseCommand(()=>this.createPozycjaFaktury())),
                new CommandViewModel("Adres", new BaseCommand(()=>this.createAdres())),
                new CommandViewModel("Adresy", new BaseCommand(()=>this.showAllAdresy())),
                new CommandViewModel("Trasa", new BaseCommand(()=>this.createTrasa())),
                new CommandViewModel("Trasy", new BaseCommand(()=>this.showAllTrasy())),
                //new CommandViewModel("Dni Tygodnia",new BaseCommand(()=>this.showAllDniTygodnia())),
                new CommandViewModel("Kontrahent", new BaseCommand(()=>this.createKontrahent())),
                new CommandViewModel("Kontrahenci",new BaseCommand(()=>this.showAllKontrahenci())),
                //new CommandViewModel("Statusy",new BaseCommand(()=>this.showAllStatusy())),
                new CommandViewModel("Typy",new BaseCommand(()=>this.showAllTypy())),
                //new CommandViewModel("Parametry Szkla",new BaseCommand(()=>this.showAllParametrySzk())),
                //new CommandViewModel("Jednostki",new BaseCommand(()=>this.showAllJednostki())),
                //new CommandViewModel("Raport sprzedaży",new BaseCommand(()=>this.showRaportSprzedazy())),
                //new CommandViewModel("Wartosc zakupow",new BaseCommand(()=>this.showWartosZakupow()))
            };
        }
        #endregion Commands

        #region Workspaces
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if(_Workspaces == null)
                {
                    _Workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _Workspaces.CollectionChanged += this.OnWorkspaceChanged;
                }
                return _Workspaces;
            }
        }
        public void OnWorkspaceChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;
            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }
        public void OnWorkspaceRequestClose(Object sender,
            EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            this.Workspaces.Remove(workspace);
        }
        #endregion Workspaces

        #region PrivateHelpers
        //To jest funkcja która uruchomi zakładkę do dodawania towarów
        private void createTowar()
        {
            //najpierw tworzymy ViewModel         
            NowyTowarViewModel workspace = new NowyTowarViewModel();
            //następnie dodajemy utworzony ViewModel do kolekcji Workspejsów 
            //czyli wszystkich zakładek
            this.Workspaces.Add(workspace);
            //uaktywniamy utworzoną zakładkę
            this.SetActiveWorkspace(workspace);
        }
        private void createTowar2(int id)
        {
            //najpierw tworzymy ViewModel drugim konstruktorem        
            NowyTowarViewModel workspace = new NowyTowarViewModel(id);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }
        private void createPracownik()
        {
            NowyPracownikViewModel workspace = new NowyPracownikViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }
        //private void createPracownik2(int id)
        //{
        //    NowyPracownikViewModel workspace = new NowyPracownikViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}
        private void createMagazyn()
        {
            NowyMagazynViewModel workspace = new NowyMagazynViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createMagazyn2(int id)
        //{
        //    NowyMagazynViewModel workspace = new NowyMagazynViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createOkresMag()
        {
            NowyOkresMagViewModel workspace = new NowyOkresMagViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createOkresMag2(int id)
        //{
        //    NowyOkresMagViewModel workspace = new NowyOkresMagViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createWaluta()
        {
            NowaWalutaViewModel workspace = new NowaWalutaViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createWaluta2(int id)
        //{
        //    NowaWalutaViewModel workspace = new NowaWalutaViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createStawkaVat()
        {
            NowaStawkaVatViewModel workspace = new NowaStawkaVatViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createStawkaVat2(int id)
        //{
        //    NowaStawkaVatViewModel workspace = new NowaStawkaVatViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createKraj()
        {
            NowyKrajViewModel workspace = new NowyKrajViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createKraj2(int id)
        //{
        //    NowyKrajViewModel workspace = new NowyKrajViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createPZ()
        {
            NowePZViewModel workspace = new NowePZViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        private void createPZ2(int id)
        {
            NowePZViewModel workspace = new NowePZViewModel(id);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        private void createPozycjaPrzyjeciaMag()
        {
            NowaPozycjaPZViewModel workspace = new NowaPozycjaPZViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createPozycjaPrzyjeciaMag2(int id)
        //{
        //    NowaPozycjaPZViewModel workspace = new NowaPozycjaPZViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createFaktura()
        {
            NowaFakturaViewModel workspace = new NowaFakturaViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        private void createFaktura2(int id)
        {
            NowaFakturaViewModel workspace = new NowaFakturaViewModel(id);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }


        private void createPozycjaFaktury()
        {
            NowaPozycjaFakturyViewModel workspace = new NowaPozycjaFakturyViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createPozycjaFaktury2(int id)
        //{
        //    NowaPozycjaFakturyViewModel workspace = new NowaPozycjaFakturyViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createAdres()
        {
            NowyAdresViewModel workspace = new NowyAdresViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createAdres2(int id)
        //{
        //    NowyAdresViewModel workspace = new NowyAdresViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createTrasa()
        {
            NowaTrasaViewModel workspace = new NowaTrasaViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createTrasa2(int id)
        //{
        //    NowaTrasaViewModel workspace = new NowaTrasaViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createTypTowaru()
        {
            NowyTypTowaruViewModel workspace = new NowyTypTowaruViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createTypTowaru2(int id)
        //{
        //    NowyTypTowaruViewModel workspace = new NowyTypTowaruViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}

        private void createKategoria()
        {
            NowaKategoriaViewModel workspace = new NowaKategoriaViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        private void createKategoria2(int id)
        {
            //najpierw tworzymy ViewModel drugim konstruktorem        
            NowaKategoriaViewModel workspace = new NowaKategoriaViewModel(id);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        private void createKontrahent()
        {
            NowyKontrahentViewModel workspace = new NowyKontrahentViewModel();
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        //private void createKontrahent2(int id)
        //{
        //    NowyKontrahentViewModel workspace = new NowyKontrahentViewModel(id);
        //    this.Workspaces.Add(workspace);
        //    this.SetActiveWorkspace(workspace);
        //}


        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkich kontrahentow
        private void showAllKontrahenci()
        {
            WszyscyKontrahenciViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszyscyKontrahenciViewModel)
                as WszyscyKontrahenciViewModel;
            if (workspace == null)
            {
                workspace = new WszyscyKontrahenciViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie towary
        private void showAllTowar()
        {
            //Pod zmienną workspace podstawiam tę pierwszą zakładkę, która jest wszystkimi
            //towarami
            //jeżeli takiej zakładki nie ma FirstOrDefault zwróci null-a
            WszystkieTowaryViewModel workspace = 
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieTowaryViewModel) 
                as WszystkieTowaryViewModel;
            //Jeżeli takiej zakładki nie ma, czyli nie ma zakładki ze wszystkimi towarami
            if (workspace == null)
            {
                //to należy ją stworzyć
                workspace = new WszystkieTowaryViewModel();
                //i dodać do kolekcji zakładek
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie kategorie
        private void showAllZasoby()
        {
            WszystkieZasobyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieZasobyViewModel)
                as WszystkieZasobyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieZasobyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        ////to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie kategorie
        private void showAllPz()
        {
            WszystkiePZViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkiePZViewModel)
                as WszystkiePZViewModel;
            if (workspace == null)
            {
                workspace = new WszystkiePZViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie kategorie
        private void showAllKategorie()
        {
            WszystkieKategorieViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieKategorieViewModel)
                as WszystkieKategorieViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieKategorieViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie kategorie
        private void showAllStawkiVat()
        {
            WszystkieStawkiVatViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieStawkiVatViewModel)
                as WszystkieStawkiVatViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieStawkiVatViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkich kontrahentow
        private void showAllWaluty()
        {
            WszystkieWalutyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieWalutyViewModel)
                as WszystkieWalutyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieWalutyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie Kraje
        private void showAllKraje()
        {
            WszystkieKrajeViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieKrajeViewModel)
                as WszystkieKrajeViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieKrajeViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie magazyny
        private void showAllMagazyny()
        {
            WszystkieMagazynyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieMagazynyViewModel)
                as WszystkieMagazynyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieMagazynyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie okresy magazynowe
        private void showAllOkresyMag()
        {
            WszystkieOkresyMagViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieOkresyMagViewModel)
                as WszystkieOkresyMagViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieOkresyMagViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie trasy
        private void showAllTrasy()
        {
            WszystkieTrasyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieTrasyViewModel)
                as WszystkieTrasyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieTrasyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkich pracowników
        private void showAllPracownik()
        {
            WszyscyPracownicyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszyscyPracownicyViewModel)
                as WszyscyPracownicyViewModel;
            if (workspace == null)
            {
                workspace = new WszyscyPracownicyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie faktury
        private void showAllFaktury()
        {
            WszystkieFakturyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieFakturyViewModel)
                as WszystkieFakturyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieFakturyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie adresy
        private void showAllAdresy()
        {
            WszystkieAdresyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieAdresyViewModel)
                as WszystkieAdresyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieAdresyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie statusy
        private void showAllStatusy()
        {
            WszystkieStatusyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieStatusyViewModel)
                as WszystkieStatusyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieStatusyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie typy
        private void showAllTypy()
        {
            WszystkieTypyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieTypyViewModel)
                as WszystkieTypyViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieTypyViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie typy
        private void showAllTypyTowaru()
        {
            WszystkieTypyTowaruViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieTypyTowaruViewModel)
                as WszystkieTypyTowaruViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieTypyTowaruViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie dni tygodnia
        private void showAllDniTygodnia()
        {
            WszystkieDniTygodniaViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieDniTygodniaViewModel)
                as WszystkieDniTygodniaViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieDniTygodniaViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie parametry (grubosci) szkla
        private void showAllParametrySzk()
        {
            WszystkieGrubosciSzklaViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieGrubosciSzklaViewModel)
                as WszystkieGrubosciSzklaViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieGrubosciSzklaViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą wszystkie trasy
        private void showAllJednostki()
        {
            WszystkieJednostkiViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WszystkieJednostkiViewModel)
                as WszystkieJednostkiViewModel;
            if (workspace == null)
            {
                workspace = new WszystkieJednostkiViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą raport sprzedaży
        private void showRaportSprzedazy()
        {
            RaportSprzedazyViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is RaportSprzedazyViewModel)
                as RaportSprzedazyViewModel;

            if (workspace == null)
            {
                workspace = new RaportSprzedazyViewModel();
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);
        }

        //to jest funkcja, która uaktywnia zakładkę pokazującą Watosc Zakupów Kontrahentów
        private void showWartosZakupow()
        {
            WartoscZakupowKlientowViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is WartoscZakupowKlientowViewModel)
                as WartoscZakupowKlientowViewModel;
            if (workspace == null)
            {
                workspace = new WartoscZakupowKlientowViewModel();
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }


        //Ta funkcja uaktywnia zakładkę
        public void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));
            ICollectionView collectionView =
                CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
        }

        // to jest metoda ktora zostaje wywolana przez messengera ktory jest w 
        // CreateCommands
        private void open(string name)//name oznacza tego stringa ktorego wyśle
                                      // messenger z WszystkieViewModel lub dla pozycji faktury
                                      //w JedenWszystkieViewModel
        {
            if (name == "FakturyAdd")
                createFaktura(); // wywoluje okno do dodawania faktury np z poziomu wszystkich faktur (przycisk dodaj)
            if (name == "Przyjęcia zewnętrzneAdd")
                createPZ(); // wywoluje okno do dodawania PZ np z poziomu wszystkich PZ (przycisk dodaj)
            if (name == "TowaryAdd")
                createTowar(); // wywoluje okno do dodawania towaru np dla przycisku dodaj w towarach
            if (name == "MagazynyAdd")
                createMagazyn(); // wywoluje okno do dodawania magazynu np dla przycisku dodaj w magazyny
            if (name == "Okresy mag.Add")
                createOkresMag(); // wywoluje okno do dodawania okresu magazynowego np dla przycisku dodaj w okresy mag
            if (name == "WalutyAdd")
                createWaluta(); // wywoluje okno do dodawania waluta np dla przycisku dodaj w waluty
            if (name == "Stawki VATAdd")
                createStawkaVat(); // wywoluje okno do dodawania stawki VAT np dla przycisku dodaj w stawkach Vat
            if (name == "KrajeAdd")
                createKraj(); // wywoluje okno do dodawania waluta np dla przycisku dodaj w Kraje
            if (name == "KontrahenciAdd")
                createKontrahent(); // wywoluje okno do dodawania kontrahenta - przycisk dodaj
            if (name == "KontrahenciShow")
                showAllKontrahenci();
            if (name == "TowaryShow") 
                showAllTowar();
            if (name == "AdressCreate")
                createAdres(); // wywoluje okno do dodawania adresu - przycisk dodaj w NowyKontrahentView;
            if (name == "AdresyAdd")
                createAdres(); // wywoluje okno do dodawania adresu - przycisk dodaj we WszystkieAdresyView;
            if (name == "TrasaCreate")
                createTrasa(); // wywoluje okno do dodawania trasy- przycisk dodaj w NowaTrasaView;
            if (name == "TrasyAdd")
                createTrasa(); // wywoluje okno do dodawania trasy - przycisk dodaj we WszystkieTrasyView;
            if (name == "Typy towaruAdd")
                createTypTowaru(); // wywoluje okno do dodawania typu towaru - przycisk dodaj we WszystkieTypyTowaruView;
            if (name == "Kategorie towarówAdd")
                createKategoria();
            //if (name == "Dni tygodniaAdd")
            //    createDzienTygodnia(); // wywoluje okno do dodawania dnia tygodnia - przycisk dodaj we WszystkieDniTygodniaView;
            //if (name == "Parametry szklaAdd")
            //    createParametrSzkla(); // wywoluje okno do dodawania parametrow szkla - przycisk dodaj we WszystkieGrubosciSzklaView;
            //if (name == "JednostkiAdd")
            //    createJednostka(); // wywoluje okno do dodawania jednostki - przycisk dodaj we WszystkieJednostkiView;
            if (name == "Pozycje fakturyAdd")
                createPozycjaFaktury(); // wywoluje okno do dodawania pozycji faktury - przycisk dodaj w NowaFakturaView;
            if (name == "Pozycje przyjęciaAdd") //("DisplayListName" + "Add") DisplayListName w konstruktorze NowaPzViewModel
                createPozycjaPrzyjeciaMag(); // wywoluje okno do dodawania pozycji PZ - przycisk dodaj w NowePzView;
        }
        private void open2(MessengerClass message)
        {
            if (message.Komunikat == "TowarModify")
                createTowar2(message.Id);
            if (message.Komunikat == "KategoriaModify")
                createKategoria2(message.Id);
            if (message.Komunikat == "FakturaModify")
                createFaktura2(message.Id);
            if (message.Komunikat == "PZModify")
                createPZ2(message.Id);
        }
        #endregion PrivateHelpers

        #region CommadsMenu
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę createPracownik, która otworzy zakładkę do dodawania
        //pracownika
        private BaseCommand _CreatePracownikCommand;
        public ICommand CreatePracownikCommand
        {
            get
            {
                if (_CreatePracownikCommand == null)
                {
                    _CreatePracownikCommand = 
                        new BaseCommand(()=>createPracownik());
                }
                return _CreatePracownikCommand;
            }
        }
        //dodane przezmnie
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę createTowar, która otworzy zakładkę do dodawania
        //towaru
        private BaseCommand _CreateTowarCommand;
        public ICommand CreateTowarCommand
        {
            get
            {
                if (_CreateTowarCommand == null)
                {
                    _CreateTowarCommand =
                        new BaseCommand(() => createTowar());
                }
                return _CreateTowarCommand;
            }
        }
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę createTowar, która otworzy zakładkę do dodawania PZ
        private BaseCommand _CreatePZCommand;
        public ICommand CreatePZCommand
        {
            get
            {
                if (_CreatePZCommand == null)
                {
                    _CreatePZCommand =
                        new BaseCommand(() => createPZ());
                }
                return _CreatePZCommand;
            }
        }
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllPz, która otworzy zakładkę z dokumentami PZ
        private BaseCommand _ShowAllPzCommand;
        public ICommand ShowAllPzCommand
        {
            get
            {
                if (_ShowAllPzCommand == null)
                {
                    _ShowAllPzCommand =
                        new BaseCommand(() => showAllPz());
                }
                return _ShowAllPzCommand;
            }
        }
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllFaktury, która otworzy zakładkę z Fakturami
        private BaseCommand _ShowAllFakturyCommand;
        public ICommand ShowAllFakturyCommand
        {
            get
            {
                if (_ShowAllFakturyCommand == null)
                {
                    _ShowAllFakturyCommand =
                        new BaseCommand(() => showAllFaktury());
                }
                return _ShowAllFakturyCommand;
            }
        }
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllKontrahenci, która otworzy zakładkę z Kontrahentami
        private BaseCommand _ShowAllKontrahenciCommandd;
        public ICommand ShowAllKontrahenciCommand
        {
            get
            {
                if (_ShowAllKontrahenciCommandd == null)
                {
                    _ShowAllKontrahenciCommandd =
                        new BaseCommand(() => showAllKontrahenci());
                }
                return _ShowAllKontrahenciCommandd;
            }
        }
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllPracownik, która otworzy zakładkę z pracownikami
        private BaseCommand _ShowAllPracownikCommand;
        public ICommand ShowAllPracownikCommand
        {
            get
            {
                if (_ShowAllPracownikCommand == null)
                {
                    _ShowAllPracownikCommand =
                        new BaseCommand(() => showAllPracownik());
                }
                return _ShowAllPracownikCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllTrasy, która otworzy zakładkę z trasami
        private BaseCommand _ShowAllTrasyCommand;
        public ICommand ShowAllTrasyCommand
        {
            get
            {
                if (_ShowAllTrasyCommand == null)
                {
                    _ShowAllTrasyCommand =
                        new BaseCommand(() => showAllTrasy());
                }
                return _ShowAllTrasyCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllJednostki, która otworzy zakładkę z jednostkami
        private BaseCommand _ShowAllJednostkiCommand;
        public ICommand ShowAllJednostkiCommand
        {
            get
            {
                if (_ShowAllJednostkiCommand == null)
                {
                    _ShowAllJednostkiCommand =
                        new BaseCommand(() => showAllJednostki());
                }
                return _ShowAllJednostkiCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllWaluty, która otworzy zakładkę z jednostkami
        private BaseCommand _ShowAllWalutyCommand;
        public ICommand ShowAllWalutyCommand
        {
            get
            {
                if (_ShowAllWalutyCommand == null)
                {
                    _ShowAllWalutyCommand =
                        new BaseCommand(() => showAllWaluty());
                }
                return _ShowAllWalutyCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllStawkiVat, która otworzy zakładkę z jednostkami
        private BaseCommand _ShowAllStawkiVatCommand;
        public ICommand ShowAllStawkiVatCommand
        {
            get
            {
                if (_ShowAllStawkiVatCommand == null)
                {
                    _ShowAllStawkiVatCommand =
                        new BaseCommand(() => showAllStawkiVat());
                }
                return _ShowAllStawkiVatCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllStatusy, która otworzy zakładkę ze statusami
        private BaseCommand _ShowAllStatusyCommand;
        public ICommand ShowAllStatusyCommand
        {
            get
            {
                if (_ShowAllStatusyCommand == null)
                {
                    _ShowAllStatusyCommand =
                        new BaseCommand(() => showAllStatusy());
                }
                return _ShowAllStatusyCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllDniTygodnia, która otworzy zakładkę ze dniami tygodnia
        private BaseCommand _ShowAllDniTygodniaCommand;
        public ICommand ShowAllDniTygodniaCommand
        {
            get
            {
                if (_ShowAllDniTygodniaCommand == null)
                {
                    _ShowAllDniTygodniaCommand =
                        new BaseCommand(() => showAllDniTygodnia());
                }
                return _ShowAllDniTygodniaCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllKraje, która otworzy zakładkę z karajami
        private BaseCommand _ShowAllKrajeCommand;
        public ICommand ShowAllKrajeCommand
        {
            get
            {
                if (_ShowAllKrajeCommand == null)
                {
                    _ShowAllKrajeCommand =
                        new BaseCommand(() => showAllKraje());
                }
                return _ShowAllKrajeCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllParametrySzk, która otworzy zakładkę z parametrami szkła
        private BaseCommand _ShowAllParametrySzkCommand;
        public ICommand ShowAllParametrySzkCommand
        {
            get
            {
                if (_ShowAllParametrySzkCommand == null)
                {
                    _ShowAllParametrySzkCommand =
                        new BaseCommand(() => showAllParametrySzk());
                }
                return _ShowAllParametrySzkCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showAllAdresy, która otworzy zakładkę z adresami
        private BaseCommand _ShowAllAdresyCommand;
        public ICommand ShowAllAdresyCommand
        {
            get
            {
                if (_ShowAllAdresyCommand == null)
                {
                    _ShowAllAdresyCommand =
                        new BaseCommand(() => showAllAdresy());
                }
                return _ShowAllAdresyCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod przycisk dodaj adres ... w widoku tworzenia nowege kontrahenta
        //wywoła ona metodę createAdres, która otworzy zakładkę z dodawaniem adresu
        private BaseCommand _CreateAdressCommand;
        public ICommand CreateAdressCommand
        {
            get
            {
                if (_CreateAdressCommand == null)
                {
                    _CreateAdressCommand =
                        new BaseCommand(() => createAdres());
                }
                return _CreateAdressCommand;
            }
        }

        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showRaportSprzedazy, która otworzy zakładkę z Raportem sprzedaży
        private BaseCommand _ShowRaportSprzedazyCommand;
        public ICommand ShowRaportSprzedazyCommand
        {
            get
            {
                if (_ShowRaportSprzedazyCommand == null)
                {
                    _ShowRaportSprzedazyCommand =
                        new BaseCommand(() => showRaportSprzedazy());
                }
                return _ShowRaportSprzedazyCommand;
            }
        }
        //to jest komenda która zostanie podpięta pod pasek narzędzi i menu
        //wywoła ona metodę showWartosZakupow, która otworzy zakładkę ...
        private BaseCommand _ShowWartoscZakpowCommand;
        public ICommand ShowWartoscZakpowCommand
        {
            get
            {
                if (_ShowWartoscZakpowCommand == null)
                {
                    _ShowWartoscZakpowCommand =
                        new BaseCommand(() => showWartosZakupow());
                }
                return _ShowWartoscZakpowCommand;
            }
        }
        #endregion CommadsMenu

    }
}
