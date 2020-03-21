using System;

namespace RestaurantManager.Models
{
    public class ClientsGroup
    {
        public ClientsGroup(
            int size)
        {
            if (size < 1 ||
                size > 6)
                throw new ArgumentException(
                    "Only groups from 1 to 6 people are served");

            Id = Guid.NewGuid();
            Size = size;
        }

        public Guid Id { get; }

        // number of clients
        public int Size { get; }
    }
}
