using PGtraining.SimpleRis.Core.Entity;
using System.Collections.Generic;

namespace PGtraining.SimpleRis.CoreServices.Interface
{
    public interface IDbService
    {
        IEnumerable<Order> GetOrder();

        IEnumerable<Menu> GetMenu();

        bool InsertOrder(Order order);

        bool InsertMenu(IEnumerable<Menu> menu);

        bool DeleteOrder(string orderNo);

        bool DeleteMenu(string orderNo);

        Order EditOrder(Order order);

        IEnumerable<Menu> EditMenu(IEnumerable<Menu> menu);
    }
}