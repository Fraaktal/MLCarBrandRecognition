using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MLCarBrandRecognition
{
    public class MLHub : Hub
    {
        public async Task AskForBrand(byte[] image)
        {
            await Clients.Caller.SendAsync("ReceiveBrandResult", "test");
        }
    }
}
