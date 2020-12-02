using Dapper;
using Dapper.FastCrud;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace PGtraining.FileImportService
{
    public class DbLib
    {
        public static bool InsertOrder(Order order)
        {
            var result = false;

            using (var connection = new SqlConnection())
            using (var command = new SqlCommand())
            {
                try
                {
                    connection.ConnectionString = Properties.Settings.Default.ConnectionString;
                    connection.Open();

                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            connection.Execute(
                                @"INSERT INTO Orders VALUES (
                                  @OrderNo
                                  ,@StudyDate
                                  ,@ProcessingType
                                  ,@InspectionTypeCode
                                  ,@InspectionTypeName
                                  ,@PatientId
                                  ,@PatientNameKanji
                                  ,@PatientNameKana
                                  ,@PatientBirth
                                  ,@PatientSex)",
                                order, tran);

                            var orderNo = order.OrderNo;

                            for (var i = 0; i < order.MenuCodes.Count(); i++)
                            {
                                connection.Execute(
                                    @"INSERT INTO Menu VALUES (
                                          @OrderNo
                                          ,@MenuCode
                                          ,@MenuName)",
                                    new
                                    {
                                        OrderNo = orderNo
                                        ,
                                        MenuCode = order.MenuCodes[i]
                                        ,
                                        MenuName = order.MenuNames[i]
                                    }, tran);
                            }

                            tran.Commit();
                            result = true;
                        }
                        catch
                        {
                            tran.Rollback();
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }

        public static bool DeleteOrder(Order order)
        {
            var result = false;

            using (var connection = new SqlConnection())
            using (var command = new SqlCommand())
            {
                try
                {
                    connection.ConnectionString = Properties.Settings.Default.ConnectionString;
                    connection.Open();

                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            connection.Execute(
                                $"DELETE FROM Menu WHERE OrderNo=@OrderNo", new { OrderNo = order.OrderNo }, tran);

                            connection.Execute(
                                $"DELETE FROM Orders WHERE OrderNo=@OrderNo", new { OrderNo = order.OrderNo }, tran);

                            tran.Commit();
                            result = true;
                        }
                        catch
                        {
                            tran.Rollback();
                            result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }

        public static Order GetOrder(string orderNo)
        {
            Order result = new Order();

            using (var connection = new SqlConnection())
            using (var command = new SqlCommand())
            {
                try
                {
                    connection.ConnectionString = Properties.Settings.Default.ConnectionString;
                    connection.Open();

                    var orders = connection.Query<Order>("SELECT * FROM Orders WHERE OrderNo=@OrderNo", new { OrderNo = orderNo });
                    var menu = connection.Query("SELECT * FROM Menu WHERE OrderNo=@OrderNo", new { OrderNo = orderNo }).ToList();

                    if (0 < orders.Count())
                    {
                        result = orders.First() as Order;

                        for (var i = 0; i < menu.Count(); i++)
                        {
                            result.MenuCodes.Add(menu[i].MenuCode);
                            result.MenuNames.Add(menu[i].MenuName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
    }
}