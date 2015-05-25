using Test.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WebSocketSharp;

namespace Test
{
    class UnTypedActionContainer
    {
        public Type ActionType { get; set; }
        public Action<Object> Action { get; set; }
    };

    class SignalRClient
    {
        private WebSocket _ws;
        private string _connectionToken;
        private Dictionary<string, UnTypedActionContainer> _actionMap;
        private String _hubName = "Message";
        public SignalRClient()
        {
            _actionMap = new Dictionary<string, UnTypedActionContainer>();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://52.74.15.79/response/negotiate?connectionData=%5B%7B%22name%22%3A%22Message%22%7D%5D&clientProtocol=1.3&_=1408716619953");
            var response = (HttpWebResponse)webRequest.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                _connectionToken = Uri.EscapeDataString(JsonConvert.DeserializeObject<NegotiateResponse>(sr.ReadToEnd()).ConnectionToken);
            }
        }

        public void Open()
        {
            if (_ws == null)
            {
                _ws = new WebSocket("ws://52.74.15.79//response/connect?transport=webSockets&connectionToken=" + _connectionToken + "&connectionData=%5B%7B%22name%22%3A%22" + _hubName + "%22%7D%5D");
            }
            else
            {
                _ws = new WebSocket("ws://52.74.15.79//response/reconnect?transport=webSockets&connectionToken=" + _connectionToken + "&connectionData=%5B%7B%22name%22%3A%22" + _hubName + "%22%7D%5D");
            }
            AttachAndConnect();
        }

        private void AttachAndConnect()
        {
            _ws.OnClose += _ws_OnClose;

            _ws.OnError += _ws_OnError;

            _ws.OnMessage += _ws_OnMessage;

            _ws.OnOpen += _ws_OnOpen;

            _ws.Connect();
        }

        void _ws_OnOpen(object sender, EventArgs e)
        {
            Console.Write("OnOpen: ");
            Console.WriteLine("Opened Connection");            
        }

        void _ws_OnMessage(object sender, MessageEventArgs e)
        {
            
                Console.WriteLine(e.RawData);
                //var deviceDataWrapper = JsonConvert.DeserializeObject<MessageWrapper>(e.Data).M[0];
                //_actionMap[deviceDataWrapper.M].Action(deviceDataWrapper.A[0]);
            
        }

        void _ws_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Console.Write("OnError: ");
            Console.WriteLine(e.Message);

        }

        void _ws_OnClose(object sender, CloseEventArgs e)
        {
            Console.Write("OnClose: ");
            Console.WriteLine(e.Reason + " Code: " + e.Code + " WasClean: " + e.WasClean);
        }

        public void On<T>(string method, Action<T> callback) where T : class
        {
            _actionMap.Add(method, new UnTypedActionContainer
            {
                Action = new Action<object>(x =>
                {
                    callback(x as T);
                }),
                ActionType = typeof(T)
            });
        }
    }
}
