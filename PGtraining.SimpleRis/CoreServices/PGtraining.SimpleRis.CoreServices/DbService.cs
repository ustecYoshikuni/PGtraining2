using PGtraining.SimpleRis.Core.Entity;
using PGtraining.SimpleRis.CoreServices.Interface;
using System;
using System.Collections.Generic;

namespace PGtraining.SimpleRis.CoreServices
{
    public class DbService : IDbService
    {
        public bool DeleteMenu(string orderNo)
        {
            throw new NotImplementedException();
        }

        public bool DeleteOrder(string orderNo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Menu> EditMenu(IEnumerable<Menu> menu)
        {
            throw new NotImplementedException();
        }

        public Order EditOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Menu> GetMenu()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrder()
        {
            throw new NotImplementedException();
        }

        public bool InsertMenu(IEnumerable<Menu> menu)
        {
            throw new NotImplementedException();
        }

        public bool InsertOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}