﻿using SistemaVenta.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Services.Deal
{
    public interface IRolService
    {
        Task<List<RolDTO>> Lista();
    }
}
