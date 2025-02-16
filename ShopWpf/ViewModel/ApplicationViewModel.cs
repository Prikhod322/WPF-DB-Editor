﻿using Microsoft.Win32;
using MvvmHelpers;
using ShopWpf.Logic;
using ShopWpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ShopWpf.ViewModel
{
    public class ApplicationViewModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableCollection<Developer> _Developers;
        private ObservableCollection<Game> _Games;
        private ObservableCollection<GameStats> _GamesStats;
        private ObservableCollection<Review> _Reviews;
        private ObservableCollection<User> _Users;

        private dynamic? _selectedItem;
        private dynamic? _menuItem;

        private Visibility _dataGridVisibility;
        private string _getRequestMessage;
        private Visibility _getRequestMessageVisibility;
        private Visibility _itemMenuVisibility;
        private string _putPostRequestMessage;
        private Visibility _putPostRequestMessageVisibility;
        private TabItem _selectedTabItem;
        private BitmapImage _openedImage;
        private bool _postOptionSelected;

        private string SelectedTable;

        #region Properties
        public ObservableCollection<Developer> Developers
        {
            get { return _Developers; }
            set
            {
                _Developers = value;
                OnPropertyChanged("Developers");
            }
        }
        public ObservableCollection<Game> Games
        {
            get { return _Games; }
            set
            {
                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        public ObservableCollection<GameStats> GamesStats
        {
            get { return _GamesStats; }
            set
            {
                _GamesStats = value;
                OnPropertyChanged("GamesStats");
            }
        }
        public ObservableCollection<Review> Reviews
        {
            get { return _Reviews; }
            set
            {
                _Reviews = value;
                OnPropertyChanged("Reviews");
            }
        }
        public ObservableCollection<User> Users
        {
            get { return _Users; }
            set
            {
                _Users = value;
                OnPropertyChanged("Users");
            }
        }
        public dynamic? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                if (_selectedItem != null)
                    MenuItem = CopyFromReferenceType(_selectedItem);

                OpenedImage = null;

                OnPropertyChanged("SelectedItem");
            }
        }

        public dynamic? MenuItem
        {
            get { return _menuItem; }
            set
            {
                _menuItem = value;
                OnPropertyChanged("MenuItem");
            }
        }

        public Visibility DataGridVisibility
        {
            get { return _dataGridVisibility; }
            set
            {
                _dataGridVisibility = value;
                OnPropertyChanged("DataGridVisibility");
            }
        }
        public string GetRequestMessage
        {
            get { return _getRequestMessage; }
            set
            {
                _getRequestMessage = value;
                OnPropertyChanged("GetRequestMessage");
            }
        }
        public Visibility GetRequestMessageVisibility
        {
            get { return _getRequestMessageVisibility; }
            set
            {
                _getRequestMessageVisibility = value;
                OnPropertyChanged("GetRequestMessageVisibility");
            }
        }
        public Visibility ItemMenuVisibility
        {
            get { return _itemMenuVisibility; }
            set
            {
                _itemMenuVisibility = value;
                OnPropertyChanged("ItemMenuVisibility");
            }
        }
        public string PutPostRequestMessage
        {
            get { return _putPostRequestMessage; }
            set
            {
                _putPostRequestMessage = value;
                OnPropertyChanged("PutPostRequestMessage");
            }
        }
        public Visibility PutPostRequestMessageVisibility
        {
            get { return _putPostRequestMessageVisibility; }
            set
            {
                _putPostRequestMessageVisibility = value;
                OnPropertyChanged("PutPostRequestMessageVisibility");
            }
        }
        public TabItem SelectedTabItem
        {
            get { return _selectedTabItem; }
            set
            {
                _selectedTabItem = value;
                SelectedTable = _selectedTabItem.Tag.ToString()!;
                OnPropertyChanged("SelectedTabItem");
            }
        }
        public BitmapImage OpenedImage
        {
            get { return _openedImage; }
            set
            {
                _openedImage = value;
                OnPropertyChanged("OpenedImage");
            }
        }
        public bool PostOptionSelected
        {
            get { return _postOptionSelected; }
            set
            {
                _postOptionSelected = value;
                OnPropertyChanged("PostOptionSelected");
            }
        }
        #endregion

        public ApplicationViewModel()
        {
            Init();
            HideTable();
            GetTable(TableNames.Developer);
        }

        public void Init()
        {
            ItemMenuVisibility = Visibility.Collapsed;
            PutPostRequestMessageVisibility = Visibility.Collapsed;
            MenuItem = new Developer();
        }

        public async void GetTable(string a = null)
        {
            HttpResponseMessage HttpResponse = await Requests.GetRequest(a ?? SelectedTable);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
            {
                DataGridVisibility = Visibility.Visible;
                GetRequestMessageVisibility = Visibility.Visible;
                GetRequestMessage = ($"Error: {(int)HttpResponse.StatusCode} ({HttpResponse.StatusCode})\n{await HttpResponse.Content.ReadAsStringAsync()}");
                return;
            }

            switch (SelectedTabItem.Tag)
            {
                case TableNames.Developer:
                    {
                        List<Developer> tmpList = new List<Developer>();
                        tmpList.AddRange(JsonSerializer.Deserialize<List<Developer>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        Developers = new ObservableCollection<Developer>();
                        foreach (var item in tmpList)
                        {
                            Developers.Add(item);
                        }
                        break;
                    }
                case TableNames.Game:
                    {
                        List<Game> tmpList = new List<Game>();
                        tmpList.AddRange(JsonSerializer.Deserialize<List<Game>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        Games = new ObservableCollection<Game>();
                        foreach (var item in tmpList)
                        {
                            Games.Add(item);
                        }
                        break;
                    }
                case TableNames.GameStats:
                    {
                        List<GameStats> tmpList = new List<GameStats>();
                        tmpList.AddRange(JsonSerializer.Deserialize<List<GameStats>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        GamesStats = new ObservableCollection<GameStats>();
                        foreach (var item in tmpList)
                        {
                            GamesStats.Add(item);
                        }
                        break;
                    }
                case TableNames.Review:
                    {
                        List<Review> tmpList = new List<Review>();
                        tmpList.AddRange(JsonSerializer.Deserialize<List<Review>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        Reviews = new ObservableCollection<Review>();
                        foreach (var item in tmpList)
                        {
                            Reviews.Add(item);
                        }
                        break;
                    }
                case TableNames.User:
                    {
                        List<User> tmpList = new List<User>();
                        tmpList.AddRange(JsonSerializer.Deserialize<List<User>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        Users = new ObservableCollection<User>();
                        foreach (var item in tmpList)
                        {
                            Users.Add(item);
                        }
                        break;
                    }
                default:
                    break;
            }
            ShowTable();
        }

        private void HideTable()
        {
            DataGridVisibility = Visibility.Collapsed;
            GetRequestMessageVisibility = Visibility.Visible;
            GetRequestMessage = "Loading...";
        }

        private void ShowTable()
        {
            GetRequestMessageVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;
        }

        public async Task DeleteSelectedItem()
        {
            await Requests.DeleteRequest(SelectedTable, SelectedItem.id);
        }

        public async Task PostNewItem()
        {
            MultipartFormDataContent? multipartContent = null;
            string content = string.Empty;
            HttpResponseMessage response;

            if (OpenedImage != null)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(OpenedImage)), "logo", "filename");
            }

            switch (SelectedTabItem.Tag)
            {
                case TableNames.Developer:
                    {
                        content = Convert.ToString(MenuItem.name);
                        break;
                    }
                case TableNames.Game:
                    {
                        content = $"{MenuItem.name}/{MenuItem.price}/{MenuItem.developerID}";
                        content += MenuItem.achievementsCount != null ? $"?achCount={MenuItem.achievementsCount}" : null;
                        break;
                    }
                case TableNames.GameStats:
                    {
                        content = $"{MenuItem.userID}/{MenuItem.gameID}";
                        content += MenuItem.achievementsGot != null ? $"?achGot={MenuItem.achievementsGot}" : null;
                        content += MenuItem.hoursPlayed != null ? $"?hoursPlayed={MenuItem.hoursPlayed}" : null;
                        break;
                    }
                case TableNames.Review:
                    {
                        content = $"{MenuItem.isPositive}/{MenuItem.userID}/{MenuItem.gameID}";
                        content += MenuItem.text != string.Empty ? $"?text={MenuItem.text}" : string.Empty;
                        break;
                    }
                case TableNames.User:
                    {
                        content = $"{MenuItem.login}/{MenuItem.passwordHash}/{MenuItem.nickname}";
                        content += MenuItem.email != string.Empty ? $"?email={MenuItem.email}" : string.Empty;
                        break;
                    }
                default:
                    break;
            }

            response = await Requests.PostRequest(SelectedTable, content, multipartContent);

            ShowRequestLog(response.StatusCode == HttpStatusCode.OK ? "Data Posted successfuly" :
                            $"Error: {(int)response.StatusCode} ({response.StatusCode})\n{await response.Content.ReadAsStringAsync()}");
        }

        public async Task UpdateSelectedItem()
        {
            Dictionary<string, object> content = new Dictionary<string, object>();
            MultipartFormDataContent? multipartContent = null;

            HttpResponseMessage requestResponse;
            string responseMessage = string.Empty;

            if (OpenedImage != null)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(OpenedImage)), "logo", "filename");
                content.Add(Routes.PutLogoRequest, null);
            }

            switch (SelectedTabItem.Tag.ToString())
            {
                case TableNames.Developer:
                    {
                        if (SelectedItem.name != MenuItem.name)
                            content.Add(Routes.PutNameRequest, MenuItem.name);
                        break;
                    }
                case TableNames.Game:
                    {
                        if (SelectedItem.name != MenuItem.name)
                            content.Add(Routes.PutNameRequest, MenuItem.name);
                        if (SelectedItem.price != MenuItem.price)
                            content.Add(Routes.PutPriceRequest, MenuItem.price);
                        if (SelectedItem.achievementsCount != MenuItem.achievementsCount)
                            content.Add(Routes.PutAchievementsCountRequest, MenuItem.achievementsCount);
                        if (SelectedItem.developerID != MenuItem.developerID)
                            content.Add(Routes.PutDeveloperRequest, MenuItem.developerID);
                        break;
                    }
                case TableNames.GameStats:
                    {
                        if (SelectedItem.userID != MenuItem.userID)
                            content.Add(Routes.PutUserRequest, MenuItem.userID);
                        if (SelectedItem.gameID != MenuItem.gameID)
                            content.Add(Routes.PutGameRequest, MenuItem.gameID);
                        if (SelectedItem.achievementsGot != MenuItem.achievementsGot)
                            content.Add(Routes.PutGottenAchievementsRequest, MenuItem.achievementsGot);
                        if (SelectedItem.hoursPlayed != MenuItem.hoursPlayed)
                            content.Add(Routes.PutHoursPlayedRequest, MenuItem.hoursPlayed);
                        break;
                    }
                case TableNames.Review:
                    {
                        if (SelectedItem.userID != MenuItem.userID)
                            content.Add(Routes.PutUserRequest, MenuItem.userID);
                        if (SelectedItem.gameID != MenuItem.gameID)
                            content.Add(Routes.PutGameRequest, MenuItem.gameID);
                        if (SelectedItem.isPositive != MenuItem.isPositive)
                            content.Add(Routes.PutApprovalRequest, MenuItem.isPositive);
                        if (SelectedItem.text != MenuItem.text)
                            content.Add(Routes.PutTextRequest, MenuItem.text);
                        break;
                    }
                case TableNames.User:
                    {
                        if (SelectedItem.email != MenuItem.email)
                            content.Add(Routes.PutEmailRequest, MenuItem.Text);
                        if (SelectedItem.nickname != MenuItem.nickname)
                            content.Add(Routes.PutNicknameRequest, MenuItem.nickname);
                        if (SelectedItem.login != MenuItem.login)
                            content.Add(Routes.PutLoginRequest, MenuItem.login);
                        if (SelectedItem.passwordHash != MenuItem.passwordHash)
                            content.Add(Routes.PutPasswordRequest, MenuItem.passwordHash);
                        break;
                    }
                default:
                    break;
            }

            if (content.Count == 0)
            {
                ShowRequestLog("No changes");
                return;
            }

            for (int i = 0; i < content.Count; i++)
            {
                if (content.ElementAt(i).Key == Routes.PutLogoRequest)
                    requestResponse = await Requests.PutRequest(SelectedTable, content.ElementAt(i).Key, MenuItem.id, null, multipartContent);
                else
                    requestResponse = await Requests.PutRequest(SelectedTable, content.ElementAt(i).Key, MenuItem.id, content.ElementAt(i).Value.ToString());

                if (requestResponse.StatusCode != HttpStatusCode.OK)
                {
                    responseMessage += $"Error: {(int)requestResponse.StatusCode} ({requestResponse.StatusCode})\n{await requestResponse.Content.ReadAsStringAsync()}\n\n";
                }
            }

            if (responseMessage == string.Empty)
            {
                ShowRequestLog("Data Updated successfuly");
            }
            else
                ShowRequestLog(responseMessage);
        }

        public byte[] ImageToHttpContent(BitmapImage img)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        private void ShowRequestLog(string errorText)
        {
            PutPostRequestMessageVisibility = Visibility.Visible;
            PutPostRequestMessage = errorText;
        }

        private dynamic CopyFromReferenceType(dynamic selectedItem)
        {
            switch (SelectedTabItem.Tag)
            {
                case TableNames.Developer:
                    {
                        return (dynamic)new Developer
                        {
                            id = selectedItem.id,
                            logoURL = selectedItem.logoURL,
                            name = selectedItem.name,
                            registrationDate = selectedItem.registrationDate
                        };
                    }
                case TableNames.Game:
                    {
                        return (dynamic)new Game
                        {
                            id = selectedItem.id,
                            logoURL = selectedItem.logoURL,
                            name = selectedItem.name,
                            price = selectedItem.price,
                            developerID = selectedItem.developerID,
                            achievementsCount = selectedItem.achievementsCount,
                            publishDate = selectedItem.publishDate
                        };
                    }
                case TableNames.GameStats:
                    {
                        return (dynamic)new GameStats
                        {
                            id = selectedItem.id,
                            userID = selectedItem.userID,
                            gameID = selectedItem.gameID,
                            hoursPlayed = selectedItem.hoursPlayed,
                            achievementsGot = selectedItem.achievementsGot,
                            purchaseDate = selectedItem.purchaseDate
                        };
                    }
                case TableNames.Review:
                    {
                        return (dynamic)new Review
                        {
                            id = selectedItem.id,
                            userID = selectedItem.userID,
                            gameID = selectedItem.gameID,
                            text = selectedItem.text,
                            isPositive = selectedItem.isPositive,
                            creationDate = selectedItem.creationDate,
                            lastEditDate = selectedItem.lastEditDate
                        };
                    }
                case TableNames.User:
                    {
                        return (dynamic)new User
                        {
                            id = selectedItem.id,
                            login = selectedItem.login,
                            avatarURL = selectedItem.avatarURL,
                            email = selectedItem.email,
                            passwordHash = selectedItem.passwordHash,
                            creationDate = selectedItem.creationDate,
                            nickname = selectedItem.nickame
                        };
                    }
                default:
                    return null;
            }

        }
        #region Commands
        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(async obj =>
                  {
                      HideTable();
                      await DeleteSelectedItem();
                      GetTable();
                  }));
            }
        }

        private RelayCommand postCommand;
        public RelayCommand PostCommand
        {
            get
            {
                return postCommand ??
                  (postCommand = new RelayCommand(obj =>
                  {
                      PostOptionSelected = true;
                      ItemMenuVisibility = Visibility.Visible;

                      switch (SelectedTabItem.Tag)
                      {
                          case TableNames.Developer:
                              {
                                  MenuItem = new Developer();
                                  break;
                              }
                          case TableNames.Game:
                              {
                                  MenuItem = new Game();
                                  break;
                              }
                          case TableNames.GameStats:
                              {
                                  MenuItem = new GameStats();
                                  break;
                              }
                          case TableNames.Review:
                              {
                                  MenuItem = new Review();
                                  break;
                              }
                          case TableNames.User:
                              {
                                  MenuItem = new User();
                                  break;
                              }
                          default:
                              break;
                      }
                  }));
            }
        }

        private RelayCommand putCommand;
        public RelayCommand PutCommand
        {
            get
            {
                return putCommand ??
                  (putCommand = new RelayCommand(obj =>
                  {
                      PostOptionSelected = false;
                      ItemMenuVisibility = Visibility.Visible;
                  }));
            }
        }

        private RelayCommand refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return refreshCommand ??
                  (refreshCommand = new RelayCommand(obj =>
                  {
                      HideTable();
                      GetTable();
                  }));
            }
        }

        private RelayCommand closeItemMenuCommand;
        public RelayCommand CloseItemMenuCommand
        {
            get
            {
                return closeItemMenuCommand ??
                  (closeItemMenuCommand = new RelayCommand(obj =>
                  {
                      ItemMenuVisibility = Visibility.Collapsed;
                  }));
            }
        }

        private RelayCommand postItemCommand;
        public RelayCommand PostItemCommand
        {
            get
            {
                return postItemCommand ??
                  (postItemCommand = new RelayCommand(async obj =>
                  {
                      HideTable();
                      await PostNewItem();
                      GetTable();

                  }));
            }
        }

        private RelayCommand updateItemCommand;
        public RelayCommand UpdateItemCommand
        {
            get
            {
                return updateItemCommand ??
                  (updateItemCommand = new RelayCommand(async obj =>
                  {
                      HideTable();
                      await UpdateSelectedItem();
                      GetTable();

                  }));
            }
        }

        private RelayCommand openImageFromFileCommand;
        public RelayCommand OpenImageFromFileCommand
        {
            get
            {
                return openImageFromFileCommand ??
                  (openImageFromFileCommand = new RelayCommand(async obj =>
                  {
                      OpenFileDialog openFileDialogLoad = new OpenFileDialog();

                      if (openFileDialogLoad.ShowDialog() == true)
                      {
                          OpenedImage = new BitmapImage(new Uri(openFileDialogLoad.FileName));
                      }
                  }));
            }
        }

        private RelayCommand tabChangedCommand;
        public RelayCommand TabChangedCommand
        {
            get
            {
                return tabChangedCommand ??
                  (tabChangedCommand = new RelayCommand(async obj =>
                  {
                      Developers = null;
                      Games = null;
                      GamesStats = null;
                      Reviews = null;
                      Users = null;

                      OpenedImage = null;

                      GetRequestMessageVisibility = Visibility.Visible;
                      GetRequestMessage = "Loading...";
                      ItemMenuVisibility = Visibility.Collapsed;

                      GetTable();
                  }));
            }
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableRangeCollection<Developer> editedOrRemovedItems = new ObservableRangeCollection<Developer>();
            foreach (Developer newItem in e.NewItems)
            {
                editedOrRemovedItems.Add(newItem);
            }

            foreach (Developer oldItem in e.OldItems)
            {
                editedOrRemovedItems.Add(oldItem);
            }

            NotifyCollectionChangedAction action = e.Action;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
