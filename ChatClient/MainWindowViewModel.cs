using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using Meziantou.Framework.WPF.Collections;
using Shared;
using ChatMessages;

namespace ChatClient
{
    public class MainWindowViewModel : ObservableObject
    {
        private Client _client;
        private string _nickname;
        private Room _selectedRoom;

        public MainWindowViewModel()
        {
            ID = RandomGenerator.GetRandomString(30);
            Nickname = "UserName";
            OnPropertyChanged(nameof(ServerAddress));
            RoomsList = new ConcurrentObservableCollection<Room>();
            RoomMessages = new ConcurrentObservableCollection<Message>();
            ConnectToNewServerCommand = new AsyncRelayCommand(ConnectToServer);
            SendMessageCommand = new AsyncRelayCommand(SendMessage);

            NewServerAddress = "192.168.0.14:55000";
        }

        public ICommand ConnectToNewServerCommand { get; }
        public ICommand SendMessageCommand { get; }
        public string ServerAddress => _client != null ? _client.ServerAddress.ToString() : "--";
        public string NewServerAddress { get; set; }
        public string MessageText { get; set; }
        public string ID { get; }
        public ConcurrentObservableCollection<Room> RoomsList { get; }
        public ConcurrentObservableCollection<Message> RoomMessages { get; }

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

                    if (_client != null)
                    {
                        var message = new ClientsNicknamePackage(ID, Nickname);

                        Task.Run(() => _client.Send(message.GetByteArray()));
                    }
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

                var message = new ClientsNicknamePackage(ID, Nickname);

                await _client.Send(message.GetByteArray());
            }
        }

        private async Task SendMessage()
        {
            if (SelectedRoom == null)
            {
                return;
            }

            var userMessage = new Message(MessageText, Nickname, ID, DateTime.Now);

            var message = new MessageToRoomPackage(userMessage, SelectedRoom.ID);

            await _client.Send(message.GetByteArray());
        }
    }
}
