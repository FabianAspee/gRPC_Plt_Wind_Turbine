﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component
{
    public interface IEventComponent
    {

    }
    public interface IEventComponent<T>:IEventComponent
    {
        public T ReturnComponent();
    }
}
