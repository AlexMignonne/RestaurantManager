using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantManager.Models
{
    public sealed class Table
    {
        private readonly ConcurrentDictionary<ClientsGroup, int> _clientsGroups;

        public Table(
            int size)
        {
            if (size < 2 ||
                size > 6)
                throw new ArgumentException(
                    "The table should be from 2 to 6 seats");

            _clientsGroups = new ConcurrentDictionary<ClientsGroup, int>();
            Id = Guid.NewGuid();
            Size = size;
        }

        public Guid Id { get; }

        // number of chairs 
        public int Size { get; }

        public IEnumerable<ClientsGroup> ClientsGroups =>
            _clientsGroups
                .Select(_ => _.Key)
                .ToList()
                .AsReadOnly();

        public bool IsFree =>
            !_clientsGroups.Any();

        public int FreeSeat()
        {
            return Size -
                   _clientsGroups
                       .Sum(_ => _.Key.Size);
        }

        public bool Arrive(
            ClientsGroup group)
        {
            if (FreeSeat() < group.Size)
                return false;

            _clientsGroups
                .TryAdd(group, default);

            return true;
        }

        public bool Leave(
            ClientsGroup group)
        {
            return _clientsGroups
                .Remove(group, out _);
        }
    }
}
