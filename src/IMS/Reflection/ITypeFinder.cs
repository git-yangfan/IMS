﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Reflection
{
    public interface ITypeFinder
    {
        Type[] Find(Func<Type, bool> predicate);

        Type[] FindAll();
    }
}
