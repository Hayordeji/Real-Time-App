﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IRedisCacheService
    {
        Task<T?> GetData<T>(string key);
        Task SetData<T>(string key, T value);
    }
}
