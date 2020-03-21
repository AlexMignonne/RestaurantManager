using System;
using System.Collections.Generic;
using System.Linq;
using RestaurantManager.Models;

namespace RestaurantManager.Managers
{
    public sealed class RestManager
    {
        private readonly List<Table> _tables;
        private readonly List<ClientsGroup> _clientGroupQueue;

        public RestManager(
            IReadOnlyCollection<Table> tables)
        {
            if (tables == null)
                throw new ArgumentNullException(
                    nameof(tables));

            if (!tables.Any())
                throw new ArgumentException(
                    "No tables set");

            _tables = tables
                .OrderBy(_ => _.Size)
                .ToList();

            _clientGroupQueue = new List<ClientsGroup>();
        }

        public IReadOnlyCollection<Table> Tables => 
            _tables.AsReadOnly();

        public IReadOnlyCollection<ClientsGroup> ClientsGroupQueue =>
            _clientGroupQueue.AsReadOnly();

        public ClientsGroup? FindClientsGroupById(
            Guid id)
        {
            return _clientGroupQueue
                       .FirstOrDefault(_ => _.Id == id) ??
                   _tables
                       .Select(_ => _.ClientsGroups
                           .FirstOrDefault(group => group.Id == id))
                       .FirstOrDefault(_ => _ != null);
        }

        // new client(s) show up
        public void OnArrive(
            ClientsGroup group)
        {
            if (_clientGroupQueue
                .Any(_ => _.Size == group.Size))
                _clientGroupQueue
                    .Add(group);
            else if (!Arrive(group))
                _clientGroupQueue.Add(group);
        }

        private bool Arrive(
            ClientsGroup group)
        {
            var table = _tables
                            .FirstOrDefault(
                                _ => _.IsFree && _.Size >= group.Size) ??
                        _tables
                            .FirstOrDefault(
                                _ => _.FreeSeat() >= group.Size);

            return table?.Arrive(group) ?? false;
        }

        // client(s) leave, either served or simply abandoning the queue
        public void OnLeave(
            ClientsGroup group)
        {
            // If the group has exited the queue
            if (_clientGroupQueue
                .Contains(group))
            {
                _clientGroupQueue
                    .Remove(group);

                return;
            }

            // If the group left the table
            var table = _tables
                .FirstOrDefault(
                    _ => _.ClientsGroups
                        .Contains(group));

            // If the group did not work in RestManager
            if (table == null)
                return;

            table.Leave(group);

            while (true)
            {
                var groupInQueue = _clientGroupQueue
                    .FirstOrDefault(_ => _.Size <= table.FreeSeat());

                if (groupInQueue == null)
                    return;

                if (table.Arrive(groupInQueue))
                    _clientGroupQueue.Remove(groupInQueue);
            }
        }

        // return table where a given client group is seated, 
        // or null if it is still queuing or has already left
        public Table? Lookup(
            ClientsGroup group)
        {
            return _tables
                .FirstOrDefault(
                    table => table
                        .ClientsGroups
                        .Contains(group));
        }
    }
}
