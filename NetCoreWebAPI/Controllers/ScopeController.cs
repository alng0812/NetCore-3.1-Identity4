using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreWebAPI.Service;

namespace NetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScopeController : ControllerBase
    {
        IScopeService orderService1;
        IScopeService orderService2;

        public ScopeController(IScopeService orderService1, IScopeService orderService2)
        {
            this.orderService1 = orderService1;
            this.orderService2 = orderService2;
        }

        [HttpGet]
        public string Get()
        {
            Console.WriteLine($"{this.orderService1}\r\n{this.orderService2} \r\n ------");
            return "helloworld";
        }
    }
}
