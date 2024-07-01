using Newtonsoft.Json;
using RestSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ExamenP3GP.Models;
using CloudKit;
using SQLite;

namespace ExamenP3GP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string BaseUrl = "https://api.chucknorris.io/jokes/random";
        private ObservableCollection<JokeModel> _jokes;
        private JokeModel _selectedJoke;

        public ObservableCollection<JokeModel> Jokes
        {
            get { return _jokes; }
            set { _jokes = value; OnPropertyChanged("Jokes"); }
        }

        public JokeModel SelectedJoke
        {
            get { return _selectedJoke; }
            set { _selectedJoke = value; OnPropertyChanged("SelectedJoke"); }
        }

        public ICommand FetchJokeCommand { get; }
        public ICommand SaveJokeCommand { get; }

        public MainViewModel()
        {
            FetchJokeCommand = new Command(async () => await FetchJoke());
            SaveJokeCommand = new Command(SaveJoke);
            Jokes = new ObservableCollection<JokeModel>();
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

        private void SaveJoke()
        {
            if ( == null) return;

            await _database.InsertAsync();
        }

        private async void InitializeDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Chuck.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<ChuckModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }  