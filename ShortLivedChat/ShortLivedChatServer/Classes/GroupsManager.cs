using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using ShortLivedChatServer.Hubs;

namespace ShortLivedChatServer.Classes
{
    /// <summary>
    /// A singleton class that is responsible for managing groups.
    /// </summary>
    public class GroupsManager
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private string _currentGroup;
        private readonly Dictionary<string, bool> _rooms = new Dictionary<string, bool>
        {
            { "Alfheim",false},
            { "Agartha",false},
            { "Alomkik",false},
            { "Annwn",false},
            { "Arcadia",false},
            { "Asgard",false},
            { "Asphodel",false},
            { "Atlantis",false},
            { "Avalon",false},
            { "Axis Mundi",false},
            { "Ayotha",false},
            { "Aztlán",false},
            { "Baltia",false},
            { "Biarmaland",false},
            { "Biringan city",false},
            { "Brahmapura",false},
            { "Hy-Brasil",false},
            { "Brittia",false},
            { "Camelot",false},
            { "City of the Caesars",false},
            { "Cloud cuckoo land",false},
            { "Cockaigne",false},
            { "Diyu",false},
            { "Dinas Affaraon/Ffaraon",false},
            { "El Dorado",false},
            { "Elysian Fields",false},
            { "Feather Mountain",false},
            { "Gorias",false},
            { "Finias",false},
            { "Murias",false},
            { "Falias",false},
            { "Garden of the Hesperides",false},
            { "Garden of Eden",false},
            { "Hawaiki",false},
            { "Heaven",false},
            { "Hel(heimr)",false},
            { "Hell",false},
            { "Hyperborea",false},
            { "Irkalla",false},
            { "Islands of the Blessed",false},
            { "Jabulqa",false},
            { "Jabulsa",false},
            { "Jotunheim",false},
            { "Kingdom of Reynes",false},
            { "Kingdom of Saguenay",false},
            { "Kolob",false},
            { "Kunlun Mountain",false},
            { "Kvenland",false},
            { "Kyöpelinvuori",false},
            { "La Ciudad Blanca",false},
            { "Laestrygon",false},
            { "Lake Parime",false},
            { "Lemuria",false},
            { "Lintukoto",false},
            { "Lyonesse",false},
            { "Mag Mell",false},
            { "Meropis",false},
            { "Mictlan",false},
            { "Mount Olympus",false},
            { "Mount Penglai",false},
            { "Mu",false},
            { "Muspelheim",false},
            { "Nibiru",false},
            { "Niflheim",false},
            { "Niflhel",false},
            { "Norumbega",false},
            { "Nysa",false},
            { "Paititi",false},
            { "Pandæmonium",false},
            { "Purgatory",false},
            { "Ram Setu",false},
            { "Quivira",false},
            { "Cíbola",false},
            { "Scholomance",false},
            { "Sierra de la Plata",false},
            { "Shambhala",false},
            { "Shangri-La",false},
            { "Sodom",false},
            { "Gomorrah",false},
            { "Suddene",false},
            { "Summerland",false},
            { "Svartálfaheimr",false},
            { "Tartarus",false},
            { "Takama-ga-hara",false},
            { "Themiscyra",false},
            { "Thule",false},
            { "Thuvaraiyam",false},
            { "Valhalla",false},
            { "Vanaheimr",false},
            { "Westernesse",false},
            { "Xibalba",false},
            { "Ys",false},
            { "Zerzura Saharan",false},
            { "Zion",false},
        };
        private readonly ConcurrentDictionary<string, List<string>> _activeRooms = new ConcurrentDictionary<string, List<string>>();

        public GroupsManager(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gets the current active room and its index.
        /// </summary>
        public (string roomName,string index) GetNewRoom()
        {
            //TODO refactor
            if (!string.IsNullOrEmpty(_currentGroup) && _rooms[_currentGroup]) return (_currentGroup,_activeRooms.Keys.IndexOf(_currentGroup).ToString());
            var (key, _) = _rooms.Where(x => x.Value == false).ToList().ElementAt(new Random().Next(_rooms.Count));
            _rooms[key] = true;
            _currentGroup = key;
            CreateTimer();

            return (key,_activeRooms.Keys.IndexOf(_currentGroup).ToString());
        }

        /// <summary>
        /// Creates a timer.
        /// </summary>
        private void CreateTimer()
        {
            var timer = new Timer(TimerCallback, null, 60000, 0);
        }

        /// <summary>
        /// Callback of timer.
        /// </summary>
        /// <param name="state"></param>
        private void TimerCallback(object state)
        {
            CloseGroup(_currentGroup);
            _hubContext.Clients.All.SendAsync("Close");
           

        }

        /// <summary>
        /// Add user to a specific group.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="groupName"></param>
        public void AddUserToGroup(string connectionId, string groupName)
        {
            if (!_activeRooms.ContainsKey(groupName))
                _activeRooms.TryAdd(groupName, new List<string>());

            _activeRooms[groupName].Add(connectionId);
        }

        /// <summary>
        /// Closes a specific group.
        /// </summary>
        /// <param name="groupName"></param>
        private void CloseGroup(string groupName)
        {
            if (groupName.Equals(_currentGroup))
                _currentGroup = null;
            if (!_activeRooms.ContainsKey(groupName)) return;
            foreach (var connectionId in _activeRooms[groupName])
            {

                _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
            }
        }


    }
}
