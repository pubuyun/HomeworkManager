namespace First
{
    using System.Security.Cryptography.X509Certificates;
    using System.Text.Json;
    using static First.AddHomework;
    using static First.MainPage;

    public partial class AddHomework : ContentPage
    {
        int ID;
        string _fileName = Path.Combine(FileSystem.AppDataDirectory, "homework.json"); // Ensure _fileName is initialized
        private HomeworkList _homeworkList;


        public AddHomework(int id, HomeworkList homeworkList)
        {
            _homeworkList = homeworkList;
            ID = id;
            InitializeComponent();
            DueDatePicker.MinimumDate = DateTime.Today;
            DueDatePicker.MaximumDate = DateTime.Today.AddDays(30);
            if (_homeworkList != null)
                if (_homeworkList.Count > 0 && id>=0)
                {
                    {
                        TitleEditor.Text = _homeworkList.Homeworks[id].Title;
                        TextEditor.Text = _homeworkList.Homeworks[id].Text;
                        DueDatePicker.Date = _homeworkList.Homeworks[id].DueDate;
                    }
                }
        }


        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            // Save the file.
            if (ID == -1) Add_Homework(TitleEditor.Text, TextEditor.Text, DueDatePicker.Date);
            else SaveHomework(TitleEditor.Text, TextEditor.Text, DueDatePicker.Date, ID);
            await Navigation.PushAsync(new MainPage());
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this homework?", "Yes", "No");
            
                if (confirm)
                {
                    if (ID != -1)
                    {
                        if (_homeworkList != null)
                        {
                            _homeworkList.Count -= 1;
                            _homeworkList.Homeworks.RemoveAt(ID);
                            for (int i = 0; i < _homeworkList.Count; i++)
                            {
                                _homeworkList.Homeworks[i].Id = i;
                            }
                            await SaveHomeworksAsync(_homeworkList);
                            await Navigation.PushAsync(new MainPage());
                        }
                    }
                TextEditor.Text = string.Empty;
                TitleEditor.Text = string.Empty;
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

        public async void LoadHomeworks()
        {
            _homeworkList = await LoadHomeworksAsync();
            // Optionally, display the homework list
            // DisplayHomeworks();
        }

        private async void Add_Homework(string title, string text, DateTime dueDate)
        {
            var newHomework = new Homework { Id = _homeworkList.Count, Title = title, Text = text, DueDate = dueDate };
            _homeworkList.Homeworks.Add(newHomework);
            _homeworkList.Count = _homeworkList.Homeworks.Count;
            await SaveHomeworksAsync(_homeworkList);
        }
        private async void SaveHomework(string title, string text, DateTime dueDate, int id)
        {
            _homeworkList.Homeworks[id].Title = title;
            _homeworkList.Homeworks[id].Text = text;
            _homeworkList.Homeworks[id].DueDate = dueDate; // Update the due date
            await SaveHomeworksAsync(_homeworkList);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await SaveHomeworksAsync(_homeworkList);
        }
    }
}
