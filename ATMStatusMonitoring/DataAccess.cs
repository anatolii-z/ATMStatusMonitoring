﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using System.Windows;

namespace ATMStatusMonitoring
{
    class DataAccess
    {
        CheckID check = new CheckID();
        public List<ATM> GetATM()   //data output method in DataGrid
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb"))) //open new connection
            {
                return connection.Query<ATM>("dbo.GetATM").ToList(); // procedure call GetATM
            }
        }
        public List<ATMStatus> GetATMStatus() //data output method in DataGridStatus
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                return connection.Query<ATMStatus>("dbo.GetATMStatus").ToList();
            }
        }

        public void AddATM(string name, string lastName, int serialNumber, string ip, string mask, string gateway, string address)  //method of adding a record when filling Last Name
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                List<ATM> atm = new List<ATM>
                {
                    new ATM { Number = name, LastNumber = lastName, SerialNumber = serialNumber, IP = ip, Mask = mask, Gateway = gateway, Address = address }
                };
                connection.Execute("dbo.InsertAndUpdateATM @Number, @LastNumber, @SerialNumber, @IP, @Mask, @Gateway, @Address", atm);
            }
        }
        public void AddATM(string name, int serialNumber, string ip, string mask, string gateway, string address)  //method of adding a record when empty Last Name
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                List<ATM> atm = new List<ATM>
                {
                    new ATM { Number = name, SerialNumber = serialNumber, IP = ip, Mask = mask, Gateway = gateway, Address = address }
                };
                connection.Execute("dbo.InsertATM @Number, @SerialNumber, @IP, @Mask, @Gateway, @Address", atm);
            }
        }

        public void DeleteATM(string name)
        {
            ClearNewNumber(check.CheckNewNumber(name));
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                connection.Execute("dbo.DeleteATM " + name + ", " + check.CheckIDATM(name));
            }
        }

        public void UpdateATMStatus(string name, string status, DateTime date, string user, string comment)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                int checkId = 0;
                checkId = check.CheckIDATM(name);
                if (check.CheckIDStatus(checkId))
                {
                    // Обновление текущей строки
                    List<ATMStatus> Status = new List<ATMStatus>
                    {
                        new ATMStatus { Id = checkId, Status = status, Date = date, Editor = user, Comment = comment}
                    };
                    connection.Execute("dbo.UpdateStatus @Id, @Status, @Date, @Editor, @Comment", Status);
                }
                else
                {
                    // Добавление новой записи если есть запись в ATM
                    if (checkId != 0)
                    {
                        List<ATMStatus> Status = new List<ATMStatus>
                        {
                            new ATMStatus { Id = checkId, Status = status, Date = date, Editor = user, Comment = comment}
                        };
                        connection.Execute("dbo.InsertStatus @Id, @Status, @Date, @Editor, @Comment", Status);
                    }
                    else
                        MessageBox.Show("Нет такого банкомата!!!");
                }
            }
        }

        public void DeleteATMStatus(string name)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                if (check.CheckIDATM(name) != 0)
                    connection.Execute("dbo.DeleteATMStatus @Id = " + check.CheckIDATM(name));
                else
                    MessageBox.Show("Нет такого банкомата для удаления его статуса!!!");
            }
        }

        public void UpdateATM(string name, string lastName, int serialNumber, string ip, string mask, string gateway, string address)
        {
            ClearNewNumber(check.CheckNewNumber(name));
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                List<ATM> atm = new List<ATM>
                {
                new ATM{ Number = name, LastNumber = lastName, SerialNumber = serialNumber, IP = ip, Mask = mask, Gateway = gateway, Address = address }
                };
                connection.Execute("dbo.UpdateATM @Number, @LastNumber, @SerialNumber, @IP, @Mask, @Gateway, @Address", atm);
            }
        }

        public void ClearNewNumber(int id) //clearing New Name by ID
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("ATMDb")))
            {
                connection.Execute("dbo.ClearNewNumber " + id);
            }
        }
    }
}