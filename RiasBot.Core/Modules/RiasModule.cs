﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RiasBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RiasBot.Modules
{
    public abstract class RiasModule : ModuleBase
    {
        public readonly string ModuleTypeName;
        public readonly string LowerModuleTypeName;

        protected RiasModule(bool isTopLevelModule = true)
        {
            ModuleTypeName = isTopLevelModule ? this.GetType().Name : this.GetType().DeclaringType.Name;
            LowerModuleTypeName = ModuleTypeName.ToLowerInvariant();
        }
    }

    public abstract class RiasModule<TService> : RiasModule where TService : IRService
    {
        public TService _service { get; set; }

        public RiasModule(bool isTopLevel = true) : base(isTopLevel)
        {
        }
    }

    public abstract class RiasSubmodule : RiasModule
    {
        protected RiasSubmodule() : base(false) { }
    }

    public abstract class RiasSubmodule<TService> : RiasModule<TService> where TService : IRService
    {
        protected RiasSubmodule() : base(false)
        {
        }
    }
}
