using Dapper;
using Dapper.FastCrud;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace PGtraining.FileImportService
{
    public class DbLib
    {
        public static string InsertOrder(Order order)
        {
            var result = "";

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
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                }
            }
            return result;
        }

        public static string DeleteOrder(Order order)
        {
            var result = "";

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
                                $"DELETE FROM Orders WHERE OrderNo=@OrderNo", new { OrderNo = order.OrderNo }, tran);

                            connection.Execute(
                                $"DELETE FROM Menu WHERE OrderNo=@OrderNo", new { OrderNo = order.OrderNo }, tran);

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                }
            }
            return result;
        }

        public static Order GetOrder(string orderNo)
        {
            Order result = null;

            using (var connection = new SqlConnection())
            using (var command = new SqlCommand())
            {
                try
                {
                    connection.ConnectionString = Properties.Settings.Default.ConnectionString;
                    connection.Open();

                    var order = connection.Query<Order>(
                        $"SELECT * FROM Orders WHERE OrderNo=@OrderNo", new { OrderNo = orderNo }).First();
                    var menu = connection.Query(
                        $"SELECT * FROM Menu WHERE OrderNo=@OrderNo", new { OrderNo = orderNo }).ToList();

                    for (var i = 0; i < menu.Count(); i++)
                    {
                        order.MenuCodes.Add(menu[i].MenuCode);
                        order.MenuNames.Add(menu[i].MenuName);
                    }

                    result = order;
                }
                catch (Exception ex)
                {
                    var text = ex.ToString();
                }
            }
            return result;
        }
    }
}