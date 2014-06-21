﻿// -----------------------------------------------------------------------
// <copyright file="Matter.cs" company="Nodine Legal, LLC">
// Licensed to Nodine Legal, LLC under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  Nodine Legal, LLC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>
// -----------------------------------------------------------------------

namespace OpenLawOffice.Data.Matters
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using AutoMapper;
    using Dapper;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Matter
    {
        public static Common.Models.Matters.Matter Get(Guid id)
        {
            return DataHelper.Get<Common.Models.Matters.Matter, DBOs.Matters.Matter>(
                "SELECT * FROM \"matter\" WHERE \"id\"=@id AND \"utc_disabled\" is null",
                new { id = id });
        }

        public static List<Common.Models.Matters.Matter> List()
        {
            return DataHelper.List<Common.Models.Matters.Matter, DBOs.Matters.Matter>(
                "SELECT * FROM \"matter\" WHERE \"utc_disabled\" is null");
        }

        public static List<Common.Models.Matters.Matter> ListChildren(Guid? parentId)
        {
            List<Common.Models.Matters.Matter> list = new List<Common.Models.Matters.Matter>();
            IEnumerable<DBOs.Matters.Matter> ie = null;
            using (IDbConnection conn = Database.Instance.GetConnection())
            {
                if (parentId.HasValue)
                    ie = conn.Query<DBOs.Matters.Matter>(
                        "SELECT \"matter\".* FROM \"matter\" " +
                        "WHERE \"matter\".\"utc_disabled\" is null  " +
                        "AND \"matter\".\"parent_id\"=@ParentId", new { ParentId = parentId.Value });
                else
                    ie = conn.Query<DBOs.Matters.Matter>(
                        "SELECT \"matter\".* FROM \"matter\" " +
                        "WHERE \"matter\".\"utc_disabled\" is null  " +
                        "AND \"matter\".\"parent_id\" is null");
            }

            foreach (DBOs.Matters.Matter dbo in ie)
                list.Add(Mapper.Map<Common.Models.Matters.Matter>(dbo));

            return list;
        }

        public static List<Common.Models.Matters.Matter> ListChildrenForContact(Guid? parentId, int contactId)
        {
            List<Common.Models.Matters.Matter> list = new List<Common.Models.Matters.Matter>();
            IEnumerable<DBOs.Matters.Matter> ie = null;
            using (IDbConnection conn = Database.Instance.GetConnection())
            {
                if (parentId.HasValue)
                    ie = conn.Query<DBOs.Matters.Matter>(
                        "SELECT \"matter\".* FROM \"matter\" " +
                        "WHERE \"matter\".\"utc_disabled\" is null  " +
                        "AND \"matter\".\"parent_id\"=@ParentId " + 
                        "AND \"matter\".\"id\" IN (SELECT \"matter_id\" FROM \"matter_contact\" WHERE \"contact_id\"=@ContactId)", 
                        new { ParentId = parentId.Value, ContactId = contactId });
                else
                    ie = conn.Query<DBOs.Matters.Matter>(
                        "SELECT \"matter\".* FROM \"matter\" " +
                        "WHERE\"matter\".\"utc_disabled\" is null  " +
                        "AND \"matter\".\"parent_id\" is null " +
                        "AND \"matter\".\"id\" IN (SELECT \"matter_id\" FROM \"matter_contact\" WHERE \"contact_id\"=@ContactId)",
                        new { ContactId = contactId });
            }

            foreach (DBOs.Matters.Matter dbo in ie)
                list.Add(Mapper.Map<Common.Models.Matters.Matter>(dbo));

            return list;
        }

        public static List<Common.Models.Matters.Matter> ListAllMattersForContact(int contactId)
        {
            List<Common.Models.Matters.Matter> list = new List<Common.Models.Matters.Matter>();
            IEnumerable<DBOs.Matters.Matter> ie = null;
            using (IDbConnection conn = Database.Instance.GetConnection())
            {
                ie = conn.Query<DBOs.Matters.Matter>(
                    "SELECT \"matter\".* FROM \"matter\" " +
                    "AND \"matter\".\"utc_disabled\" is null  " +
                    "AND \"matter\".\"id\" IN (SELECT \"matter_id\" FROM \"matter_contact\" WHERE \"contact_id\"=@ContactId)",
                    new { ContactId = contactId });
            }

            foreach (DBOs.Matters.Matter dbo in ie)
                list.Add(Mapper.Map<Common.Models.Matters.Matter>(dbo));

            return list;
        }

        public static Common.Models.Matters.Matter Create(Common.Models.Matters.Matter model,
            Common.Models.Account.Users creator)
        {
            // Matter
            if (!model.Id.HasValue) model.Id = Guid.NewGuid();
            model.CreatedBy = model.ModifiedBy = creator;
            model.Created = model.Modified = DateTime.UtcNow;
            DBOs.Matters.Matter dbo = Mapper.Map<DBOs.Matters.Matter>(model);

            using (IDbConnection conn = Database.Instance.GetConnection())
            {
                conn.Execute("INSERT INTO \"matter\" (\"id\", \"title\", \"active\", \"parent_id\", \"synopsis\", \"utc_created\", \"utc_modified\", \"created_by_user_pid\", \"modified_by_user_pid\") " +
                    "VALUES (@Id, @Title, @Active, @ParentId, @Synopsis, @UtcCreated, @UtcModified, @CreatedByUserPId, @ModifiedByUserPId)",
                    dbo);
            }

            return model;
        }

        public static Common.Models.Matters.Matter Edit(Common.Models.Matters.Matter model,
            Common.Models.Account.Users modifier)
        {
            model.ModifiedBy = modifier;
            model.Modified = DateTime.UtcNow;
            DBOs.Matters.Matter dbo = Mapper.Map<DBOs.Matters.Matter>(model);

            using (IDbConnection conn = Database.Instance.GetConnection())
            {
                conn.Execute("UPDATE \"matter\" SET " +
                    "\"title\"=@Title, \"active\"=@Active, \"parent_id\"=@ParentId, \"synopsis\"=@Synopsis, \"utc_modified\"=@UtcModified, \"modified_by_user_pid\"=@ModifiedByUserPId " +
                    "WHERE \"id\"=@Id", dbo);
            }

            return model;
        }
    }
}