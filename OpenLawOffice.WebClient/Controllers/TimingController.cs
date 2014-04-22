﻿// -----------------------------------------------------------------------
// <copyright file="TimingController.cs" company="Nodine Legal, LLC">
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

namespace OpenLawOffice.WebClient.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Data;
    using AutoMapper;

    public class TimingController : BaseController
    {
        [SecurityFilter(SecurityAreaName = "Timing", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Read)]
        public ActionResult Details(Guid id)
        {
            Common.Models.Timing.Time model = OpenLawOffice.Data.Timing.Time.Get(id);
            ViewModels.Timing.TimeViewModel viewModel = Mapper.Map<ViewModels.Timing.TimeViewModel>(model);
            Common.Models.Contacts.Contact contact = OpenLawOffice.Data.Contacts.Contact.Get(viewModel.Worker.Id.Value);
            viewModel.Worker = Mapper.Map<ViewModels.Contacts.ContactViewModel>(contact);
            Common.Models.Tasks.Task task = OpenLawOffice.Data.Timing.Time.GetRelatedTask(model.Id.Value);
            PopulateCoreDetails(viewModel);
            ViewData["TaskId"] = task.Id.Value;
            return View(viewModel);
        }

        [SecurityFilter(SecurityAreaName = "Timing", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Modify)]
        public ActionResult Edit(Guid id)
        {
            Common.Models.Timing.Time model = OpenLawOffice.Data.Timing.Time.Get(id);
            ViewModels.Timing.TimeViewModel viewModel = Mapper.Map<ViewModels.Timing.TimeViewModel>(model);
            Common.Models.Contacts.Contact contact = OpenLawOffice.Data.Contacts.Contact.Get(viewModel.Worker.Id.Value);
            viewModel.Worker = Mapper.Map<ViewModels.Contacts.ContactViewModel>(contact);
            Common.Models.Tasks.Task task = OpenLawOffice.Data.Timing.Time.GetRelatedTask(model.Id.Value);
            ViewData["TaskId"] = task.Id.Value;
            return View(viewModel);
        }        

        [SecurityFilter(SecurityAreaName = "Timing", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Modify)]
        [HttpPost]
        public ActionResult Edit(Guid id, ViewModels.Timing.TimeViewModel viewModel)
        {
            try
            {
                Common.Models.Security.User currentUser = UserCache.Instance.Lookup(Request);
                Common.Models.Timing.Time model = Mapper.Map<Common.Models.Timing.Time>(viewModel);
                model = OpenLawOffice.Data.Timing.Time.Edit(model, currentUser);
                return RedirectToAction("Details", new { Id = id });
            }
            catch
            {
                return View(viewModel);
            }
        }
    }
}
