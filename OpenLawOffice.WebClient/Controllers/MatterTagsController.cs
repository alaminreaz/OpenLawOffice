﻿// -----------------------------------------------------------------------
// <copyright file="MatterTagsController.cs" company="Nodine Legal, LLC">
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

    public class MatterTagsController : BaseController
    {
        //
        // GET: /MatterTags/Details/{Guid}
        [SecurityFilter(SecurityAreaName = "Matters", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Read)]
        public ActionResult Details(Guid id)
        {
            ViewModels.Matters.MatterTagViewModel viewModel = null;
            Common.Models.Matters.MatterTag model = OpenLawOffice.Data.Matters.MatterTag.Get(id);
            viewModel = Mapper.Map<ViewModels.Matters.MatterTagViewModel>(model);
            PopulateCoreDetails(viewModel);
            return View(viewModel);
        }

        //
        // GET: /MatterTags/Create/{Guid}
        [SecurityFilter(SecurityAreaName = "Matters", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Create)]
        public ActionResult Create(Guid id)
        {
            Common.Models.Matters.Matter matter = OpenLawOffice.Data.Matters.Matter.Get(id);

            return View(new ViewModels.Matters.MatterTagViewModel()
            {
                Matter = Mapper.Map<ViewModels.Matters.MatterViewModel>(matter)
            });
        } 

        //
        // POST: /MatterTags/Create/{Guid}
        [SecurityFilter(SecurityAreaName = "Matters", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Create)]
        [HttpPost]
        public ActionResult Create(ViewModels.Matters.MatterTagViewModel viewModel)
        {
            try
            {
                Common.Models.Security.User currentUser = UserCache.Instance.Lookup(Request);
                Common.Models.Matters.MatterTag model = Mapper.Map<Common.Models.Matters.MatterTag>(viewModel);
                model.Matter = new Common.Models.Matters.Matter() { Id = Guid.Parse(RouteData.Values["Id"].ToString()) };
                model.TagCategory = Mapper.Map<Common.Models.Tagging.TagCategory>(viewModel.TagCategory);
                model = OpenLawOffice.Data.Matters.MatterTag.Create(model, currentUser);
                return RedirectToAction("Tags", "Matters", new { Id = model.Matter.Id.Value.ToString() });
            }
            catch (Exception)
            {
                Common.Models.Matters.Matter matter = OpenLawOffice.Data.Matters.Matter.Get(
                    Guid.Parse(RouteData.Values["Id"].ToString()));

                return View(new ViewModels.Matters.MatterTagViewModel()
                {
                    Matter = Mapper.Map<ViewModels.Matters.MatterViewModel>(matter)
                });
            }
        }
        
        //
        // GET: /MatterTags/Edit/{Guid}
        [SecurityFilter(SecurityAreaName = "Matters", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Modify)]
        public ActionResult Edit(Guid id)
        {
            ViewModels.Matters.MatterTagViewModel viewModel = null;
            Common.Models.Matters.MatterTag model = OpenLawOffice.Data.Matters.MatterTag.Get(id);
            model.Matter = Data.Matters.Matter.Get(model.Matter.Id.Value);
            viewModel = Mapper.Map<ViewModels.Matters.MatterTagViewModel>(model);
            viewModel.Matter = Mapper.Map<ViewModels.Matters.MatterViewModel>(model.Matter);
            PopulateCoreDetails(viewModel);
            return View(viewModel);
        }

        //
        // POST: /MatterTags/Edit/{Guid}
        [SecurityFilter(SecurityAreaName = "Matters", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Modify)]
        [HttpPost]
        public ActionResult Edit(Guid id, ViewModels.Matters.MatterTagViewModel viewModel)
        {
            try
            {
                Common.Models.Security.User currentUser = UserCache.Instance.Lookup(Request);
                Common.Models.Matters.MatterTag model = Mapper.Map<Common.Models.Matters.MatterTag>(viewModel);
                model.TagCategory = Mapper.Map<Common.Models.Tagging.TagCategory>(viewModel.TagCategory);
                model.Matter = Data.Matters.MatterTag.Get(id).Matter;
                model = OpenLawOffice.Data.Matters.MatterTag.Edit(model, currentUser);
                return RedirectToAction("Tags", "Matters", new { Id = model.Matter.Id.Value.ToString() });
            }
            catch
            {
                return View(viewModel);
            }
        }

        //
        // GET: /MatterTags/Delete/{Guid}
        [SecurityFilter(SecurityAreaName = "Matters", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Disable)]
        public ActionResult Delete(Guid id)
        {
            return Details(id);
        }

        //
        // POST: /MatterTags/Delete/{Guid}
        [SecurityFilter(SecurityAreaName = "Matters", IsSecuredResource = false,
            Permission = Common.Models.PermissionType.Disable)]
        [HttpPost]
        public ActionResult Delete(Guid id, ViewModels.Matters.MatterTagViewModel viewModel)
        {
            try
            {
                Common.Models.Security.User currentUser = UserCache.Instance.Lookup(Request);
                Common.Models.Matters.MatterTag model = Mapper.Map<Common.Models.Matters.MatterTag>(viewModel);
                model = OpenLawOffice.Data.Matters.MatterTag.Disable(model, currentUser);
                return RedirectToAction("Tags", "Matters", new { Id = model.Matter.Id.Value.ToString() });
            }
            catch
            {
                return Details(id);
            }
        }
    }
}
