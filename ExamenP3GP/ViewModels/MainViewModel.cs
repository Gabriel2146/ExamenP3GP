using Newtonsoft.Json;
using RestSharp;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using ExamenP3GP.Models;

namespace ExamenP3GP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string BaseUrl = "https://api.chucknorris.io/jokes/random";
        private ObservableCollection<JokeModel> _jokes;
        private JokeModel _selectedJoke;
        private SQLiteAsyncConnection _database;

        public ObservableCollection<JokeModel> Jokes
        {
            get { return _jokes; }
            set { _jokes = value; OnPropertyChanged(nameof(Jokes)); }
        }

        public JokeModel SelectedJoke
        {
            get { return _selectedJoke; }
            set { _selectedJoke = value; OnPropertyChanged(nameof(SelectedJoke)); }
        }

        public ICommand FetchJokeCommand { get; }
        public ICommand SaveJokeCommand { get; }

        public MainViewModel()
        {
            FetchJokeCommand = new Command(async () => await FetchJoke());
            SaveJokeCommand = new Command(async () => await SaveJoke());
            Jokes = new ObservableCollection<JokeModel>();
            InitializeDatabase();
        }

        private async Task FetchJoke()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest();
            request.Method = Method.Get;

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var joke = JsonConvert.DeserializeObject<JokeModel>(response.Content);
                Jokes.Add(joke);
            }
        }

        private async Task SaveJoke()
        {
            if (SelectedJoke == null) return;

            await _database.InsertAsync(SelectedJoke);
        }

        private async void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "jokes.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<JokeModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}