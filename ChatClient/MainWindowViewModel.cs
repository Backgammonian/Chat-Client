using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using Meziantou.Framework.WPF.Collections;
using Shared;
using ChatMessages;
using InputBox;

namespace ChatClient
{
    public class MainWindowViewModel : ObservableObject
    {
        private Client _client;
        private string _nickname;
        private Room _selectedRoom;
        private bool _nicknameUpdatedStatus;
        private readonly DispatcherTimer _statusTimer;

        public MainWindowViewModel()
        {
            _statusTimer = new DispatcherTimer();
            _statusTimer.Interval = new TimeSpan(0, 0, 2);
            _statusTimer.Tick += (s, e) => NicknameUpdatedStatus = false;

            ID = RandomGenerator.GetRandomString(30);
            Nickname = "UserName";
            NicknameUpdatedStatus = false;
            OnPropertyChanged(nameof(ServerAddress));
            RoomsList = new ConcurrentObservableCollection<Room>();
            RoomMessages = new ConcurrentObservableCollection<Message>();
            ClientsListOfServer = new ConcurrentObservableCollection<ClientOfServer>();

            ConnectToNewServerCommand = new AsyncRelayCommand(ConnectToServer);
            SendMessageCommand = new AsyncRelayCommand(SendMessage);

            NewServerAddress = NetworkUtils.GetLocalIPAddress().ToString() + ":55000";
        }

        public ICommand ConnectToNewServerCommand { get; }
        public ICommand SendMessageCommand { get; }
        public string ServerAddress => _client != null ? _client.ServerAddress.ToString() : "--";
        public string NewServerAddress { get; set; }
        public string MessageText { get; set; }
        public string ID { get; }
        public ConcurrentObservableCollection<Room> RoomsList { get; }
        public ConcurrentObservableCollection<Message> RoomMessages { get; }
        public ConcurrentObservableCollection<ClientOfServer> ClientsListOfServer { get; }

        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                SetProperty(ref _selectedRoom, value);

                if (SelectedRoom != null)
                {
                    var message = new RequestAllMessagesInRoomPackage(SelectedRoom.ID);

                    Task.Run(() => _client.Send(message.GetByteArray()));
                }
            }
        }
        
        public string Nickname
        {
            get => _nickname;
            set
            {
                if (!string.IsNullOrEmpty(value) &&
                    !string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _nickname, value);

                    NicknameUpdatedStatus = true;

                    if (_client != null)
                    {
                        var message = new ClientsNewNicknamePackage(ID, Nickname);

                        Task.Run(() => _client.Send(message.GetByteArray()));
                    }
                }
            }
        }

        public bool NicknameUpdatedStatus
        {
            get => _nicknameUpdatedStatus;
            private set
            {
                SetProperty(ref _nicknameUpdatedStatus, value);

                if (NicknameUpdatedStatus)
                {
                    _statusTimer.Stop();
                    _statusTimer.Start();
                }
            }
        }

        private void OnDataReceived(object sender, NetworkDataReceivedEventArgs e)
        {
            if (e.Data.Length == 0)
            {
                return;
            }

            var dataReader = new SimpleReader(e.Data);
            var type = (PackageTypes)dataReader.GetByte();
            var json = dataReader.GetString();

            switch (type)
            {
                case PackageTypes.ListOfRooms:
                    Debug.WriteLine("ListOfRooms");

                    SelectedRoom = null;
                    RoomsList.Clear();
                    RoomMessages.Clear();
                    var roomsList = JsonConvert.DeserializeObject<List<(string, string)>>(json);
                    foreach (var room in roomsList)
                    {
                        RoomsList.Add(new Room(room.Item1, room.Item2));
                    }
                    break;

                case PackageTypes.ResponseAllMessagesInRoom:
                    Debug.WriteLine("ResponseAllMessagesInRoom");

                    RoomMessages.Clear();
                    var messagesInRoom = JsonConvert.DeserializeObject<(string, List<Message>)>(json);
                    if (RoomsList.Any(room => room.ID == messagesInRoom.Item1))
                    {
                        foreach (var message in messagesInRoom.Item2)
                        {
                            Debug.WriteLine("Added message from " + message.AuthorsNickname);

                            RoomMessages.Add(message);
                        }
                    }
                    break;

                case PackageTypes.NewMessageInRoom:
                    Debug.WriteLine("NewMessageInRoom");

                    var newMessageInRoom = JsonConvert.DeserializeObject<(Message, string)>(json);
                    if (SelectedRoom.ID == newMessageInRoom.Item2)
                    {
                        RoomMessages.Add(newMessageInRoom.Item1);
                    }
                    break;

                case PackageTypes.ClientsListUpdated:
                    Debug.WriteLine("ClientsListUpdated");

                    ClientsListOfServer.Clear();
                    var clientsOfServer = JsonConvert.DeserializeObject<List<(string, string, string)>>(json);
                    foreach (var client in clientsOfServer)
                    {
                        if (client.Item2 != ID)
                        {
                            ClientsListOfServer.Add(new ClientOfServer(client.Item1, client.Item2, client.Item3));
                        }
                    }
                    break;

                default:
                    Debug.WriteLine("Unknown type: " + type);
                    break;
            }
        }

        private async Task ConnectToServer()
        {
            if (!IPEndPoint.TryParse(NewServerAddress, out IPEndPoint endPoint))
            {
                return;
            }

            if (_client != null)
            {
                _client.Stop();
                _client.DataReceived -= OnDataReceived;
            }

            _client = new Client();
            _client.DataReceived += OnDataReceived;

            var connectionStatus = await _client.Connect(endPoint);
            if (connectionStatus)
            {
                _client.StartListen();
                OnPropertyChanged(nameof(ServerAddress));

                var hello = new ClientHelloPackage(ID, Nickname);
                await _client.Send(hello.GetByteArray());
            }
        }

        private async Task SendMessage()
        {
            if (SelectedRoom == null)
            {
                return;
            }

            Debug.WriteLine("(SendMessage)");

            var userMessage = new Message(MessageText, Nickname, ID, DateTime.Now);
            var message = new MessageToRoomPackage(userMessage, SelectedRoom.ID);
            await _client.Send(message.GetByteArray());
        }

        public void AskNickname()
        {
            var inputBox = new InputBoxUtils();
            var newNickname = string.Empty;
            if (inputBox.AskNickname(Nickname, out newNickname))
            {
                Nickname = newNickname;
            }
        }

        public async void CloseClient()
        {
            if (_client != null)
            {
                var disconnectMessage = new ClientDisconnectPackage();
                await _client.Send(disconnectMessage.GetByteArray());

                _client.Stop();
            }
        }
    }
}
