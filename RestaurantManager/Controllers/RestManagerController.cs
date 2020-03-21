using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantManager.Managers;
using RestaurantManager.Models;

namespace RestaurantManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestManagerController : ControllerBase
    {
        private readonly ILogger<RestManagerController> _logger;
        private readonly RestManager _restManager;

        public RestManagerController(
            ILogger<RestManagerController> logger,
            RestManager restManager)
        {
            _logger = logger;
            _restManager = restManager;
        }

        [HttpPost("{size}")]
        public IActionResult Arrive(
            int size)
        {
            var clientsGroup = new ClientsGroup(size);

             _restManager
                .OnArrive(clientsGroup);

             return Ok(clientsGroup);
        }

        [HttpDelete("{id}")]
        public IActionResult Leave(
            Guid id)
        {
            var clientsGroup = _restManager
                .FindClientsGroupById(id);

            if (clientsGroup == null)
                return NotFound();

            _restManager
                .OnLeave(clientsGroup);

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Lookup(
            Guid id)
        {
            var clientsGroup = _restManager
                .FindClientsGroupById(id);

            if (clientsGroup == null)
                return NotFound();

            var result = _restManager
                .Lookup(clientsGroup);

            return result == null
                ? (IActionResult) NotFound()
                : Ok(result);
        }

        [HttpGet("tables")]
        public IActionResult Tables()
        {
            var result = _restManager
                .Tables;

            return result == null
                ? (IActionResult)NoContent()
                : Ok(result);
        }

        [HttpGet("client-group-queue")]
        public IActionResult ClientGroupQueue()
        {
            var result = _restManager
                .ClientsGroupQueue;

            return result == null
                ? (IActionResult)NoContent()
                : Ok(result);
        }
    }
}
