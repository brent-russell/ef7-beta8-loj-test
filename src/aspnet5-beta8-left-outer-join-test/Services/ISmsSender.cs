using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet5_beta8_left_outer_join_test.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
