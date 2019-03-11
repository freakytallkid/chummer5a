﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace SINners.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class SINnerGroup
    {
        /// <summary>
        /// Initializes a new instance of the SINnerGroup class.
        /// </summary>
        public SINnerGroup() { }

        /// <summary>
        /// Initializes a new instance of the SINnerGroup class.
        /// </summary>
        public SINnerGroup(Guid? id = default(Guid?), bool? isPublic = default(bool?), string gameMasterUsername = default(string), SINnerGroupSetting mySettings = default(SINnerGroupSetting), string groupname = default(string), IList<SINnerGroup> myGroups = default(IList<SINnerGroup>), SINnerGroup myParentGroup = default(SINnerGroup), string myAdminIdentityRole = default(string))
        {
            Id = id;
            IsPublic = isPublic;
            GameMasterUsername = gameMasterUsername;
            MySettings = mySettings;
            Groupname = groupname;
            MyGroups = myGroups;
            MyParentGroup = myParentGroup;
            MyAdminIdentityRole = myAdminIdentityRole;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isPublic")]
        public bool? IsPublic { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gameMasterUsername")]
        public string GameMasterUsername { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "mySettings")]
        public SINnerGroupSetting MySettings { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "groupname")]
        public string Groupname { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "myGroups")]
        public IList<SINnerGroup> MyGroups { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "myParentGroup")]
        public SINnerGroup MyParentGroup { get; set; }

        /// <summary>
        /// Only users of the specified Role can join this group
        /// </summary>
        [JsonProperty(PropertyName = "myAdminIdentityRole")]
        public string MyAdminIdentityRole { get; set; }

    }
}
