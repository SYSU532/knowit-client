﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace knowit
{
    class NetworkControl
    {
        public static string AccessingURI = "chat.chenmt.science";
        private const string httpsPrefix = "https://";
        private const string wssPrefix = "wss://";
        private const string wssPostfix = "/wss";
        public static string username;

        public static string accessName { get { return httpsPrefix + AccessingURI; } }

        public static MessageWebSocket ws;

        public static void WebSocket_ReceivedMessage(MessageWebSocket ws, MessageWebSocketMessageReceivedEventArgs args)
        {

            DataReader reader = args.GetDataReader();
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            string result = reader.ReadString(reader.UnconsumedBufferLength);
            string username = result.Substring(0, result.IndexOf(' '));
            string content = result.Substring(result.IndexOf(' ') + 1, result.Length - 1 - result.IndexOf(' '));
            //Here, you need to put receive message to the chat room view model.
            Debug.WriteLine(result);
            ChatWindowViewModel myChatViewModel = ChatWindowViewModel.GetInstance();
            if(username == NetworkControl.username)
            {
                myChatViewModel.AddMessageSelf(username, content);
            }
            else
            {
                myChatViewModel.AddMessage(username, content);
            }
        }

        public static async Task SendChatMessage(string username, string password, string message)
        {
            NetworkControl.username = username;
            try
            {
                //Start to send messages
                DataWriter writer = new DataWriter(ws.OutputStream);
                writer.WriteString(username + " " + password + " " + message);
                await writer.StoreAsync();
            }
            catch
            {
                //Connection error
            }
        }
        public static async Task InitialWebSocket()
        {
            ws = new MessageWebSocket();
            ws.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;
            ws.MessageReceived += WebSocket_ReceivedMessage;
            await ws.ConnectAsync(new Uri(wssPrefix + AccessingURI + wssPostfix));
        }
        public static void closeWs()
        {
            ws = null;
        }
        public static async Task<Dictionary<string, string>> QueryUserInfo(string userName)
        {
            Dictionary<String, String> requestData = new Dictionary<string, string>
            {
                {"name", userName}
            };

            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/getUserInfo", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseUserInfoJSON(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        public static string GetFullPathUrl(string fileName)
        {
            string fullPath = accessName + "/img/" + fileName;
            return fullPath;
        }

        private static Dictionary<string, string> parseUserInfoJSON(string JSONStr)
        {
            var json = JsonObject.Parse(JSONStr);
            var returnCode = json.GetNamedNumber("code");
            if (returnCode == 0)
            {
                var errMsg = json.GetNamedString("errMessage");
                return new Dictionary<string, string>
                {
                    {"code", "0"},
                    {"errMessage", errMsg }
                };
            }
            else if (returnCode == 1)
            {
                var username = json.GetNamedString("username");
                var email = json.GetNamedString("email");
                var imageUrl = json.GetNamedString("imageUrl");
                var phone = json.GetNamedString("phone");
                return new Dictionary<string, string>
                {
                    {"code", "1"},
                    {"username", username },
                    {"email", email},
                    {"phone", phone},
                    {"imageUrl", imageUrl}
                };
            }
            //Unexpected Return value 
            return new Dictionary<string, string>
            {
                {"code", "-2"}
            };
        }

        public static async Task<Dictionary<string, string>> AttemptSignin(string username, string password)
        {
            var requestData = new Dictionary<string, string>
            {
                {"name", username},
                {"pass", password}
            };
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/login", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseBinaryResponse(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        public static async Task<Dictionary<string, object>> GetPostFromID(string username, string password, string postID)
        {
            var requestData = new Dictionary<string, string>
            {
                {"name", username},
                {"pass", password},
                {"id", postID }
            };
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/data", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parsePostInfo(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, object>
                {
                    {"code", "-1"}
                };
            }
        }

        private static Dictionary<string, object> parsePostInfo(string JSONStr)
        {
            JsonObject json = JsonObject.Parse(JSONStr);
            var returnCode = json.GetNamedNumber("code");
            if (returnCode == 0)
            {
                var errMsg = json.GetNamedString("errMessage");
                return new Dictionary<string, object>
                {
                    {"code", "0"},
                    {"errMessage", errMsg }
                };
            }
            else if (returnCode == 1)
            {
                var content = json.GetNamedString("content");
                var media = json.GetNamedString("media");
                var image = json.GetNamedString("image");
                var thumbs = json.GetNamedNumber("thumbs");
                var editor = json.GetNamedString("editor");
                var title = json.GetNamedString("title");
                JsonObject comments = json.GetNamedObject("comments");
                Dictionary<string, object> res = new Dictionary<string, object>
                {
                    {"code", "1"},
                    {"content", content },
                    {"image", image },
                    {"media", media },
                    {"thumbs", thumbs.ToString() },
                    {"editor", editor },
                    {"title", title }
                };
                
                List<KeyValuePair<String, String>> commentDict = new List<KeyValuePair<string, string>>();
                for(int i = 0; i < comments.Count; i++)
                {
                    var com = comments.GetNamedObject(i.ToString());
                    foreach(var pair in com)
                    {
                        commentDict.Add(new KeyValuePair<string, string>(pair.Key, pair.Value.GetString()));
                    }
                }
                res.Add("comment", commentDict);
                return res;
            }
            //Unexpected Return value 
            return new Dictionary<string, object>
            {
                {"code", "-2"}
            };

        }

        public static PostItem PostDictToPostItem(Dictionary<string, object> postDict, string id)
        {
            if ((string)postDict["code"] != "1") return null;
            string content = (string)postDict["content"];
            string imageURL = (string)postDict["image"];
            string videoURL = (string)postDict["media"];
            int thumbs = Convert.ToInt32((string)postDict["thumbs"]);
            string title = (string)postDict["title"];
            string editor = (string)postDict["editor"];
            return new PostItem(id, content, title, editor, imageURL, videoURL, thumbs,
                            (List<KeyValuePair<String, String>>)postDict["comment"]);
        }

        public static async Task<ObservableCollection<PostItem>> GetPostCollection(string username, string password)
        {
            var idList = await GetAllPostIDs();
            if (idList == null) return null;
            ObservableCollection<PostItem> collection = new ObservableCollection<PostItem>();
            foreach (var id in idList)
            {
                var postDict = await GetPostFromID(username, password, id);
                PostItem item = PostDictToPostItem(postDict, id);
                if (item == null)
                {
                    Debug.WriteLine("Server Error!");
                    continue;
                }
                collection.Add(item);
            }
            return collection;
        }

        public static async Task<List<string>> GetAllPostIDs()
        {
            Dictionary<String, String> requestData = new Dictionary<string, string>();

            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/allPostID", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parsePostIDs(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return null;
            }
        }

        private static List<String> parsePostIDs(string JSONStr)
        {
            try
            {
                var json = JsonObject.Parse(JSONStr);
                List<String> li = new List<string>();
                var idArray = json.GetNamedArray("postIDs");
                foreach (var item in idArray)
                {
                    li.Add(item.ToString());
                }
                return li;
            }
            catch
            {
                //Unexpected Return value 
                return null;
            }
        }

        public static async Task<Boolean> CheckUserThumbOrNot(string username, string id)
        {
            var requestData = new Dictionary<string, string>
            {
                {"name", username},
                {"id", id }
            };

            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/checkThumb", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var json = JsonObject.Parse(responseString);
                var res = json.GetNamedNumber("haveThumb");
                if (res == 1)
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                //Connection error
                return false;
            }
        }
        public static async Task<Dictionary<string, string>> GiveThumbToPost(string username, string password, string postID)
        {
            var requestData = new Dictionary<string, string>
            {
                {"name", username},
                {"pass", password},
                {"postID", postID }
            };
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/thumbUp", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseBinaryResponse(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        private static Dictionary<string, string> parseBinaryResponse(string JSONStr)
        {
            var json = JsonObject.Parse(JSONStr);
            var returnCode = json.GetNamedNumber("code");
            if (returnCode == 0)
            {
                var errMsg = json.GetNamedString("errMessage");
                return new Dictionary<string, string>
                {
                    {"code", "0"},
                    {"errMessage", errMsg }
                };
            }
            else if (returnCode == 1)
            {
                return new Dictionary<string, string>
                {
                    {"code", "1"}
                };
            }
            //Unexpected Return value 
            return new Dictionary<string, string>
            {
                {"code", "-2"}
            };
        }


        public static async Task<Dictionary<string, string>> CancelThumbToPost(string username, string password, string postID)
        {
            var requestData = new Dictionary<string, string>
            {
                {"name", username},
                {"pass", password},
                {"postID", postID }
            };
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/thumbDown", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseBinaryResponse(responseString);

                return parseResult;

            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        public static async Task<Dictionary<string, string>> PostComment(string username, string password, string postID, string content)
        {
            var requestData = new Dictionary<string, string>
            {
                {"name", username},
                {"pass", password},
                {"postID", postID },
                {"content", content }
            };
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(accessName + "/commentUp", new FormUrlEncodedContent(requestData));

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseBinaryResponse(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        public static async Task<Dictionary<string, string>> AttemptSignUp(string username, string password, string repass,
                                                                            string phone, string email, StorageFile avatar)
        {
            HttpClient client = new HttpClient();

            Stream fileStream = await avatar.OpenStreamForReadAsync();
            var buffer = new byte[(int)fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);

            ByteArrayContent content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            string imgType = avatar.FileType;

            StringBuilder builder = new StringBuilder();
            builder.Append(accessName);
            builder.Append("/logup?name=");
            builder.Append(username);
            builder.Append("&pass=");
            builder.Append(password);
            builder.Append("&rePass=");
            builder.Append(repass);
            builder.Append("&phone=");
            builder.Append(phone);
            builder.Append("&email=");
            builder.Append(email);
            builder.Append("&imageType=");
            builder.Append(imgType.ToLower());
            string url = builder.ToString();

            try
            {
                var response = await client.PostAsync(url, content);

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseBinaryResponse(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        private static List<String> imagePostfixes = new List<String> { ".jpg", ".jpeg", ".bmp", ".png", ".gif", ".tiff" };
        private static List<String> videoPostfixes = new List<String> { ".mp4", ".wmv", ".mkv", ".avi" };

        public static async Task<Dictionary<string, string>> PublishPost(string username, string password, string title,
                                                                            string passage, StorageFile media)
        {
            HttpClient client = new HttpClient();
            byte[] buffer;
            string fileType = "";
            if (media != null && (imagePostfixes.Contains(media.FileType.ToLower()) ||
                videoPostfixes.Contains(media.FileType.ToLower())))
            {
                Stream fileStream = await media.OpenStreamForReadAsync();
                buffer = new byte[(int)fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileType = media.FileType.ToLower();
            }
            else
            {
                buffer = new byte[1];
                buffer[0] = 0;
            }

            ByteArrayContent content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            bool isImage = imagePostfixes.Contains(fileType);
            bool isVideo = videoPostfixes.Contains(fileType);

            StringBuilder builder = new StringBuilder();
            builder.Append(accessName);
            builder.Append("/upload?name=");
            builder.Append(username);
            builder.Append("&pass=");
            builder.Append(password);
            builder.Append("&title=");
            builder.Append(title);
            builder.Append("&content=");
            builder.Append(passage);
            builder.Append("&imageType=");
            builder.Append(isImage ? fileType.ToLower() : "");
            builder.Append("&mediaType=");
            builder.Append(isVideo ? fileType.ToLower() : "");
            string url = builder.ToString();

            try
            {
                var response = await client.PostAsync(url, content);

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseBinaryResponse(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        public static async Task<Dictionary<string, string>> UserInfoModify(string username, string password, string phone,
                                                                            string email, StorageFile avatar)
        {
            HttpClient client = new HttpClient();
            byte[] buffer;
            string imgType = "";
            if (avatar != null)
            {
                Stream fileStream = await avatar.OpenStreamForReadAsync();
                buffer = new byte[(int)fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                imgType = avatar.FileType;
            }
            else
            {
                buffer = new byte[1];
                buffer[0] = 0;
            }
            ByteArrayContent content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


            StringBuilder builder = new StringBuilder();
            builder.Append(accessName);
            builder.Append("/modifyUserInfo?name=");
            builder.Append(username);
            builder.Append("&pass=");
            builder.Append(password);
            builder.Append("&newPhone=");
            builder.Append(phone);
            builder.Append("&newEmail=");
            builder.Append(email);
            builder.Append("&imgType=");
            builder.Append(imgType.ToLower());
            string url = builder.ToString();

            try
            {
                var response = await client.PostAsync(url, content);

                string responseString = await response.Content.ReadAsStringAsync();

                var parseResult = parseBinaryResponse(responseString);

                return parseResult;
            }
            catch
            {
                //Connection error
                return new Dictionary<String, String>
                {
                    {"code", "-1"}
                };
            }
        }

        public static async void upload()
        {
            var fop = new FileOpenPicker();
            fop.FileTypeFilter.Add(".jpg");
            fop.FileTypeFilter.Add(".jpeg");
            fop.FileTypeFilter.Add(".png");
            fop.FileTypeFilter.Add(".mp4");
            fop.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var file = await fop.PickSingleFileAsync();
            if (file != null)
            {

                Stream fileStream = await file.OpenStreamForReadAsync();
                HttpClient client = new HttpClient();

                var buffer = new byte[(int)fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);


                ByteArrayContent content = new ByteArrayContent(buffer);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


                var response = await client.PostAsync("http://127.0.0.1:3001/upload?aaa=123&bb=11", content);
                var alert = new MessageDialog(response.ToString());
                await alert.ShowAsync();
            }
        }

    }
}
