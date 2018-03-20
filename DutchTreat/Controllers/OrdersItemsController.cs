using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DutchTreat.Controllers
{
    [Route("api/orders/{orderid}/items")]
    // No cookie authentication, just token credentials.
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersItemController:Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<Product> _logger;
        private readonly IMapper _mapper;

        public OrdersItemController(IDutchRepository repository, ILogger<Product> logger, IMapper mapper)
        {            
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int orderId)
        {
            try
            {
                var order = _repository.GetOrderById(User.Identity.Name, orderId);

                if (order != null)
                    return Ok(_mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemViewModel>>(order.Items));

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get order items {ex}");
                return BadRequest("Failed to get order items");
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int orderId, int id)
        {
            try
            {
                var order = _repository.GetOrderById(User.Identity.Name, orderId);

                if (order != null)
                {
                    var item = order.Items.Where(i => i.Id == id).FirstOrDefault();
                    if (item != null)
                    {
                        return Ok(_mapper.Map<OrderItem, OrderItemViewModel>(item));
                    }
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get order items {ex}");
                return BadRequest("Failed to get order items");
            }
        }
    }
}
