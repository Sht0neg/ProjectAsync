using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string key = "sk-or-vv-0dfbe36091f6cb5989322569d3878d26021399798fa8f09be885b1803e747c9c";
        string url = "https://api.vsegpt.ru/v1/";

        HttpClient client = new HttpClient();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonS.IsEnabled = false;
            FirstAnswer.Text = "Загрузка...";
            SecondAnswer.Text = "Загрузка...";
            ThirdAnswer.Text = "Загрузка...";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            string promt = "Напиши отзыв о студенте," +
                $"Имя: {FirstName.Text} " + $"Фамилия: {SecondName.Text} " +
                $"Краткое описание студента: {Description.Text}";

            List<dynamic> messages = new List<dynamic>();
            messages.Add(new { role = "user", content = promt });
            var requestData = new
            {
                model = "openai/gpt-3.5-turbo",
                messages = messages,
                temperature = 0.7,
                n = 1,
                max_tokens = 3000
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response1 = await client.PostAsync(url + "chat/completions", content);
            var response2 = await client.PostAsync(url + "chat/completions", content);
            var response3 = await client.PostAsync(url + "chat/completions", content);

            if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
            {
                var jsonResponse1 = await response1.Content.ReadAsStringAsync();
                var jsonResponse2 = await response2.Content.ReadAsStringAsync();
                var jsonResponse3 = await response3.Content.ReadAsStringAsync();
                dynamic responseData1 = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse1);
                dynamic responseData2 = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse2);
                dynamic responseData3 = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse3);
                FirstAnswer.Text = responseData1.choices[0].message.content;
                SecondAnswer.Text = responseData2.choices[0].message.content;
                ThirdAnswer.Text = responseData3.choices[0].message.content;
            }
            else
            {
                FirstAnswer.Text = "Ошибка: " + response1.ReasonPhrase;
                SecondAnswer.Text = "Ошибка: " + response2.ReasonPhrase;
                ThirdAnswer.Text = "Ошибка: " + response3.ReasonPhrase;
            }

            ButtonS.IsEnabled = true;
        }
    }
}