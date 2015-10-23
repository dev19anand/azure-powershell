﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Commands.Intune
{
    using System;
    using RestClient;
    using Microsoft.Azure.Commands.ResourceManager.Cmdlets.Extensions;
    using System.Linq;
    using System.Management.Automation;

    /// <summary>
    /// Cmdlet to get apps for iOS platform.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureRmIntuneiOSMAMApps"), OutputType(typeof(PSObject))]
    public sealed class GetIntuneiOSMAMAppsCmdlet : IntuneBaseCmdlet
    {
        /// <summary>
        /// Contains the cmdlet's execution logic.
        /// </summary>
        protected override void ProcessRecord()
        {
            Action action = () =>
            {
                string filter = string.Format("platform eq '{0}'", PlatformType.iOS.ToString().ToLower());
                var resources = this.IntuneClient.GetApplications(this.AsuHostName, filter);
                if (resources != null && resources.Value.Count > 0)
                {
                    var genericResources = resources.Value.Where(res => res != null).SelectArray(res => res.ToJToken().ToResource());
                    foreach (var batch in genericResources.Batch())
                    {
                        var powerShellObjects = batch.SelectArray(genericResource => genericResource.ToPsObject());
                        this.WriteObject(sendToPipeline: powerShellObjects, enumerateCollection: true);
                    }
                }
                else
                {
                    this.WriteObject("0 Apps returned");
                }
            };

            this.SafeExecutor(action);
        }

    }
}
