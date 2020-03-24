using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UtilZ.Dotnet.ConfigManagementTool.Model;
using UtilZ.Dotnet.DBIBase;
using UtilZ.Dotnet.DBIBase.Core;
using UtilZ.Dotnet.DBIBase.Interface;

namespace UtilZ.Dotnet.ConfigManagementTool.BLL
{
    public class ConfigBLL
    {
        private IDBAccess _dbAccess;
        private object inPara;

        public ConfigBLL()
        {

        }

        public void Init()
        {
            this._dbAccess = DBAccessManager.GetDBAccessInstance(1);
        }

        public void ModifyConfigParaKeyValue(ConfigParaKeyValue configParaKeyValue, List<int> validDomainIds)
        {
            using (var dbcon = this._dbAccess.CreateConnection(DBIBase.Model.DBVisitType.W))
            {
                IDbConnection con = dbcon.DbConnection;
                IDbTransaction transaction = con.BeginTransaction();
                var paraSign = this._dbAccess.ParaSign;

                if (configParaKeyValue.ID > 0)
                {
                    var cmdDel = this._dbAccess.CreateCommand(con);
                    cmdDel.CommandText = string.Format("delete from ConfigParaValidDomain where CID={0}CID", paraSign);
                    DBAccessEx.AddParameter(cmdDel, "CID", configParaKeyValue.ID);
                    cmdDel.Transaction = transaction;
                    cmdDel.ExecuteNonQuery();

                    var cmdUpdate = this._dbAccess.CreateCommand(con);
                    cmdUpdate.CommandText = string.Format("update ConfigParaKeyValue set Key={0}Key,Value={0}Value,GID={0}GID,Des={0}Des WHERE ID={0}ID", paraSign);
                    DBAccessEx.AddParameter(cmdUpdate, "Key", configParaKeyValue.Key);
                    DBAccessEx.AddParameter(cmdUpdate, "Value", configParaKeyValue.Value);
                    DBAccessEx.AddParameter(cmdUpdate, "GID", configParaKeyValue.GID);
                    DBAccessEx.AddParameter(cmdUpdate, "Des", configParaKeyValue.Des);
                    DBAccessEx.AddParameter(cmdUpdate, "ID", configParaKeyValue.ID);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();

                    var insertUpdate = this._dbAccess.CreateCommand(con);
                    insertUpdate.CommandText = string.Format("insert into ConfigParaValidDomain (CID,SID) Values ({0}CID,{0}SID)", paraSign);
                    insertUpdate.Transaction = transaction;
                    DBAccessEx.AddParameter(insertUpdate, "CID", configParaKeyValue.ID);
                    foreach (var validDomainId in validDomainIds)
                    {
                        DBAccessEx.AddParameter(insertUpdate, "SID", validDomainId);
                        insertUpdate.ExecuteNonQuery();
                        insertUpdate.Parameters.RemoveAt(1);
                    }
                }
                else
                {
                    var cmdInsertConfigParaKeyValue = this._dbAccess.CreateCommand(con);
                    cmdInsertConfigParaKeyValue.CommandText = string.Format("insert into ConfigParaKeyValue(Key,Value,GID,Des) Values({0}Key,{0}Value,{0}GID,{0}Des)", paraSign);
                    DBAccessEx.AddParameter(cmdInsertConfigParaKeyValue, "Key", configParaKeyValue.Key);
                    DBAccessEx.AddParameter(cmdInsertConfigParaKeyValue, "Value", configParaKeyValue.Value);
                    DBAccessEx.AddParameter(cmdInsertConfigParaKeyValue, "GID", configParaKeyValue.GID);
                    DBAccessEx.AddParameter(cmdInsertConfigParaKeyValue, "Des", configParaKeyValue.Des);
                    DBAccessEx.AddParameter(cmdInsertConfigParaKeyValue, "ID", configParaKeyValue.ID);
                    cmdInsertConfigParaKeyValue.Transaction = transaction;
                    cmdInsertConfigParaKeyValue.ExecuteNonQuery();

                    var insertUpdate = this._dbAccess.CreateCommand(con);
                    insertUpdate.CommandText = string.Format("insert into ConfigParaValidDomain (CID,SID) Values ({0}CID,{0}SID)", paraSign);
                    insertUpdate.Transaction = transaction;
                    DBAccessEx.AddParameter(insertUpdate, "CID", configParaKeyValue.ID);
                    foreach (var validDomainId in validDomainIds)
                    {
                        DBAccessEx.AddParameter(insertUpdate, "SID", validDomainId);
                        insertUpdate.ExecuteNonQuery();
                        insertUpdate.Parameters.RemoveAt(1);
                    }
                }
            }
        }

