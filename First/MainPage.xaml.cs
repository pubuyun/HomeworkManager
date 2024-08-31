using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace First
{
    public partial class MainPage : ContentPage
    {
        public class Homework
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
        }

        public class HomeworkList
        {
            public int Count { get; set; }
            public List<Homework> Homeworks { get; set; } = new List<Homework>();
        }

        private string _fileName = Path.Combine(FileSystem.AppDataDirectory, "homework.json");
        private HomeworkList _homeworkList;

        public MainPage()
        {
            InitializeComponent();
            LoadHomeworks();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LoadHomeworks();
            // Optionally, refresh the UI with the loaded homework list
            // DisplayHomeworks();
        }
        private async void OnAddClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddHomework(-1, _homeworkList));
        }
        private async void OnCompleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var homework = button.BindingContext as Homework;
            if (homework != null && homework.Id != -1)
            {
                bool confirm = await DisplayAlert("Complete", "Are you sure you completed this homework?", "Yes", "No");
                if (confirm)
                {
                    if (_homeworkList != null)
                    {
                        _homeworkList.Count -= 1;
                        _homeworkList.Homeworks.RemoveAt(homework.Id);
                        for (int i = 0; i < _homeworkList.Count; i++)
                        {
                            _homeworkList.Homeworks[i].Id = i;
                        }
                        await SaveHomeworksAsync(_homeworkList);
                        await Navigation.PushAsync(new MainPage());
                    }
                }
            }
        }

        public async Task SaveHomeworksAsync(HomeworkList homeworkList)
        {
            try
            {
                string json = JsonSerializer.Serialize(homeworkList);
                await File.WriteAllTextAsync(_fileName, json);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error saving homeworks: {ex.Message}");
            }
        }
        private async void LoadHomeworks()
        {
            _homeworkList = await LoadHomeworksAsync();
            HomeworkListView.ItemsSource = _homeworkList.Homeworks;
        }

        public async Task<HomeworkList> LoadHomeworksAsync()
        {
            try
            {
                if (File.Exists(_fileName))
                {
                    string json = await File.ReadAllTextAsync(_fileName);
                    return JsonSerializer.Deserialize<HomeworkList>(json);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error loading homeworks: {ex.Message}");
            }
            return new HomeworkList();
        }

        private async void OnHomeworkTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var homework = (Homework)e.Item;
                await Navigation.PushAsync(new AddHomework(homework.Id, _homeworkList));
            }
        }
        
    }
}
