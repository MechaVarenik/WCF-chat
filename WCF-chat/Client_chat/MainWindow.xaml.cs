using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Client_chat.ServiceChat;
using System.Data.Entity;
using WCF_chat;

namespace Client_chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ServiceChat.IServiceChatCallback
    {
        UserContext db;
        ServiceChatClient client;
        int Id;
        bool isConnected = false;
        public Brush Red { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            db = new UserContext();
            db.Users.Load();
            usersGrid.ItemsSource = db.Users.Local.ToBindingList();
            client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
        }

        //авторизация
        private void Open_Login_Screen(object sender, RoutedEventArgs e)
        {
            UserAutorisation autorisationWindow = new UserAutorisation();
            if (autorisationWindow.ShowDialog() == true)
            {
                string Nickname = autorisationWindow.tbNickName.Text;
                string Password = autorisationWindow.tbPassword.Password;
                if (autorisationWindow.tbNickName.Text != "" && autorisationWindow.tbPassword.Password != "")
                {
                    bool result = client.Check_User(Nickname, Password);
                    if (result == false) MessageBox.Show("User does not exist. Try another login or password", 
                        "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        tblockUserName.Text = Nickname;
                        tblockUserName.Foreground = Brushes.Red;
                    }
                }
                else MessageBox.Show("Username and password fields can not be empty!", 
                    "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        //регистрация
        private void Registration_of_User(object sender, RoutedEventArgs e)
        {
            UserRegistration registrationWindow = new UserRegistration();
            if (registrationWindow.ShowDialog() == true)
            {
                string Nickname = registrationWindow.tbNickName.Text;
                string Password = registrationWindow.tbPassword.Password;
                if (registrationWindow.tbNickName.Text != "" && registrationWindow.tbPassword.Password != "")
                {
                    bool result = client.InsertUser(Nickname, Password);
                    if (result == false) MessageBox.Show("Nickname "+ Nickname + " already exists", 
                        "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        MessageBox.Show("Registration was a success. Welcome to WCF_Chat!",
                            "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                        tblockUserName.Text = Nickname;
                        tblockUserName.Foreground = Brushes.Red;
                        db.Users.Load();
                        usersGrid.ItemsSource = db.Users.Local.ToBindingList();
                    }
                }
                else MessageBox.Show("Username and password fields can not be empty!", 
                    "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //подключение к серверу
        void ConnectUser()
        {
            if (!isConnected)
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                PrintUsers();
                Id = client.Connect(tblockUserName.Text);
                client.SendOnlineUsers(tbMessage.Text, Id);
                tblockUserName.Foreground = Brushes.Green;
                isConnected = true;
            }
        }

        //отключение от сервера
        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(Id);
                client = null;
                tblockUserName.IsEnabled = true;
                lbUsersList.Items.Clear();
                tblockUserName.Foreground = Brushes.Red;
                isConnected = false;
            }
        }

        //кнопка подключения
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ConnectUser();
        }

        //кнопка отключения
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();
            }
        }

        //ф-я обратного вызова отправки сообщений всем клиентам
        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
        }

        //ф-я обратного вызова демонстрации пользователей в сети
        public void UsersCallback(string name)
        {
            lbUsersList.Items.Add(name);
        }

        //пользватели онлайн
        public void PrintUsers()
        {
            foreach (var us in client.GetOnlineUsers())
            {
                lbUsersList.Items.Add(us);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(tbMessage.Text, Id);
                    tbMessage.Text = string.Empty;
                }
            }
        }

        private void SendMsg_Click(object sender, RoutedEventArgs e)
        {
            client.SendMsg(tbMessage.Text, Id);
            tbMessage.Text = string.Empty;
        }

    }
    }