        public void ModifyValidDomain(List<int> addIDs, List<int> delIDs, ConfigParaServiceMap serviceMap)
        {
            using (var dbcon = this._dbAccess.CreateConnection(DBIBase.Model.DBVisitType.W))
            {
                IDbConnection con = dbcon.DbConnection;
                IDbTransaction transaction = con.BeginTransaction();
                var paraSign = this._dbAccess.ParaSign;

                if (delIDs.Count > 0)
                {
                    var cmdDel = this._dbAccess.CreateCommand(con);
                    cmdDel.Transaction = transaction;
                    cmdDel.CommandText = string.Format("delete from ConfigParaValidDomain where SID={0}SID AND CID={0}CID", paraSign);
                    DBAccessEx.AddParameter(cmdDel, "SID", serviceMap.ID);
                    IDbDataParameter delParameter = DBAccessEx.CreateParameter(cmdDel, "CID");

                    foreach (var delID in delIDs)
                    {
                        delParameter.Value = delID;
                        cmdDel.ExecuteNonQuery();
                    }
                }

                if (addIDs.Count > 0)
                {
                    var cmdInsert = this._dbAccess.CreateCommand(con);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.CommandText = string.Format("INSERT INTO ConfigParaValidDomain (SID, CID) VALUES ({0}SID, {0}CID)", paraSign);
                    IDbDataParameter insertSIDParameter = DBAccessEx.CreateParameter(cmdInsert, "SID");
                    IDbDataParameter insertCIDParameter = DBAccessEx.CreateParameter(cmdInsert, "CID");

                    foreach (var addID in addIDs)
                    {
                        insertSIDParameter.Value = serviceMap.ID;
                        insertCIDParameter.Value = addID;
                        cmdInsert.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<ConfigParaKeyValue> GetConfigParaKeyValueByServiceId(int serviceID)
        {
            string sqlStr = string.Format(@"SELECT * FROM ConfigParaKeyValue WHERE ID in (SELECT CID FROM ConfigParaValidDomain WHERE SID={0}SID)", this._dbAccess.ParaSign);
            var collection = new NDbParameterCollection();
            collection.Add("SID", serviceID);
            var items = this._dbAccess.Query<ConfigParaKeyValue>(sqlStr, collection);
            this.UpdateConfigParaGroup(items);
            return items;
        }

        private void UpdateConfigParaGroup(List<ConfigParaKeyValue> items)
        {
            var groupDic = this._dbAccess.QueryT<ConfigParaGroup>().ToDictionary((tmpItem) => { return tmpItem.ID; });
            foreach (var item in items)
            {
                if (groupDic.ContainsKey(item.GID))
                {
                    item.Group = groupDic[item.GID];
                }
            }
        }

        public void SaveConfigParaServiceMap(List<ConfigParaServiceMap> addItems, List<ConfigParaServiceMap> delItems, List<ConfigParaServiceMap> updateItems)
        {
            this._dbAccess.ExcuteAdoNetTransaction(new Tuple<List<ConfigParaServiceMap>, List<ConfigParaServiceMap>, List<ConfigParaServiceMap>>(addItems, delItems, updateItems), this.SaveConfigParaServiceMapTransaction);
        }

        private object SaveConfigParaServiceMapTransaction(IDbConnection con, IDbTransaction transaction, object para)
        {
            var inPara = para as Tuple<List<ConfigParaServiceMap>, List<ConfigParaServiceMap>, List<ConfigParaServiceMap>>;
            List<ConfigParaServiceMap> addItems = inPara.Item1;
            List<ConfigParaServiceMap> delItems = inPara.Item2;
            List<ConfigParaServiceMap> updateItems = inPara.Item3;

            var paraSign = this._dbAccess.ParaSign;

            if (delItems.Count > 0)
            {
                var cmdDel = this._dbAccess.CreateCommand(con);
                cmdDel.Transaction = transaction;
                cmdDel.CommandText = string.Format("delete from ConfigParaServiceMap where ID={0}ID", paraSign);
                foreach (var delItem in delItems)
                {
                    DBAccessEx.AddParameter(cmdDel, "ID", delItem.ID);
                    cmdDel.ExecuteNonQuery();
                }
            }

            if (addItems.Count > 0)
            {
                var cmdInsert = this._dbAccess.CreateCommand(con);
                cmdInsert.Transaction = transaction;
                cmdInsert.CommandText = string.Format("INSERT INTO ConfigParaServiceMap (ServiceMapID, Name, Des) VALUES ({0}ServiceMapID, {0}Name, {0}Des)", paraSign);
                foreach (var addItem in addItems)
                {
                    DBAccessEx.AddParameter(cmdInsert, "ServiceMapID", addItem.ServiceMapID);
                    DBAccessEx.AddParameter(cmdInsert, "Name", addItem.Name);
                    DBAccessEx.AddParameter(cmdInsert, "Des", addItem.Des);
                    cmdInsert.ExecuteNonQuery();
                    cmdInsert.Parameters.Clear();
                }
            }

            if (updateItems.Count > 0)
            {
                var cmdUpdate = this._dbAccess.CreateCommand(con);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.CommandText = string.Format("UPDATE ConfigParaServiceMap SET ServiceMapID={0}ServiceMapID, Name={0}Name,Des={0}Des Where ID={0}ID", paraSign);
                foreach (var updateItem in updateItems)
                {
                    DBAccessEx.AddParameter(cmdUpdate, "ServiceMapID", updateItem.ServiceMapID);
                    DBAccessEx.AddParameter(cmdUpdate, "Name", updateItem.Name);
                    DBAccessEx.AddParameter(cmdUpdate, "Des", updateItem.Des);
                    DBAccessEx.AddParameter(cmdUpdate, "ID", updateItem.ID);
                    cmdUpdate.ExecuteNonQuery();
                    cmdUpdate.Parameters.Clear();
                }
            }

            return null;
        }

        public void SaveGroup(List<ConfigParaGroup> addItems, List<ConfigParaGroup> delItems, List<ConfigParaGroup> updateItems)
        {
            using (var dbcon = this._dbAccess.CreateConnection(DBIBase.Model.DBVisitType.W))
            {
                IDbConnection con = dbcon.DbConnection;
                IDbTransaction transaction = con.BeginTransaction();

                var paraSign = this._dbAccess.ParaSign;
                if (delItems.Count > 0)
                {
                    var cmdDel = this._dbAccess.CreateCommand(con);
                    cmdDel.Transaction = transaction;
                    cmdDel.CommandText = string.Format("delete from ConfigParaGroup where ID={0}ID", paraSign);
                    var delParameter = DBAccessEx.CreateParameter(cmdDel, "ID");
                    foreach (var delItem in delItems)
                    {
                        delParameter.Value = delItem.ID;
                        cmdDel.ExecuteNonQuery();
                    }
                }

                if (addItems.Count > 0)
                {
                    var cmdInsert = this._dbAccess.CreateCommand(con);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.CommandText = string.Format("INSERT INTO ConfigParaGroup (Name, Des) VALUES ({0}Name,{0}Des)", paraSign);
                    foreach (var addItem in addItems)
                    {
                        DBAccessEx.AddParameter(cmdInsert, "Name", addItem.Name);
                        DBAccessEx.AddParameter(cmdInsert, "Des", addItem.Des);
                        cmdInsert.ExecuteNonQuery();
                        cmdInsert.Parameters.Clear();
                    }
                }

                if (updateItems.Count > 0)
                {
                    var cmdUpdate = this._dbAccess.CreateCommand(con);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.CommandText = string.Format("UPDATE ConfigParaGroup SET Name={0}Name,Des={0}Des Where ID={0}ID", paraSign);
                    foreach (var updateItem in updateItems)
                    {
                        DBAccessEx.AddParameter(cmdUpdate, "Name", updateItem.Name);
                        DBAccessEx.AddParameter(cmdUpdate, "Des", updateItem.Des);
                        DBAccessEx.AddParameter(cmdUpdate, "ID", updateItem.ID);
                        cmdUpdate.ExecuteNonQuery();
                        cmdUpdate.Parameters.Clear();
                    }
                }

            }
        }

        public void SaveConfigParaKeyValueEdit(List<ConfigParaKeyValue> addItems, List<ConfigParaKeyValue> delItems, List<ConfigParaKeyValue> updateItems)
        {
            using (var dbcon = this._dbAccess.CreateConnection(DBIBase.Model.DBVisitType.W))
            {
                IDbConnection con = dbcon.DbConnection;
                IDbTransaction transaction = con.BeginTransaction();
                var paraSign = this._dbAccess.ParaSign;
                try
                {
                    if (delItems.Count > 0)
                    {
                        var cmdDel = this._dbAccess.CreateCommand(con);
                        cmdDel.Transaction = transaction;
                        cmdDel.CommandText = string.Format("delete from ConfigParaKeyValue where ID={0}ID", paraSign);
                        foreach (var delItem in delItems)
                        {
                            DBAccessEx.AddParameter(cmdDel, "ID", delItem.ID);
                            cmdDel.ExecuteNonQuery();
                        }
                    }

                    if (addItems.Count > 0)
                    {
                        var cmdInsert = this._dbAccess.CreateCommand(con);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.CommandText = string.Format("INSERT INTO ConfigParaKeyValue (Key, Value, GID, Des) VALUES ({0}Key,{0}Value,{0}GID,{0}Des)", paraSign);
                        foreach (var addItem in addItems)
                        {
                            DBAccessEx.AddParameter(cmdInsert, "Key", addItem.Key);
                            DBAccessEx.AddParameter(cmdInsert, "Value", addItem.Value);
                            DBAccessEx.AddParameter(cmdInsert, "GID", addItem.GID);
                            DBAccessEx.AddParameter(cmdInsert, "Des", addItem.Des);
                            cmdInsert.ExecuteNonQuery();
                            cmdInsert.Parameters.Clear();
                        }
                    }

                    if (updateItems.Count > 0)
                    {
                        var cmdUpdate = this._dbAccess.CreateCommand(con);
                        cmdUpdate.Transaction = transaction;
                        cmdUpdate.CommandText = string.Format("UPDATE ConfigParaKeyValue SET Key={0}Key,Value={0}Value,GID={0}GID,Des={0}Des Where ID={0}ID", paraSign);
                        foreach (var updateItem in updateItems)
                        {
                            DBAccessEx.AddParameter(cmdUpdate, "Key", updateItem.Key);
                            DBAccessEx.AddParameter(cmdUpdate, "Value", updateItem.Value);
                            DBAccessEx.AddParameter(cmdUpdate, "GID", updateItem.GID);
                            DBAccessEx.AddParameter(cmdUpdate, "Des", updateItem.Des);
                            DBAccessEx.AddParameter(cmdUpdate, "ID", updateItem.ID);
                            cmdUpdate.ExecuteNonQuery();
                            cmdUpdate.Parameters.Clear();
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<ConfigParaServiceMap> GetValidDomainConfigParaServiceMap(ConfigParaKeyValue configParaKeyValue)
        {
            var collection = new NDbParameterCollection();
            string sqlStr = string.Format(@"SELECT * from ConfigParaServiceMap WHERE ID in (select sid from ConfigParaValidDomain WHERE cid={0}cid)", this._dbAccess.ParaSign);
            collection.Add("cid", configParaKeyValue.ID);
            return this._dbAccess.QueryT<ConfigParaServiceMap>(sqlStr, collection);
        }

        public List<ConfigParaKeyValue> GetGroupConfigParaKeyValue(ConfigParaGroup selectedItem)
        {
            List<ConfigParaKeyValue> configParas;
            if (selectedItem != null)
            {
                var query = new ConfigParaKeyValue();
                query.GID = selectedItem.ID;
                var conditionProperties = new List<string>();
                conditionProperties.Add(nameof(ConfigParaKeyValue.GID));
                configParas = this._dbAccess.QueryT<ConfigParaKeyValue>(query, conditionProperties);
                foreach (var item in configParas)
                {
                    item.Group = selectedItem;
                }
            }
            else
            {
                configParas = this._dbAccess.QueryT<ConfigParaKeyValue>();
                this.UpdateConfigParaGroup(configParas);
            }

            return configParas;
        }

        public List<ConfigParaGroup> GetAllConfigParaGroup()
        {
            var groups = this._dbAccess.QueryT<ConfigParaGroup>();
            if (groups.Count == 0)
            {
                var defaultGroup = new ConfigParaGroup();
                defaultGroup.Name = "默认组";
                defaultGroup.Des = "默认组";
                this._dbAccess.InsertT<ConfigParaGroup>(defaultGroup);
                groups = this._dbAccess.QueryT<ConfigParaGroup>();
            }

            return groups;
        }

        public List<ConfigParaKeyValue> GetAllConfigParaKeyValue()
        {
            return this._dbAccess.QueryT<ConfigParaKeyValue>();
        }

        public List<ConfigParaServiceMap> GetAllConfigParaServiceMap()
        {
            return this._dbAccess.QueryT<ConfigParaServiceMap>();
        }

        public List<ConfigParaServiceMap3> GetConfigParaServiceMap(int id)
        {
            string sqlStr = string.Format("SELECT * FROM ConfigParaServiceMap WHERE ID IN (SELECT SID from ConfigParaValidDomain WHERE CID={0}CID)", this._dbAccess.ParaSign);
            NDbParameterCollection collection = new NDbParameterCollection();
            collection.Add("CID", id);
            var validServiceDic = this._dbAccess.QueryT<ConfigParaServiceMap>(sqlStr, collection).ToDictionary((tmpItem) => { return tmpItem.ID; });

            var allServcieMap = this._dbAccess.QueryT<ConfigParaServiceMap>();
            List<ConfigParaServiceMap3> items = new List<ConfigParaServiceMap3>();
            foreach (var servcieMap in allServcieMap)
            {
                items.Add(new ConfigParaServiceMap3(servcieMap, validServiceDic.ContainsKey(servcieMap.ID)));
            }

            return items;
        }
    }
}
